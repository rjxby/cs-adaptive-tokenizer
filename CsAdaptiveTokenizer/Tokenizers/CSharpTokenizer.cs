using System.Text;
using CsAdaptiveTokenizer.TokenProcessors;

namespace CsAdaptiveTokenizer.Tokenizers;

public class CSharpTokenizer : ITokenizer
{
    private readonly IEnumerable<ITokenProcessor> _processors;
    private readonly ILogger<CSharpTokenizer> _logger;
    private readonly Dictionary<string, long> _stringToTokens = new();
    private readonly Dictionary<long, string> _tokensToString = new();
    private long _lastToken = 0;

    public CSharpTokenizer(
        IEnumerable<ITokenProcessor> processors,
        ILogger<CSharpTokenizer> logger)
    {
        // Order is important: RegexProcessor first, BpeProcessor last
        _processors = processors.OrderBy(p => p is BpeTokenProcessor);
        _logger = logger;
    }

    public async Task<bool> LoadAsync(string path)
    {
        try
        {
            var content = await LoadContentAsync(path);
            if (!string.IsNullOrEmpty(content))
            {
                TokenizeAndUpdateVocabulary(content);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tokenizer");
            return false;
        }
    }

    public List<long> Encode(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new List<long>();
        }

        var encodedData = new List<long>();
        var currentPosition = 0;

        while (currentPosition < input.Length)
        {
            var longestMatch = FindLongestTokenMatch(input, currentPosition);
            if (longestMatch.token != 0)
            {
                encodedData.Add(longestMatch.token);
                currentPosition += longestMatch.length;
            }
            else
            {
                // Try to process unknown token with cascading processors
                var newToken = ProcessWithCascading(input, currentPosition);
                if (!string.IsNullOrEmpty(newToken))
                {
                    AddToken(newToken);
                    encodedData.Add(_stringToTokens[newToken]);
                    currentPosition += newToken.Length;
                }
                else
                {
                    currentPosition++;
                }
            }
        }

        return encodedData;
    }

    public string Decode(IEnumerable<long> tokens)
    {
        return string.Concat(tokens
            .Select(token => _tokensToString.TryGetValue(token, out var value) ? value : string.Empty));
    }

    private async Task<string> LoadContentAsync(string path)
    {
        var contentBuilder = new StringBuilder();
        var isFileLoad = path.Contains(".");

        if (isFileLoad && File.Exists(path))
        {
            contentBuilder.Append(await File.ReadAllTextAsync(path));
        }
        else if (Directory.Exists(path))
        {
            foreach (var filePath in Directory.EnumerateFiles(path, "*.cs"))
            {
                contentBuilder.AppendLine(await File.ReadAllTextAsync(filePath));
            }
        }

        return contentBuilder.ToString();
    }

    private void TokenizeAndUpdateVocabulary(string input)
    {
        var position = 0;

        while (position < input.Length)
        {
            var token = ProcessWithCascading(input, position);
            if (!string.IsNullOrEmpty(token))
            {
                AddToken(token);
                position += token.Length;
            }
            else
            {
                position++;
            }
        }
    }

    private string ProcessWithCascading(string input, int position)
    {
        foreach (var processor in _processors)
        {
            var tokens = processor.Process(input, position);
            if (tokens.Any())
            {
                // Take the longest token found by this processor
                return tokens.OrderByDescending(t => t.Length).First();
            }
        }
        return string.Empty;
    }

    private void AddToken(string value)
    {
        if (!_stringToTokens.ContainsKey(value))
        {
            _lastToken++;
            _stringToTokens[value] = _lastToken;
            _tokensToString[_lastToken] = value;
        }
    }

    private (long token, int length) FindLongestTokenMatch(string input, int position)
    {
        var longestMatch = (token: 0L, length: 0);

        foreach (var token in _stringToTokens)
        {
            if (position + token.Key.Length <= input.Length &&
                input.Substring(position, token.Key.Length) == token.Key &&
                token.Key.Length > longestMatch.length)
            {
                longestMatch = (token.Value, token.Key.Length);
            }
        }

        return longestMatch;
    }
}
