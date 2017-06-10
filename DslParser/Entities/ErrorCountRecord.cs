using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.Entities
{
    public class ErrorCountRecord
    {
        public string Fingerprint { get; set; }
        public string ApplicationId { get; set; }
        public string OriginExceptionType { get; set; }
        public string OriginStackFrame { get; set; }
        public string LowestAppStackFrame { get; set; }
        public string HighestAppStackFrame { get; set; }
        public int Count { get; set; }
    }
}
