using System.Text.RegularExpressions;

namespace CsAdaptiveTokenizer.TokenProcessors;

public class RegexTokenProcessor : ITokenProcessor
{
    private static readonly Regex[] Patterns = {
        // Namespaces and usings
        new(@"namespace\s+[\w.]+", RegexOptions.Compiled),
        new(@"using\s+[\w.]+", RegexOptions.Compiled),

        // Access modifiers with class/interface/struct
        new(@"(public|private|protected|internal)\s+(abstract\s+)?(class|interface|struct)\s+\w+(<[\w,\s]+>)?", RegexOptions.Compiled),

        // Methods with various modifiers
        new(@"(public|private|protected|internal)\s+(static\s+)?(async\s+)?\w+(<[\w,\s]+>)?\s+\w+\s*\([^)]*\)", RegexOptions.Compiled),

        // Properties
        new(@"(public|private|protected|internal)\s+\w+(<[\w,\s]+>)?\s+\w+\s*{\s*(get;)?\s*(set;)?\s*}", RegexOptions.Compiled),

        // String literals
        new(@"""([^""\\]|\\.)*""|@""[^""]*""", RegexOptions.Compiled),

        // Comments
        new(@"//.*?$|/\*.*?\*/", RegexOptions.Compiled | RegexOptions.Multiline),

        // Keywords
        new(@"\b(if|else|while|for|foreach|return|break|continue|switch|case)\b", RegexOptions.Compiled),

        // Operators
        new(@"[+\-*/%=<>!&|^~?:;.,{}\[\]()]", RegexOptions.Compiled),

        // Numbers
        new(@"\b\d+(\.\d+)?([eE][+-]?\d+)?\b", RegexOptions.Compiled),

        // Identifiers
        new(@"\b[a-zA-Z_]\w*\b", RegexOptions.Compiled)
    };

    public IEnumerable<string> Process(string input, int position)
    {
        var matches = Patterns
            .Select(p => p.Match(input, position))
            .Where(m => m.Success && m.Index == position)
            .OrderByDescending(m => m.Length)
            .ToList();

        return matches.Select(m => m.Value).ToList();
    }
}
