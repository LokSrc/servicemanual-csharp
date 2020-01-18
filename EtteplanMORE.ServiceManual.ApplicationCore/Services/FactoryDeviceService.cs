using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
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
            StringBuilder safeQuery = new StringBuilder(@"SELECT * FROM FactoryDevice WHERE ");
            safeQuery.Append("Id = @Id;");

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("Id", id);

            var output = await RunQuerySafe(safeQuery.ToString(), dynamicParameters);
            if (output.Count() > 0)
            {
                // There should be only one always
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

        /// <summary>
        ///     Sql injection protected
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="param">query params</param>
        /// <param name="db"></param>
        /// <returns></returns>
        private async Task<IEnumerable<FactoryDevice>> RunQuerySafe(string query, DynamicParameters dynamicParameters, string db = "SMDB")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString(db)))
            {
                var output = cnn.Query<FactoryDevice>(query, dynamicParameters);
                return await Task.FromResult(output);
            }
        }

        private static string LoadConnectionString(string id)
        {
            return $"Data Source=.\\{id}.db;Version=3;";
        }
    }
}