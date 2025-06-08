using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace TableShot.Models
{
    [DynamoDBTable("TableRows")]
    public class TableRow
    {
        [DynamoDBHashKey]
        public string TableId { get; set; }

        [DynamoDBRangeKey]
        public string RowId { get; set; }

        [DynamoDBProperty]
        public string Name { get; set; }

        // maps C1…CN → value
        [DynamoDBProperty]
        public Dictionary<string, double> Values { get; set; }
    }
}
