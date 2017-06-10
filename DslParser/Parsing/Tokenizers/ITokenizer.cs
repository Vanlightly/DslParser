using DslParser.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.Parsing.Tokenizers
{
    public interface ITokenizer
    {
        IEnumerable<DslToken> Tokenize(string queryDsl);
    }
}
