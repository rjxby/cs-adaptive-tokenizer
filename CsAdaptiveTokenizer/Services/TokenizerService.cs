using CsAdaptiveTokenizer.Contracts;
using CsAdaptiveTokenizer.Tokenizers;
using Grpc.Core;

namespace CsAdaptiveTokenizer.Services;

public class TokenizerService : Tokenizer.TokenizerBase
{
    private readonly ITokenizer _tokenizer;
    private readonly ILogger<TokenizerService> _logger;

    public TokenizerService(ITokenizer tokenizer, ILogger<TokenizerService> logger)
    {
        _tokenizer = tokenizer;
        _logger = logger;
    }

    public override async Task<LoadReply> Load(LoadRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Path))
        {
            return new LoadReply { IsSuccess = false };
        }

        var success = await _tokenizer.LoadAsync(request.Path);
        return new LoadReply { IsSuccess = success };
    }

    public override Task<EncodeReply> Encode(EncodeRequest request, ServerCallContext context)
    {
        var reply = new EncodeReply();
        reply.EncodedData.AddRange(_tokenizer.Encode(request.DataToEncode));
        return Task.FromResult(reply);
    }

    public override Task<DecodeReply> Decode(DecodeRequest request, ServerCallContext context)
    {
        return Task.FromResult(new DecodeReply
        {
            DecodedData = _tokenizer.Decode(request.DataToDecode)
        });
    }
}
