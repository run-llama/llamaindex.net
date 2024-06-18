using LlamaIndex.Core.Schema;
using LlamaParse;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<LlamaParseClient>(sp =>
{
    var client = sp.GetRequiredService<HttpClient>();
    var llamaParseClient = new LlamaParseClient(client, apiKey: builder.Configuration.GetSection("LlamaParse")["ApiKey"]!);
    return llamaParseClient;
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var fileUploadHandler = async (LlamaParseClient client, IFormFile file) =>
{
    var fileName = file.FileName;

    var inMemoryFile = new InMemoryFile(File.ReadAllBytes(fileName), fileName);

    var sb = new StringBuilder();
    await foreach (var doc in client.LoadDataAsync(inMemoryFile))
    {
        if(doc is ImageDocument)
        {
            continue;
        }
        else
        {
            sb.AppendLine(doc.Text);
        }
    }
    return Results.Ok(sb.ToString());
};

app.MapPost("/parse", fileUploadHandler)
    .WithName("ParseFile")
    .DisableAntiforgery();

app.Run();