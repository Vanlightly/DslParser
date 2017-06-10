using DslParser.DataRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.SqlGeneration
{
    public class SqlGenerator
    {
        public AdoQueryPayload GenerateQueryPayload(DslQueryModel dslQueryModel)
        {
            var adoQueryPayload = new AdoQueryPayload();

            if(dslQueryModel.Limit.HasValue)
                adoQueryPayload.AppendLine("SELECT TOP " + dslQueryModel.Limit.Value);
            else
                adoQueryPayload.AppendLine("SELECT");

            adoQueryPayload.AppendLine(@"        ED.FingerprintText
        ,ED.ApplicationId
        ,ED.OriginExceptionType
        ,ED.OriginStackFrame
        ,ED.LowestAppStackFrame
        ,ED.HighestAppStackFrame
        ,SUM(T.Frequency) AS TotalErrors
FROM Timeline T
JOIN ErrorDefinition AS ED ON T.Fingerprint = ED.Fingerprint");

            adoQueryPayload.Append("WHERE T.ErrorDateTime BETWEEN ");
            adoQueryPayload.AddFromDateParameter(dslQueryModel.DateRange.From);
            adoQueryPayload.Append(" AND ");
            adoQueryPayload.AddToDateParameter(dslQueryModel.DateRange.To);
            adoQueryPayload.AddNewLine();

            for(int i=0; i<dslQueryModel.MatchConditions.Count; i++)
            {
                if (i == 0)
                    AddLogicalOperator(adoQueryPayload, DslLogicalOperator.And); 
                else
                    AddLogicalOperator(adoQueryPayload, dslQueryModel.MatchConditions[i-1].LogOpToNextCondition);

                AddClause(dslQueryModel.MatchConditions[i], adoQueryPayload);
            }

            adoQueryPayload.AppendLine(@"GROUP BY ED.FingerprintText
        ,ED.ApplicationId
        ,ED.OriginExceptionType
        ,ED.OriginStackFrame
        ,ED.LowestAppStackFrame
        ,ED.HighestAppStackFrame
ORDER BY TotalErrors DESC");

            return adoQueryPayload;
        }

        private void AddLogicalOperator(AdoQueryPayload queryPayload, DslLogicalOperator logicalOperator)
        {
            if (logicalOperator == DslLogicalOperator.And)
                queryPayload.Append("AND ");
            else if (logicalOperator == DslLogicalOperator.Or)
                queryPayload.Append("OR ");
        }

        private void AddClause(MatchCondition matchCondition, AdoQueryPayload queryPayload)
        {
            queryPayload.AppendColumnName(matchCondition.Object);

            switch (matchCondition.Operator)
            {
                case DslOperator.Equals:
                    queryPayload.Append(" = ");
                    queryPayload.AddParameter(matchCondition);
                    break;
                case DslOperator.NotEquals:
                    queryPayload.Append(" <> ");
                    queryPayload.AddParameter(matchCondition);
                    break;
                case DslOperator.Like:
                    queryPayload.Append(" LIKE '%' + ");
                    queryPayload.AddParameter(matchCondition);
                    queryPayload.Append(" + '%'");
                    break;
                case DslOperator.NotLike:
                    queryPayload.Append(" NOT LIKE '%' + ");
                    queryPayload.AddParameter(matchCondition);
                    queryPayload.Append(" + '%'");
                    break;
                case DslOperator.In:
                    queryPayload.Append(" IN (");
                    queryPayload.AddParameter(matchCondition);
                    queryPayload.Append(")");
                    break;
                case DslOperator.NotIn:
                    queryPayload.Append(" NOT IN (");
                    queryPayload.AddParameter(matchCondition);
                    queryPayload.Append(")");
                    break;
                default:
                    throw new Exception("DSL Operator not supported for SQL query generation: " + matchCondition.Operator);
            }

            queryPayload.AddNewLine();
        }

        
    }
}
