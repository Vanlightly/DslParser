using DslParser.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.SqlGeneration
{
    public class SqlExecutor
    {
        public IList<ErrorCountRecord> GetTopRankingErrors(AdoQueryPayload adoQueryPayload)
        {
            var results = new List<ErrorCountRecord>();

            using (var connection = new SqlConnection("Database=ErrorsDb;Server=(local);Trusted_Connection=true;"))
            {
                using (var command = new SqlCommand(adoQueryPayload.GetSqlText(), connection))
                {
                    foreach (var parameter in adoQueryPayload.Parameters)
                        command.Parameters.Add(parameter);

                    using (var reader = command.ExecuteReader())
                    {
                        var record = new ErrorCountRecord();
                        record.ApplicationId = reader["ApplicationId"].ToString();
                        record.Count = (int)reader["Count"];
                        record.Fingerprint = reader["FingerprintText"].ToString();
                        record.HighestAppStackFrame = reader["HighestAppStackFrame"].ToString();
                        record.LowestAppStackFrame = reader["LowestAppStackFrame"].ToString();
                        record.OriginExceptionType = reader["OriginExceptionType"].ToString();
                        record.OriginStackFrame = reader["OriginStackFrame"].ToString();

                        results.Add(record);
                    }
                }
            }

            return results;
        }
    }
}
