namespace CsAdaptiveTokenizer.TokenProcessors;

public interface ITokenProcessor
{
    IEnumerable<string> Process(string text, int position);
}
