using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace TableShot.Models
{
    [DynamoDBTable("TableDefinitions")]
    public class TableDefinition
    {
        [DynamoDBHashKey]
        public string TableId { get; set; }

        [DynamoDBProperty]
        public int AdditionalColumns { get; set; }

        [DynamoDBProperty]
        public List<string> ColumnNames { get; set; }
    }
}
