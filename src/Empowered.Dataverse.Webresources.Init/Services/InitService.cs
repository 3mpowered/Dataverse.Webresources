using System.IO.Abstractions;
using System.Reflection;
using CliWrap.Exceptions;
using Empowered.Dataverse.Webresources.Init.Events;
using Empowered.Dataverse.Webresources.Init.Extensions;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Reactive.Extensions;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Webresources.Init.Services;

internal class InitService(
    ILogger<InitService> logger,
    IFileSystem fileSystem,
    ICliWrapper cliWrapper,
    IEventObservable? observable = null
) : IInitService
{
    public async Task<IDirectoryInfo> Init(InitOptions options)
    {
        logger.LogTrace("Initializing webresource project with options {Options}", options);
        observable?.Publish(InitInvokedEvent.From(options));

        var projectDirectory = GetOrCreateDirectory(options.Directory, options);
        logger.LogTrace("Processing project directory {Directory}", projectDirectory.FullName);

        SetupProjectFiles(options, projectDirectory);

        await NpmInstallProject(projectDirectory);
        await UpgradeDependencies(options, projectDirectory);
        await OpenProjectInVsCode(projectDirectory);

        return projectDirectory;
    }

    private void SetupProjectFiles(InitOptions options, IDirectoryInfo projectDirectory)
    {
        // src
        var sourcePath = Path.Combine(projectDirectory.FullName, "src");
        var sourceDirectory = GetOrCreateDirectory(sourcePath, options);

        // command
        var commandPath = Path.Combine(sourceDirectory.FullName, "command");
        var commandsDirectory = GetOrCreateDirectory(commandPath, options);

        var gitKeepPath = Path.Combine(commandsDirectory.FullName, ".gitkeep");
        GetOrCreateFile(gitKeepPath, options);

        // form
        var formPath = Path.Combine(sourceDirectory.FullName, "form");
        var formDirectory = GetOrCreateDirectory(formPath, options);

        const string formFile = "form.handler.ts";
        var formHandlerPath = Path.Combine(formDirectory.FullName, formFile);
        GetOrCreateFile(formHandlerPath, options, formFile);

        // view
        var viewPath = Path.Combine(sourceDirectory.FullName, "view");
        var viewDirectory = GetOrCreateDirectory(viewPath, options);

        const string viewFile = "view.handler.ts";
        var viewHandlerPath = Path.Combine(viewDirectory.FullName, viewFile);
        GetOrCreateFile(viewHandlerPath, options, viewFile);

        // package.json
        const string packageJsonFile = "package.json";
        var packagePath = Path.Combine(projectDirectory.FullName, packageJsonFile);
        GetOrCreateFile(packagePath, options, packageJsonFile,
            content =>
            {
                content = content.Replace("<project>", options.Project);
                if (!string.IsNullOrWhiteSpace(options.Author))
                {
                    content = content.Replace("<author>", options.Author);
                    logger.LogTrace("Replaced <author> placeholder with author {Author} in package.json",
                        options.Author);
                }

                if (!string.IsNullOrWhiteSpace(options.Repository))
                {
                    content = content.Replace("<repository>", options.Repository);
                    logger.LogTrace("Replaced <repository> placeholder with repository {Repository} in package.json",
                        options.Repository);
                }

                return content;
            }
        );

        // tsconfig.json
        const string tsconfigJsonFile = "tsconfig.json";
        var tsconfigPath = Path.Combine(projectDirectory.FullName, tsconfigJsonFile);
        GetOrCreateFile(tsconfigPath, options, tsconfigJsonFile);

        // webpack.config.js
        const string webpackConfigFile = "webpack.config.js";
        var webpackConfigPath = Path.Combine(projectDirectory.FullName, webpackConfigFile);
        GetOrCreateFile(webpackConfigPath, options, webpackConfigFile,
            content => content.Replace("<globalNamespace>", options.GlobalNamespace));

        // .gitignore
        const string gitignoreFile = ".gitignore";
        var gitIgnorePath = Path.Combine(projectDirectory.FullName, gitignoreFile);
        GetOrCreateFile(gitIgnorePath, options, gitignoreFile);

        // .vscode/settings.json
        const string vscodeDir = ".vscode";
        var vscodePath = Path.Combine(projectDirectory.FullName, vscodeDir);
        var vscodeDirectory = GetOrCreateDirectory(vscodePath, options);

        const string settingsJsonFile = "settings.json";
        var settingsPath = Path.Combine(vscodeDirectory.FullName, settingsJsonFile);
        GetOrCreateFile(settingsPath, options, settingsJsonFile);
    }

    private async Task OpenProjectInVsCode(IDirectoryInfo projectDirectory)
    {
        try
        {
            var result = await cliWrapper.VsCodeOpen(projectDirectory.FullName);
            logger.LogTrace("Opened project {Directory} in visual studio code with result {Result}",
                projectDirectory.FullName, result);
        }
        catch (CommandExecutionException exception)
        {
            logger.LogError(exception,
                "Opening project directory {Directory} in visual studio code failed with error: {Error}",
                projectDirectory.FullName, exception.Message);
            throw new InvalidOperationException(
                $"Opening project directory {projectDirectory.FullName} in visual studio code failed with error: {exception.Message}",
                exception);
        }
    }

    private async Task UpgradeDependencies(InitOptions options, IDirectoryInfo projectDirectory)
    {
        if (!options.UpgradeDependencies)
        {
            logger.LogTrace("Skipping upgrade dependencies because option is set to false");
            return;
        }

        try
        {
            var result = await cliWrapper.NpmUpgradeDependencies(projectDirectory.FullName);
            observable?.Publish(NpmUpgradeSucceededEvent.From(result));
            logger.LogTrace("Upgraded project {Project} with result {result}", projectDirectory.FullName, result);
        }
        catch (CommandExecutionException exception)
        {
            logger.LogError(exception, "Dependency upgrade command failed with message: {Message}", exception.Message);
            throw new InvalidOperationException(
                $"Failed to upgrade project dependencies for project {options.Project} with exit code {exception.ExitCode} and message {exception.Message}",
                exception);
        }
    }

    private async Task NpmInstallProject(IDirectoryInfo projectDirectory)
    {
        try
        {
            var result = await cliWrapper.NpmInstall(projectDirectory.FullName);
            observable?.Publish(NpmInstallSucceededEvent.From(result));
            logger.LogTrace("Executed npm install with result {Result}", result);
        }
        catch (CommandExecutionException exception)
        {
            logger.LogWarning(
                "Failed to run npm install for created project {Project} with exit code {ExitCode} and message {Message}",
                projectDirectory.FullName, exception.ExitCode, exception.Message);
            throw new InvalidOperationException(
                $"Couldn't successfully run the npm install command with error message {exception.Message}. Please manually validate the generated project.",
                exception);
        }
    }

    private IDirectoryInfo GetOrCreateDirectory(string directoryPath, InitOptions options)
    {
        var directory = fileSystem.DirectoryInfo.New(directoryPath);

        if (directory.Exists && !options.Force)
        {
            logger.LogTrace("Directory {Directory} already exists --> skip", directory.FullName);
            return directory;
        }

        directory.Create();
        observable?.Publish(DirectoryCreatedEvent.From(directory));
        logger.LogTrace("Created directory {Directory} in parent directory {ParentDirectory}", directoryPath,
            directoryPath);

        return directory;
    }

    private void GetOrCreateFile(string filePath, InitOptions options, string? contentResource = null,
        Func<string, string>? replacerFunction = null)
    {
        var file = fileSystem.FileInfo.New(filePath);

        if (file.Exists && !options.Force)
        {
            logger.LogTrace("File {File} already exists and no force --> skip", file.FullName);
            return;
        }

        var content = !string.IsNullOrWhiteSpace(contentResource)
            ? Assembly.GetExecutingAssembly().GetEmbeddedResource(contentResource)
            : string.Empty;
        if (replacerFunction != null)
        {
            content = replacerFunction(content);
        }

        fileSystem.File.WriteAllText(file.FullName, content);
        observable?.Publish(FileCreatedEvent.From(file));
        logger.LogTrace("Created file {File} with content {Content} in directory {Directory}", file.FullName,
            content, file.Directory?.FullName);
    }
}
