using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Services
{
    public class FactoryDeviceService : IFactoryDeviceService
    {
        public async Task<IEnumerable<FactoryDevice>> GetAll()
        {
            string query = "SELECT * FROM FactoryDevice";
            return await await Task.FromResult(RunQuery(query));
        }

        public async Task<FactoryDevice> Get(int id)
        {
            string query = "SELECT * FROM FactoryDevice " +
                $"WHERE Id = {id};";
            var output = await RunQuery(query);
            if (output.Count() > 0)
            {
                return output.First();
            }
            return null;
        }

        private async Task<IEnumerable<FactoryDevice>> RunQuery(string query, string db = "SMDB")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString(db)))
            {
                var output = cnn.Query<FactoryDevice>(query, new DynamicParameters());
                return await Task.FromResult(output);
            }
        }

        private static string LoadConnectionString(string id = "SMDB")
        {
            return $"Data Source=.\\{id}.db;Version=3;";
        }
    }
}