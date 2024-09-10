namespace LlamaParse.Tests;

public sealed class SkipOnKeyNotFoundFact : FactAttribute
{

    private readonly string _reason = "Test skipped: Requires LLAMA_CLOUD_API_KEY to be set and not empty";

#pragma warning disable CS8603 // Possible null reference return.
    public override string Skip => SkipIfKeyNotFound() ? _reason : null;
#pragma warning restore CS8603 // Possible null reference return.

    public bool SkipIfKeyNotFound()
    {
        return string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY"));
    }
}
