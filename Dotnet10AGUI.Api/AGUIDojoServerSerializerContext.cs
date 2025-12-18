// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Dotnet10AGUI.Api.AgenticUI;
using Dotnet10AGUI.Api.BackendToolRendering;
using Dotnet10AGUI.Api.PredictiveStateUpdates;
using Dotnet10AGUI.Api.SharedState;

namespace Dotnet10AGUI.Api;

[JsonSerializable(typeof(WeatherInfo))]
[JsonSerializable(typeof(Recipe))]
[JsonSerializable(typeof(Ingredient))]
[JsonSerializable(typeof(RecipeResponse))]
[JsonSerializable(typeof(Plan))]
[JsonSerializable(typeof(Step))]
[JsonSerializable(typeof(StepStatus))]
[JsonSerializable(typeof(StepStatus?))]
[JsonSerializable(typeof(JsonPatchOperation))]
[JsonSerializable(typeof(List<JsonPatchOperation>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(DocumentState))]
internal sealed partial class AguiDojoServerSerializerContext : JsonSerializerContext;
