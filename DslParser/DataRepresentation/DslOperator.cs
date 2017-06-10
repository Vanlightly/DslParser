using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.DataRepresentation
{
    public enum DslOperator
    {
        NotDefined,
        Equals,
        NotEquals,
        Like,
        NotLike,
        In,
        NotIn
    }
}
