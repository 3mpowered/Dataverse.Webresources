using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Generate.Model;
using Empowered.Dataverse.Webresources.Generate.Services;
using Empowered.Dataverse.Webresources.Init.Model;
using Empowered.Dataverse.Webresources.Init.Services;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using FluentAssertions;
using NSubstitute;
using Spectre.Console;

namespace Empowered.Dataverse.Webresources.Commands.Tests;

public class WebresourceCommandTests
{
    private readonly WebresourceCommand _webresourceCommand;
    private readonly IPushService _pushService;
    private readonly IOptionWriter _optionWriter;
    private readonly IOptionResolver _optionResolver;
    private readonly IInitService _initService;
    private readonly IGenerateService _generateService;

    public WebresourceCommandTests()
    {
        _optionResolver = Substitute.For<IOptionResolver>();
        _optionWriter = Substitute.For<IOptionWriter>();
        _pushService = Substitute.For<IPushService>();
        _initService = Substitute.For<IInitService>();
        _generateService = Substitute.For<IGenerateService>();
        _webresourceCommand =
            new WebresourceCommand(AnsiConsole.Console, _pushService, _initService, _generateService, _optionResolver,
                _optionWriter);
    }

    [Fact]
    public async Task ShouldWritePushOptionFileIfPersistConfigurationIsSet()
    {
        var pushArguments = new PushArguments
        {
            PersistConfiguration = new FileInfo(Path.GetTempPath())
        };
        _optionResolver.Resolve<PushOptions, PushArguments>(pushArguments).Returns(new PushOptions
        {
            Directory = Path.GetTempPath(),
            Solution = "something"
        });
        _pushService.PushWebresources(Arg.Any<PushOptions>())
            .Returns([]);
        _optionWriter.Write(Arg.Any<PushOptions>(), pushArguments.PersistConfiguration)
            .Returns(new FileInfoWrapper(new MockFileSystem(), pushArguments.PersistConfiguration));

        var result = await _webresourceCommand.Push(pushArguments);

        result.Should().Be(await ExitCodes.Success);
        _optionWriter.Received(1).Write(Arg.Any<PushOptions>(), pushArguments.PersistConfiguration);
    }

    [Fact]
    public async Task ShouldWriteGenerateOptionFileIfPersistConfigurationIsSet()
    {
        var arguments = new GenerateArguments
        {
            PersistConfiguration = new FileInfo(Path.GetTempPath())
        };
        var options = new GenerateOptions
        {
            Directory = Path.GetTempPath(),
        };

        _optionResolver.Resolve<GenerateOptions, GenerateArguments>(arguments).Returns(options);
        _generateService.Generate(Arg.Any<GenerateOptions>())
            .Returns(new DirectoryInfoWrapper(new MockFileSystem(), options.DirectoryInfo));
        _optionWriter.Write(Arg.Any<GenerateOptions>(), arguments.PersistConfiguration)
            .Returns(new FileInfoWrapper(new MockFileSystem(), arguments.PersistConfiguration));

        var result = await _webresourceCommand.Generate(arguments);

        result.Should().Be(await ExitCodes.Success);
        _optionWriter.Received(1).Write(Arg.Any<GenerateOptions>(), arguments.PersistConfiguration);
    }

    [Fact]
    public async Task ShouldSuccessfullyExecuteInitCommand()
    {
        var directory = new DirectoryInfo(Path.GetTempPath());
        var projectDirectory = new DirectoryInfoWrapper(new MockFileSystem(), directory);
        _initService.Init(Arg.Any<InitOptions>())
            .Returns(projectDirectory);

        var arguments = new InitArguments
        {
            Directory = directory,
            Project = directory.Name,
            GlobalNamespace = "Any",
            Force = true
        };
        _optionResolver.Resolve<InitOptions, InitArguments>(arguments);

        var result = await _webresourceCommand.Init(arguments);

        result.Should().Be(await ExitCodes.Success);
        await _initService.Received(1).Init(Arg.Any<InitOptions>());
    }
}
