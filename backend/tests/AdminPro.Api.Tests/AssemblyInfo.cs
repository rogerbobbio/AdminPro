using Xunit;

// Program.cs assigns Serilog's global static Log.Logger on every boot. Two
// WebApplicationFactory-based test classes booting concurrently would race on
// that static field and clobber each other's LoggerProviderCollection, causing
// intermittent missing log captures. Serialize test classes to avoid it.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
