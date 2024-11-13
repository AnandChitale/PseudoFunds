using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Open AI Service

var azureOpenAIConfig = builder.Configuration.GetSection("AzureOpenAI");
builder.Services.AddSingleton(new AzureOpenAIClient(
    new Uri(azureOpenAIConfig["EndPoint"]),
    new System.ClientModel.ApiKeyCredential(azureOpenAIConfig["ApiKey"])));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
