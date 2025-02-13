namespace CsAdaptiveTokenizer.Tokenizers;

public interface ITokenizer
{
    Task<bool> LoadAsync(string path);
    List<long> Encode(string input);
    string Decode(IEnumerable<long> tokens);
}
