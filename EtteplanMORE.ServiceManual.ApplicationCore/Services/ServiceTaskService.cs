using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Services
{
    public class ServiceTaskService : IServiceTaskService
    {
        public async Task<IAsyncResult> CreateAsync(ServiceTask task)
        {
            StringBuilder safeQuery = new StringBuilder(@"INSERT INTO ServiceTask (TargetId, Criticality, DateIssued, Description, Closed) VALUES ");
            safeQuery.Append("(@TargetId, @Criticality, @Date, @Description, @Closed);");

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("TargetId", task.TargetId);
            dynamicParameters.Add("Criticality", task.Criticality);
            dynamicParameters.Add("Date", FormatDate(task.DateIssued));
            dynamicParameters.Add("Description", task.Description);
            dynamicParameters.Add("Closed", task.Closed);

            return await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
        } 

        public async Task<IAsyncResult> DeleteAsync(int TaskId)
        {
            StringBuilder safeQuery = new StringBuilder(@"DELETE FROM ServiceTask WHERE TaskId = @TaskId;");

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("TaskId", TaskId);

            return await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
        }

        public async Task<IEnumerable<ServiceTask>> GetAllAsync()
        {
            // No user input -> we can run this without dynamicparameters
            string query = "SELECT * FROM ServiceTask " +
                "ORDER BY Criticality, DateIssued desc;";
            return await await Task.FromResult(RunQuery(query));
        }

        public async Task<IEnumerable<ServiceTask>> GetAsync(int TargetId)
        {
            StringBuilder safeQuery = new StringBuilder(@"SELECT * FROM ServiceTask WHERE TargetId = @TargetId ");
            safeQuery.Append("ORDER BY Criticality, DateIssued desc;");

            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("TargetId", TargetId);

            return await await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
        }

        public async Task<IEnumerable<ServiceTask>> SearchAsync(Search SearchData)
        {
            StringBuilder safeQuery = new StringBuilder(@"SELECT * FROM ServiceTask WHERE ");
            DynamicParameters dynamicParameters = new DynamicParameters();

            // Criticality added always
            int MinCriticality = SearchData.MinCriticality != 0 ?
                SearchData.MinCriticality : (int)TaskCriticality.Mild;
            safeQuery.Append("Criticality <= @MinCriticality ");
            dynamicParameters.Add("MinCriticality", MinCriticality);

            // If TaskId is provided return matching one
            if (SearchData.TaskId != 0)
            {
                safeQuery.Append("AND TaskId = @TaskId;");
                dynamicParameters.Add("TaskId", SearchData.TaskId);
                return await await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
            }

            // Dates
            if (SearchData.IssuedBefore != new DateTime()) // If param is provided
            {
                safeQuery.Append("AND DateIssued < @IssuedBefore ");
                dynamicParameters.Add("IssuedBefore", FormatDate(SearchData.IssuedBefore));
            }

            if (SearchData.IssuedAfter != new DateTime()) // If param is provided
            {
                safeQuery.Append("AND DateIssued > @IssuedAfter ");
                dynamicParameters.Add("IssuedAfter", FormatDate(SearchData.IssuedAfter));
            }

            // Target
            if (SearchData.TargetId != 0)
            {
                safeQuery.Append("AND TargetId = @TargetId ");
                dynamicParameters.Add("TargetId", SearchData.TargetId);
            }

            // Closed
            if (SearchData.Closed != 0)
            {
                int Closed = SearchData.Closed == 1 ? 1 : 0;
                safeQuery.Append("AND Closed = @Closed ");
                dynamicParameters.Add("Closed", Closed);
            }

            // Description contains
            if (SearchData.DescContains != null)
            {
                safeQuery.Append("AND Description LIKE @DescContains ");
                dynamicParameters.Add("DescContains", "%" + SearchData.DescContains + "%");
            }

            safeQuery.Append("ORDER BY Criticality, DateIssued desc;");

            return await await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
        }

        public async Task<IAsyncResult> UpdateAsync(ServiceTask UpdateData, int TaskId)
        {
            StringBuilder safeQuery = new StringBuilder(@"UPDATE ServiceTask SET ");
            DynamicParameters dynamicParameters = new DynamicParameters();

            // Closed status is set to false always if true is not provided
            safeQuery.Append("Closed = @Closed");
            dynamicParameters.Add("Closed", UpdateData.Closed);

            // TargetId
            if (UpdateData.TargetId != 0)
            {
                safeQuery.Append(", TargetId = @TargetId");
                dynamicParameters.Add("TargetId", UpdateData.TargetId);
            }
            
            // Criticality
            if (UpdateData.Criticality != 0)
            {
                safeQuery.Append(", Criticality = @Criticality");
                dynamicParameters.Add("Criticality", (int)UpdateData.Criticality);
            }

            // Description
            if (UpdateData.Description != null)
            {
                safeQuery.Append(", Description = @Description");
                dynamicParameters.Add("Description", UpdateData.Description);
            }

            safeQuery.Append(" WHERE TaskId = @TaskId;");
            dynamicParameters.Add("TaskId", TaskId);

            return await Task.FromResult(RunQuerySafe(safeQuery.ToString(), dynamicParameters));
        }

        private async Task<IEnumerable<ServiceTask>> RunQuery(string query, string db = "SMDB")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString(db)))
            {
                var output = cnn.Query<ServiceTask>(query, new DynamicParameters());
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
        private async Task<IEnumerable<ServiceTask>> RunQuerySafe(string query, DynamicParameters dynamicParameters, string db = "SMDB")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString(db)))
            {
                var output = cnn.Query<ServiceTask>(query, dynamicParameters);
                return await Task.FromResult(output);
            }
        }

        private static string LoadConnectionString(string id)
        {
            return $"Data Source=.\\{id}.db;Version=3;";
        }

        private static string FormatDate(DateTime date)
        {
            // Date to right format for database.
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
