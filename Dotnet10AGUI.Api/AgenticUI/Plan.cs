// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Dotnet10AGUI.Api.AgenticUI;

internal sealed class Plan
{
    [JsonPropertyName("steps")]
    public List<Step> Steps { get; set; } = [];
}
