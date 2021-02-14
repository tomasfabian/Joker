using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kafka.DotNet.ksqlDB.KSql.RestApi.Parsers
{
  internal class JsonArrayParser
  {
    internal string CreateJson(string[] headerColumns, string row)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendLine("{");

      bool isFirst = true;

      var rowValues = Split(row);

      foreach (var column in headerColumns.Zip(rowValues.Select(c => c.Trim(' ')), (s, s1) => new { ColumnName = s, Value = s1 }))
      {
        if (!isFirst)
        {
          stringBuilder.Append(",");
        }

        stringBuilder.AppendLine($"\"{column.ColumnName}\": {column.Value}");

        isFirst = false;
      }

      stringBuilder.AppendLine("}");

      return stringBuilder.ToString();
    }

    readonly char[] structuredTypeStarted = { '[', '{' };
    readonly char[] structuredTypeEnded = { ']', '}' };

    private IEnumerable<string> Split(string row)
    {
      var stringBuilder = new StringBuilder();
      bool isStructuredType = false;

      foreach(var ch in row)
      {
        if(structuredTypeStarted.Contains(ch))
          isStructuredType = true;
		 
        if(structuredTypeEnded.Contains(ch))
          isStructuredType = false;
					
        if(ch != ',' || isStructuredType)
          stringBuilder.Append(ch);

        if(ch == ',' && !isStructuredType)
        {
          yield return stringBuilder.ToString();
          stringBuilder.Clear();
        }
      }
	
      yield return stringBuilder.ToString();
    }
  }
}