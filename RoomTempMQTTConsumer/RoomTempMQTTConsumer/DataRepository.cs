using Dapper;
using Microsoft.Extensions.Configuration;
using RoomTempMQTTConsumer.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RoomTempMQTTConsumer
{
    internal class DataRepository
    {
        private readonly string _dataSource;
        private readonly string _tableName;

        public DataRepository(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.GetSection("DataSource").Value))
                throw new ArgumentNullException("Missing datasource connection string");

            if (string.IsNullOrEmpty(configuration.GetSection("TableName").Value))
                throw new ArgumentNullException("Missing table name");
    
            this._dataSource = configuration.GetSection("DataSource").Value;
            this._tableName = configuration.GetSection("TableName").Value;
        }
        public bool SavePayload(MetricData payload)
        {

            using (IDbConnection db = new SqlConnection(_dataSource))
            {
                string insertQuery = $"INSERT INTO [dbo].[{_tableName}]([Temperature], [Humidity], [MeasuredAt]) VALUES (@Temperature, @Humidity, @MeasuredAt)";

                var result = db.Execute(insertQuery, payload);
            }

            return true;
        }
    }
}
