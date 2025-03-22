using System.IO.Abstractions;
using System.Text.Json;
using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Model;
using Empowered.Dataverse.Webresources.Push.Model;
using Microsoft.Extensions.Logging;
using ArgumentException = System.ArgumentException;

namespace Empowered.Dataverse.Webresources.Commands.Services;

internal class OptionResolver(IFileSystem fileSystem, ILogger<OptionResolver> logger) : IOptionResolver
{
    public TOptions Resolve<TOptions, TArguments>(TArguments arguments)
        where TOptions : class where TArguments : IArgumentModel
    {
        object options = arguments switch
        {
            PushArguments pushArguments => Resolve(pushArguments),
            InitArguments initArguments => Resolve(initArguments),
            _ => throw new ArgumentException(
                $"Arguments of type {arguments.GetType().Name} are not handled by option resolver",
                nameof(arguments))
        };

        return options as TOptions ?? throw new InvalidOperationException(
            $"Couldn't return options of type {options.GetType().Name} in specified type {typeof(TOptions).Name}");
    }

    private static InitOptions Resolve(InitArguments arguments) => new()
    {
        Directory = arguments.Directory.FullName,
        Project = arguments.Project,
        GlobalNamespace = arguments.GlobalNamespace,
        Force = arguments.Force,
        UpgradeDependencies = arguments.UpgradeDependencies,
        Author = arguments.Author,
        Repository = arguments.Repository?.ToString(),
    };

    private PushOptions Resolve(PushArguments arguments)
    {
        if (arguments.Configuration != null)
        {
            logger.LogDebug(
                "Resolving push options using configuration file {ConfigurationFile} and overwriting with inline arguments {InlineArguments}",
                arguments.Configuration.FullName, arguments);
            var configurationFile = fileSystem.FileInfo.Wrap(arguments.Configuration);

            if (configurationFile is not { Exists: true })
            {
                logger.LogWarning("Configuration file {ConfigurationFile} doesn't exist", configurationFile?.FullName);
                throw new ArgumentException($"Configuration file {configurationFile?.FullName} doesn't exist!",
                    nameof(arguments));
            }

            using var fileSystemStream = configurationFile.OpenRead();
            var configOptions = JsonSerializer.Deserialize<PushOptions>(fileSystemStream);

            if (configOptions == null)
            {
                throw new ArgumentException(
                    $"Couldn't deserialize configuration file {configurationFile.FullName} to configuration object",
                    nameof(arguments));
            }

            if (!fileSystem.Path.IsPathRooted(configOptions.Directory) && configurationFile.Directory != null)
            {
                var absolutePath =
                    fileSystem.Path.GetFullPath(configOptions.Directory, configurationFile.Directory.FullName);
                logger.LogDebug(
                    "Configuration directory {Directory} is a relative path. converting to absolute path {AbsolutePath} using configuration directory {ConfigurationDirectory}",
                    configOptions.Directory, absolutePath, configurationFile.Directory.FullName);

                configOptions = configOptions with
                {
                    Directory = absolutePath
                };
            }

            var mergedOptions = new PushOptions
            {
                Directory = arguments.Directory?.FullName ?? configOptions.Directory,
                Solution = arguments.Solution ?? configOptions.Solution,
                AllowManagedUpdates = arguments.AllowManagedUpdates ?? configOptions.AllowManagedUpdates,
                DefaultWebresourceType = arguments.DefaultType ?? configOptions.DefaultWebresourceType,
                ForceUpdate = arguments.ForceUpdate ?? configOptions.ForceUpdate,
                PublisherPrefix = arguments.Publisher ?? configOptions.PublisherPrefix,
                FileExtensions = arguments.FileExtensions ?? configOptions.FileExtensions,
                Publish = arguments.Publish ?? configOptions.Publish,
                WebresourcePrefix = arguments.Prefix ?? configOptions.WebresourcePrefix,
                IncludeSubDirectories = arguments.Recursive ?? configOptions.IncludeSubDirectories,
                ConfigurationFilePath =
                    arguments.PersistConfiguration?.FullName ?? arguments.Configuration.FullName,
            };

            return mergedOptions;
        }

        logger.LogDebug("Resolving push options via inline arguments {InlineArguments}", arguments);
        if (arguments.Directory == null)
        {
            throw new ArgumentException("Directory cannot be null", nameof(arguments));
        }

        if (string.IsNullOrWhiteSpace(arguments.Solution))
        {
            throw new ArgumentException("Solution cannot be null or empty", nameof(arguments));
        }

        var inlineOptions = new PushOptions
        {
            Directory = arguments.Directory.FullName,
            Solution = arguments.Solution,
            IncludeSubDirectories = arguments.Recursive ?? true,
            FileExtensions = arguments.FileExtensions ?? [],
            ForceUpdate = arguments.ForceUpdate ?? false,
            PublisherPrefix = arguments.Publisher ?? string.Empty,
            WebresourcePrefix = arguments.Prefix ?? string.Empty,
            AllowManagedUpdates = arguments.AllowManagedUpdates ?? false,
            DefaultWebresourceType = arguments.DefaultType ?? webresource_webresourcetype.Script_JScript,
            Publish = arguments.Publish ?? true
        };

        return inlineOptions;
    }
}
