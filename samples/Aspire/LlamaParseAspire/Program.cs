using LlamaIndex.Core.Schema;
using LlamaParse;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddLlamaParseClient(apiKey: builder.Configuration.GetSection("LlamaParse")["ApiKey"]!);

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

    // Read the file into a byte array
    using var ms = new MemoryStream();
    file.CopyTo(ms);

    var inMemoryFile = new InMemoryFile(ms.ToArray(), fileName);

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