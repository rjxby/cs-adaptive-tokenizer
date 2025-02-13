using CsAdaptiveTokenizer.Services;
using CsAdaptiveTokenizer.Tokenizers;
using CsAdaptiveTokenizer.TokenProcessors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddSingleton<ITokenProcessor, RegexTokenProcessor>();
builder.Services.AddSingleton<ITokenProcessor, BpeTokenProcessor>();
builder.Services.AddSingleton<ITokenizer, CSharpTokenizer>();

var app = builder.Build();

app.MapGrpcService<TokenizerService>();

app.Run();
