global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using AppleSignInApp.Models;
global using AppleSignInApp.Presentation;
global using AppleSignInApp.DataContracts;
global using AppleSignInApp.DataContracts.Serialization;
global using AppleSignInApp.Services.Caching;
global using AppleSignInApp.Services.Endpoints;
#if MAUI_EMBEDDING
global using AppleSignInApp.MauiControls;
#endif
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]
