using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DslParser.Exceptions;
using DslParser.Parsing.Tokens;
using DslParser.Parsing.Tokens.Tokenizers.SlowAndSimple;

namespace DslParser.Parsing.Tokenizers.SlowAndSimple
{
    public class SimpleRegexTokenizer : ITokenizer
    {
        private List<TokenDefinition> _tokenDefinitions;

        public SimpleRegexTokenizer()
        {
            _tokenDefinitions = new List<TokenDefinition>();

            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "^and"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Application, "^app|^application"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Between, "^between"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CloseParenthesis, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.ExceptionType, "^ex|^exception"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Fingerprint, "^fingerprint"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotIn, "^not in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.In, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Like, "^like"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Limit, "^limit"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Match, "^match"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Message, "^msg|^message"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "^!="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotLike, "^not like"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.OpenParenthesis, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "^or"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StackFrame, "^sf|^stackframe"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DateTimeValue, "^\\d\\d\\d\\d-\\d\\d-\\d\\d \\d\\d:\\d\\d:\\d\\d"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "^'[^']*'"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "^\\d+"));
        }

            
        public IEnumerable<DslToken> Tokenize(string lqlText)
        {
            var tokens = new List<DslToken>();

            string remainingText = lqlText;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    tokens.Add(new DslToken(match.TokenType, match.Value));
                    remainingText = match.RemainingText;
                }
                else
                {
                    if (IsWhitespace(remainingText))
                    {
                        remainingText = remainingText.Substring(1);
                    }
                    else
                    {
                        var invalidTokenMatch = CreateInvalidTokenMatch(remainingText);
                        tokens.Add(new DslToken(invalidTokenMatch.TokenType, invalidTokenMatch.Value));
                        remainingText = invalidTokenMatch.RemainingText;
                    }
                }
            }

            tokens.Add(new DslToken(TokenType.SequenceTerminator, string.Empty));

            return tokens;
        }

        private TokenMatch FindMatch(string lqlText)
        {
            foreach (var tokenDefinition in _tokenDefinitions)
            {
                var match = tokenDefinition.Match(lqlText);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() {  IsMatch = false };
        }

        private bool IsWhitespace(string lqlText)
        {
            return Regex.IsMatch(lqlText, "^\\s+");
        }

        private TokenMatch CreateInvalidTokenMatch(string lqlText)
        {
            var match = Regex.Match(lqlText, "(^\\S+\\s)|^\\S+");
            if (match.Success)
            {
                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = lqlText.Substring(match.Length),
                    TokenType = TokenType.Invalid,
                    Value = match.Value.Trim()
                };
            }

            throw new DslParserException("Failed to generate invalid token");
        }
    }
}
