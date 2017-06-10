using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DslParser.Parsing.Tokenizers.SlowAndSimple;

namespace DslParser.Parsing.Tokens.Tokenizers.SlowAndSimple
{
    public class TokenDefinition
    {
        private Regex _regex;
        private readonly TokenType _returnsToken;

        public TokenDefinition(TokenType returnsToken, string regexPattern)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            _returnsToken = returnsToken;
        }

        public TokenMatch Match(string inputString)
        {
            var match = _regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                    remainingText = inputString.Substring(match.Length);

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = _returnsToken,
                    Value = match.Value
                };
            }
            else
            {
                return new TokenMatch() { IsMatch = false};
            }

        }
    }
}
