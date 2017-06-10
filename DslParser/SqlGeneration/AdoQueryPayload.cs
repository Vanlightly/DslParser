using DslParser.DataRepresentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DslParser.SqlGeneration
{
    public class AdoQueryPayload
    {
        private int _paramCounter;
        private StringBuilder _sb;

        public AdoQueryPayload()
        {
            Parameters = new List<SqlParameter>();
            _sb = new StringBuilder();
        }

        public IList<SqlParameter> Parameters { get; private set; }

        public void Append(string queryText)
        {
            _sb.Append(queryText);
        }

        public void AppendLine(string queryText)
        {
            _sb.Append(queryText + Environment.NewLine);
        }

        public void AddNewLine()
        {
            _sb.Append(Environment.NewLine);
        }

        public void AppendColumnName(DslObject dslObject)
        {
            _sb.Append(GetColumnName(dslObject));
        }

        public void AddParameter(MatchCondition matchCondition)
        {
            if (matchCondition.Operator == DslOperator.In || matchCondition.Operator == DslOperator.NotIn)
            {
                AddInParameters(matchCondition.Object, matchCondition.Values);
            }
            else
            {
                AddParameter(matchCondition.Object, matchCondition.Value);
            }
        }

        public void AddFromDateParameter(DateTime dateValue)
        {
            AddDateParameter(dateValue, "@FromDate");
        }

        public void AddToDateParameter(DateTime dateValue)
        {
            AddDateParameter(dateValue, "@ToDate");
        }

        public string GetSqlText()
        {
            return _sb.ToString();
        }

        private void AddDateParameter(DateTime dateValue, string paramName)
        {
            _sb.Append(paramName);

            if (!Parameters.Any(x => x.ParameterName.Equals(paramName)))
            {
                var parameter = new SqlParameter(paramName, SqlDbType.DateTime);
                parameter.Value = dateValue;

                Parameters.Add(parameter);
            }
        }

        private void AddInParameters(DslObject dslObject, List<string> values)
        {
            int counter = 0;
            foreach (var value in values)
            {
                if (counter > 0)
                    _sb.Append(",");

                IncrementParamCounter();
                var paramName = GetParameterName();
                var parameter = new SqlParameter(paramName, SqlDbType.VarChar, GetVarcharLength(dslObject));
                parameter.Value = value;
                Parameters.Add(parameter);
                _sb.Append(paramName);

                counter++;
            }
        }

        private void AddParameter(DslObject dslObject, string value)
        {
            IncrementParamCounter();
            var paramName = GetParameterName();
            _sb.Append(paramName);

            var parameter = new SqlParameter(paramName, SqlDbType.VarChar, GetVarcharLength(dslObject));
            parameter.Value = value;
            Parameters.Add(parameter);
        }

        private void IncrementParamCounter()
        {
            _paramCounter++;
        }

        private string GetParameterName()
        {
            return "@Param" + _paramCounter;
        }

        private string GetColumnName(DslObject dslObject)
        {
            switch (dslObject)
            {
                case DslObject.Application:
                    return "ED.ApplicationId";
                case DslObject.Fingerprint:
                    return "ED.FingerprintText";
                case DslObject.StackFrame:
                    return "EB.StackFrame";
                case DslObject.ExceptionType:
                    return "EB.ExceptionType";
                case DslObject.Message:
                    return "T.MessageDetails";
                default:
                    throw new Exception("LQL object not supported for SQL generation: " + dslObject.ToString());
            }
        }

        private int GetVarcharLength(DslObject dslObject)
        {
            switch (dslObject)
            {
                case DslObject.Application:
                    return 200;
                case DslObject.StackFrame:
                    return 1000;
                case DslObject.ExceptionType:
                    return 200;
                case DslObject.Message:
                    return 1000;
                case DslObject.Fingerprint:
                    return 32;
                default:
                    return 50;
            }
        }
    }
}
