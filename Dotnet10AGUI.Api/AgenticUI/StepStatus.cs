// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Dotnet10AGUI.Api.AgenticUI;

[JsonConverter(typeof(JsonStringEnumConverter<StepStatus>))]
internal enum StepStatus
{
    Pending,
    Completed
}
