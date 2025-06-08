using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TableShot.Models;
using Amazon;
using Amazon.CognitoIdentity;

namespace TableShot.ViewModels
{
    public partial class TableItemsViewModel
    {
        DynamoDBContext _db;
        const string IdentityPoolId = "eu-north-1:39350368-a57a-4e6b-b949-9c7068b6e368";
        const string UserPoolId = "eu-north-1_0ELf78r9L";
        static readonly RegionEndpoint Region = RegionEndpoint.EUNorth1;


        public TableItemsViewModel()
        {
            // ← your Cognito Identity Pool ID here  


            var creds = new CognitoAWSCredentials(
                IdentityPoolId,
                RegionEndpoint.EUNorth1);

            var client = new AmazonDynamoDBClient(
                creds,
                RegionEndpoint.EUNorth1);


        }

        // ---- Definitions ----  

        public Task CreateTableDefinitionAsync(string tableId, int addCols)
        {
            var def = new TableDefinition
            {
                TableId = tableId,
                AdditionalColumns = addCols,
                ColumnNames = Enumerable.Range(1, addCols)
                                              .Select(i => $"C{i}")
                                              .ToList()
            };
            return _db.SaveAsync(def);
        }

        public Task<List<TableDefinition>> GetAllTableDefinitionsAsync()
        {
            return _db
                .ScanAsync<TableDefinition>(Enumerable.Empty<ScanCondition>())
                .GetRemainingAsync();
        }

        public Task<TableDefinition> GetTableDefinitionAsync(string tableId)
        {
            return _db.LoadAsync<TableDefinition>(tableId);
        }

        // ---- Rows ----  

        public async Task<List<TableRow>> GetRowsForTableAsync(string tableId)
        {
            var rows = await _db.QueryAsync<TableRow>(tableId)
                                .GetRemainingAsync();
            return rows;
        }

        public Task AddRowAsync(TableRow row)
        {
            return _db.SaveAsync(row);
        }

        public Task UpdateRowAsync(TableRow row)
        {
            return _db.SaveAsync(row);
        }
        public async Task InitAsync(string idToken)
        {
            var creds = new CognitoAWSCredentials(IdentityPoolId, Region);

            // associate the user-pool IdToken so creds become "authenticated"
            string provider = $"cognito-idp.{Region.SystemName}.amazonaws.com/{UserPoolId}";
            creds.AddLogin(provider, idToken);

            // now build the real DynamoDBContext
            var client = new AmazonDynamoDBClient(creds, Region);
            _db = new DynamoDBContext(client);

            // optional warm-up call to verify permissions


            // (optionally) you can warm up a call here to verify permissions
            //await _db.ScanAsync<TableDefinition>(new List<ScanCondition>()).GetRemainingAsync();
        }
    }
}
