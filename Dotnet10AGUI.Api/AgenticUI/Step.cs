// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Dotnet10AGUI.Api.AgenticUI;

internal sealed class Step
{
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("status")]
    public StepStatus Status { get; set; } = StepStatus.Pending;
}
