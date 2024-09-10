var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.LlamaParseAspire>("llamaparseaspire");

builder.Build().Run();
