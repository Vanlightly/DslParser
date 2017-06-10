using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DslParser.Parsing.Tokens;

namespace DslParser.DataRepresentation
{
    public class MatchCondition
    {
        public DslObject Object { get; set; }
        public DslOperator Operator { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }
        
        public DslLogicalOperator LogOpToNextCondition { get; set; }
    }
}
