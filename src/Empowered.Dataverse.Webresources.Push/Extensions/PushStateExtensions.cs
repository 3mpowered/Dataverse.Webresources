﻿using Empowered.Dataverse.Webresources.Push.Model;

namespace Empowered.Dataverse.Webresources.Push.Extensions;

public static class PushStateExtensions
{
    public static string Format(this PushState pushState) => pushState switch
    {
        PushState.Created => "Created",
        PushState.Updated => "Updated",
        PushState.Uptodate => "Skipped",
        _ => throw new ArgumentOutOfRangeException(nameof(pushState), pushState, null)
    };
}
