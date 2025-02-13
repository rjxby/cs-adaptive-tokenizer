namespace CsAdaptiveTokenizer.TokenProcessors;

public class BpeTokenProcessor : ITokenProcessor
{
    private readonly Dictionary<string, int> _pairFrequencies = new();
    private readonly int _maxMerges = 10;

    public IEnumerable<string> Process(string input, int position)
    {
        var text = input[position].ToString();
        var tokens = text.Select(c => c.ToString()).ToList();

        for (int i = 0; i < _maxMerges && tokens.Count > 1; i++)
        {
            UpdatePairFrequencies(tokens);
            if (!_pairFrequencies.Any()) break;

            var mostFrequentPair = _pairFrequencies
                .OrderByDescending(x => x.Value)
                .First().Key;

            MergeTokens(tokens, mostFrequentPair);
        }

        return tokens;
    }

    private void UpdatePairFrequencies(List<string> tokens)
    {
        _pairFrequencies.Clear();
        for (int j = 0; j < tokens.Count - 1; j++)
        {
            var pair = tokens[j] + tokens[j + 1];
            _pairFrequencies[pair] = _pairFrequencies.GetValueOrDefault(pair) + 1;
        }
    }

    private void MergeTokens(List<string> tokens, string pair)
    {
        for (int j = 0; j < tokens.Count - 1; j++)
        {
            if (j >= tokens.Count - 1) break;

            if (tokens[j] + tokens[j + 1] == pair)
            {
                tokens[j] = pair;
                tokens.RemoveAt(j + 1);
            }
        }
    }
}
