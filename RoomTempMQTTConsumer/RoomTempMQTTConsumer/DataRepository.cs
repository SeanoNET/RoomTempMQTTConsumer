using Dapper;
using RoomTempMQTTConsumer.Entities;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RoomTempMQTTConsumer
{
    internal class DataRepository
    {
        private readonly string _dataSource;

        public DataRepository(string dataSource)
        {
            if (string.IsNullOrEmpty(dataSource))
                throw new ArgumentNullException("Missing datasource connection string", nameof(dataSource));

            this._dataSource = dataSource;
        }
        public bool SavePayload(MetricData payload)
        {

            using (IDbConnection db = new SqlConnection(_dataSource))
            {
                string insertQuery = @"INSERT INTO [dbo].[Test]([Temperature], [Humidity], [MeasuredAt]) VALUES (@Temperature, @Humidity, @MeasuredAt)";

                var result = db.Execute(insertQuery, payload);
            }

            return true;
        }
    }
}
