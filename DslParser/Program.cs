using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DslParser.Exceptions;
using DslParser.Parsing;
using DslParser.Parsing.Tokenizers;
using DslParser.Parsing.Tokenizers.MoreEfficient;
using DslParser.Parsing.Tokenizers.SlowAndSimple;
using Newtonsoft.Json;
using DslParser.SqlGeneration;
using DslParser.DataRepresentation;

namespace DslParser
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("Press 1 : view inefficient version output");
                Console.WriteLine("Press 2 : Perf test of inefficient tokenizer with a small query");
                Console.WriteLine("Press 3 : Perf test of inefficient tokenizer with a large query");
                Console.WriteLine("Press 4 : view more efficient version output");
                Console.WriteLine("Press 5 : Perf test of more efficient tokenizer with a small query");
                Console.WriteLine("Press 6 : Perf test of more efficient tokenizer with a large query");

                var key = Console.ReadKey();
                Console.WriteLine("");

                switch (key.KeyChar.ToString())
                {
                    case "1":
                        ITokenizer slowTokenizer = new SimpleRegexTokenizer();
                        RunOnceAndPrintOutput(slowTokenizer, "Run with inefficient tokenizer");
                        break;
                    case "2":
                        PerfTestWithSlowTokenizerAndSmallQuery();
                        break;
                    case "3":
                        PerfTestWithSlowTokenizerAndLargeQuery();
                        break;
                    case "4":
                        ITokenizer fastTokenizer = new PrecedenceBasedRegexTokenizer();
                        RunOnceAndPrintOutput(fastTokenizer, "Run with faster tokenizer");
                        break;
                    case "5":
                        PerfTestWithFastTokenizerAndSmallQuery();
                        break;
                    case "6":
                        PerfTestWithFastTokenizerAndLargeQuery();
                        break;
                    default:
                        Console.WriteLine("Press 1, 2, 3, 4, 5 or 6");
                        break;
                }

                Console.WriteLine("");
            }
        }

        public void RunOnceAndPrintOutput(ITokenizer tokenizer, string startMessage)
        {
            Console.WriteLine(startMessage);
            Console.WriteLine("");

            var parser = new Parser();
            var sqlGenerator = new SqlGenerator();

            string query = @"MATCH app = 'MyTestApp'
AND ex IN ('System.NullReferenceException', 'System.FormatException')
BETWEEN 2016-01-01 00:00:00 AND 2016-02-01 00:00:00
LIMIT 100";

            Console.WriteLine("");
            Console.WriteLine("The DSL query:");
            Console.WriteLine(query);
            Console.WriteLine("");
            Console.WriteLine("Tokens generated:");

            var tokenSequence = tokenizer.Tokenize(query).ToList();
            foreach(var token in tokenSequence)
                Console.WriteLine(string.Format("TokenType: {0}, Value: {1}", token.TokenType, token.Value));

            var dataRepresentation = parser.Parse(tokenSequence);
            Console.WriteLine("");
            Console.WriteLine("Data Representation (serialized to JSON)");
            Console.WriteLine(JsonConvert.SerializeObject(dataRepresentation, Formatting.Indented));

            Console.WriteLine("");
            Console.WriteLine("SQL Generated:");
            var sql = sqlGenerator.GenerateQueryPayload(dataRepresentation);
            Console.WriteLine(sql.GetSqlText());

            Console.WriteLine("");
            Console.WriteLine("Process complete");
        }

        public void PerfTestWithSlowTokenizerAndSmallQuery()
        {
            ITokenizer tokenizer = new SimpleRegexTokenizer();
            string query = @"MATCH app = 'MyTestApp'
AND ex IN ('System.NullReferenceException', 'System.FormatException')
BETWEEN 2016-01-01 00:00:00 AND 2016-02-01 00:00:00
LIMIT 100";

            PerfTest(tokenizer, query, "Slow tokenizer + small query");
        }

        public void PerfTestWithSlowTokenizerAndLargeQuery()
        {
            ITokenizer tokenizer = new SimpleRegexTokenizer();
            string query = @"MATCH app = 'MyTestApp'
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND sf = 'sadsdfsdfsdfsdfssdfjhsfjhsdfjhsdfjhsdfjhsdjfhsdjhfsdjfhsdhfsdjhfsdjhfjsdhfjsdhfjhsdjfhsdjfh'
AND sf = 'fggdfgdfgfdgdfgdfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggh'
AND sf = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
AND sf = 'bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
AND sf = 'ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
AND sf = 'ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd'
AND sf = '1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee'
AND sf = '2sadsdfsdfsdfsdfssdfjhsfjhsdfjhsdfjhsdfjhsdjfhsdjhfsdjfhsdhfsdjhfsdjhfjsdhfjsdhfjhsdjfhsdjfh'
AND sf = '3fggdfgdfgfdgdfgdfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggh'
AND sf = '4aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
AND sf = '5bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
AND sf = '6ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
AND sf = '7ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd'
AND sf = '8eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee'
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
BETWEEN 2016-01-01 00:00:00 AND 2016-02-01 00:00:00
LIMIT 100";

            PerfTest(tokenizer, query, "Slow tokenizer + large query");
        }

        public void PerfTestWithFastTokenizerAndSmallQuery()
        {
            ITokenizer tokenizer = new PrecedenceBasedRegexTokenizer();
            string query = @"MATCH app = 'MyTestApp'
AND ex IN ('System.NullReferenceException', 'System.FormatException')
BETWEEN 2016-01-01 00:00:00 AND 2016-02-01 00:00:00
LIMIT 100";

            PerfTest(tokenizer, query, "Fast tokenizer + small query");
        }

        public void PerfTestWithFastTokenizerAndLargeQuery()
        {
            ITokenizer tokenizer = new PrecedenceBasedRegexTokenizer();
            string query = @"MATCH app = 'MyTestApp'
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND sf = 'sadsdfsdfsdfsdfssdfjhsfjhsdfjhsdfjhsdfjhsdjfhsdjhfsdjfhsdhfsdjhfsdjhfjsdhfjsdhfjhsdjfhsdjfh'
AND sf = 'fggdfgdfgfdgdfgdfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggh'
AND sf = 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
AND sf = 'bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
AND sf = 'ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
AND sf = 'ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd'
AND sf = '1eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee'
AND sf = '2sadsdfsdfsdfsdfssdfjhsfjhsdfjhsdfjhsdfjhsdjfhsdjhfsdjfhsdhfsdjhfsdjhfjsdhfjsdhfjhsdjfhsdjfh'
AND sf = '3fggdfgdfgfdgdfgdfgggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggh'
AND sf = '4aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa'
AND sf = '5bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb'
AND sf = '6ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc'
AND sf = '7ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd'
AND sf = '8eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee'
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
AND ex IN ('System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException','System.NullReferenceException', 'System.FormatException')
BETWEEN 2016-01-01 00:00:00 AND 2016-02-01 00:00:00
LIMIT 100";

            PerfTest(tokenizer, query, "Fast tokenizer + large query");
        }

        public void PerfTest(ITokenizer tokenizer, string query, string startMessage)
        {
            Console.WriteLine(startMessage);
            Console.WriteLine("Will run the process 1000 times. Query char count: " + query.Length);

            var sw = new Stopwatch();
            sw.Start();

            for(int i=0; i<1000; i++)
                RunOnceWithoutOutput(tokenizer, query);

            sw.Stop();
            Console.WriteLine("Elapsed milliseconds: " + sw.ElapsedMilliseconds);
            Console.WriteLine("");
        }

        public void RunOnceWithoutOutput(ITokenizer tokenizer, string queryText)
        {
            var parser = new Parser();
            var sqlGenerator = new SqlGenerator();

            var tokenSequence = tokenizer.Tokenize(queryText).ToList();
            var dataRepresentation = parser.Parse(tokenSequence);
            var sql = sqlGenerator.GenerateQueryPayload(dataRepresentation);
        }

    }
}
