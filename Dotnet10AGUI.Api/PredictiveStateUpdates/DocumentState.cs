// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Dotnet10AGUI.Api.PredictiveStateUpdates;

internal sealed class DocumentState
{
    [JsonPropertyName("document")]
    public string Document { get; set; } = string.Empty;
}
