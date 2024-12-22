using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using CommandDotNet;
using Empowered.Dataverse.Webresources.Commands.Arguments;
using Empowered.Dataverse.Webresources.Commands.Services;
using Empowered.Dataverse.Webresources.Push.Model;
using Empowered.Dataverse.Webresources.Push.Services;
using FluentAssertions;
using NSubstitute;
using Spectre.Console;
using Xunit;

namespace Empowered.Dataverse.Webresources.Commands.Tests;

public class WebresourceCommandTests
{
    private WebresourceCommand _webresourceCommand;
    private readonly IPushService _pushService;
    private readonly IPushOptionWriter _optionWriter;
    private readonly IPushOptionResolver _optionResolver;

    public WebresourceCommandTests()
    {
        _optionResolver = Substitute.For<IPushOptionResolver>();
        _optionWriter = Substitute.For<IPushOptionWriter>();
        _pushService = Substitute.For<IPushService>();
        _webresourceCommand =
            new WebresourceCommand(AnsiConsole.Console, _pushService, _optionResolver, _optionWriter);
    }

    [Fact]
    public async Task ShouldWriteOptionFileIfPersistConfigurationIsSet()
    {
        var pushArguments = new PushArguments
        {
            PersistConfiguration = new FileInfo(Path.GetTempPath())
        };
        _optionResolver.Resolve(pushArguments).Returns(new PushOptions
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
}
