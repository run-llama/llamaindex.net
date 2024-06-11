using System.Diagnostics;

namespace LlamaParse;

internal static class LlamaDiagnostics
{
    private static readonly string Namespace = typeof(LlamaDiagnostics).Namespace!;
    private static readonly ActivitySource ActivitySource = new(Namespace);

    private const string EnableDiagnosticsSwitch = "LlamaParse.GenAI.EnableOTelDiagnostics";
    private const string EnableSensitiveEventsSwitch = "LlamaParse.GenAI.EnableOTelDiagnosticsSensitive";
    private const string EnableDiagnosticsEnvVar = "LLAMAPARSE_GENAI_ENABLE_OTEL_DIAGNOSTICS";
    private const string EnableSensitiveEventsEnvVar = "LLAMAPARSE_GENAI_ENABLE_OTEL_DIAGNOSTICS_SENSITIVE";

    private static readonly bool EnableDiagnostics = AppContextSwitchHelper.GetConfigValue(EnableDiagnosticsSwitch, EnableDiagnosticsEnvVar);
    private static readonly bool EnableSensitiveEvents = AppContextSwitchHelper.GetConfigValue(EnableSensitiveEventsSwitch, EnableSensitiveEventsEnvVar);

    /// <summary>
    /// Check if diagnostics is enabled
    /// Diagnostics is enabled if either EnableDiagnostics or EnableSensitiveEvents is set to true and there are listeners.
    /// </summary>
    public static bool IsDiagnosticsEnabled()
    {
        return (EnableDiagnostics || EnableSensitiveEvents) && ActivitySource.HasListeners();
    }

    /// <summary>
    /// Check if sensitive events are enabled.
    /// Sensitive events are enabled if EnableSensitiveEvents is set to true and there are listeners.
    /// </summary>
    public static bool IsSensitiveEventsEnabled() => EnableSensitiveEvents && ActivitySource.HasListeners();
}