using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
            // Date to right format for database.
            string dateFormatted = task.DateIssued.Year+
                "-"+task.DateIssued.Month+
                "-"+task.DateIssued.Day+
                " "+task.DateIssued.Hour+
                ":"+task.DateIssued.Minute+
                ":"+task.DateIssued.Second;
            string query = "INSERT INTO ServiceTask  " +
                "(TargetId, Criticality, DateIssued, Description, Closed) " +
                $"VALUES ({task.TargetId}, {(int)task.Criticality}, \"{dateFormatted}\"," +
                $" \"{task.Description}\", {task.Closed});";
            return await Task.FromResult(RunQuery(query));
        }

        public async Task<IAsyncResult> DeleteAsync(int TaskId)
        {
            string query = "DELETE FROM ServiceTask " +
                $"WHERE TaskId = {TaskId};";
            return await Task.FromResult(RunQuery(query));
        }

        public async Task<IEnumerable<ServiceTask>> GetAllAsync()
        {
            string query = "SELECT * FROM ServiceTask " +
                "ORDER BY Criticality, DateIssued desc;";
            return await await Task.FromResult(RunQuery(query));
        }

        public async Task<IEnumerable<ServiceTask>> GetAsync(int TargetId)
        {
            string query = "SELECT * FROM ServiceTask " +
                $"WHERE TargetId = {TargetId} " +
                "ORDER BY Criticality, DateIssued desc;";
            return await await Task.FromResult(RunQuery(query));
        }

        public async Task<IEnumerable<ServiceTask>> SearchAsync(Search SearchData)
        {
            bool first = true; // To keep track of ' AND ' separators
            string query = "SELECT * FROM ServiceTask WHERE ";
            
            // If TaskId is provided return matching one
            if (SearchData.TaskId != 0)
            {
                query += $"TaskId = {SearchData.TaskId};";
                return await await Task.FromResult(RunQuery(query));
            }

            // Construct query

            // Dates
            if (SearchData.IssuedBefore != new DateTime()) // If param is provided
            {
                string dateFormatted = SearchData.IssuedBefore.Year + "-" +
                    SearchData.IssuedBefore.Month + "-" +
                    SearchData.IssuedBefore.Day;
                query += $"DateIssued < \"{dateFormatted}\"";
                first = false;
            }

            if (SearchData.IssuedAfter != new DateTime()) // If param is provided
            {
                if (!first)
                {
                    query += " AND ";
                }
                string dateFormatted = SearchData.IssuedAfter.Year + "-" +
                    SearchData.IssuedAfter.Month + "-" +
                    SearchData.IssuedAfter.Day;
                query += $"DateIssued > \"{dateFormatted}\"";
                first = false;
            }

            // Target
            if (SearchData.TargetId != 0)
            {
                if (!first)
                {
                    query += " AND ";
                }
                query += $"TargetId = {SearchData.TargetId}";
                first = false;
            }

            // Closed
            if (SearchData.Closed != 0) 
            {
                if (!first)
                {
                    query += " AND ";
                }

                int Closed;
                if (SearchData.Closed == 1)
                {
                    Closed = 1;
                } else
                {
                    Closed = 0;
                }
                query += $"Closed = {Closed}";
                first = false;
            }

            // Description contains
            if (SearchData.DescContains != null) 
            {
                if (!first)
                {
                    query += " AND ";
                }
                query += $"Description LIKE '%{SearchData.DescContains}%'";
                first = false;
            }

            // Criticality and order by
            int MinCriticality;
            if (SearchData.MinCriticality == 0)
            {
                MinCriticality = (int)TaskCriticality.Mild; // All criticalities
            } else
            {
                MinCriticality = SearchData.MinCriticality;
            }

            if (!first)
            {
                query += " AND ";
            }
            query += $"Criticality <= {MinCriticality}";
            query += " ORDER BY Criticality, DateIssued desc;";

            return await await Task.FromResult(RunQuery(query));
        }

        public async Task<IAsyncResult> UpdateAsync(ServiceTask UpdateData, int TaskId)
        {
            bool first = true; // To keep track of ',' separators

            // Construct query
            string query = "UPDATE ServiceTask SET ";
            if (UpdateData.TargetId != 0)
            {
                query += $"TargetId = {UpdateData.TargetId}";
                first = false;
            }
            
            if (UpdateData.Criticality != 0)
            {
                if (!first)
                {
                    query += ", ";
                }
                query += $"Criticality = {(int)UpdateData.Criticality}";
                first = false;
            }

            if (UpdateData.Description != null)
            {
                if (!first)
                {
                    query += ", ";
                }
                query += $"Description = \"{UpdateData.Description}\"";
                first = false;

            }

            if (!first)
            {
                query += ", ";
            }

            // Closed status is set to false always if true is not provided
            query += $"Closed = {UpdateData.Closed}";

            query += $" WHERE TaskId = {TaskId};";

            return await Task.FromResult(RunQuery(query));
        }

        private async Task<IEnumerable<ServiceTask>> RunQuery(string query, string db = "SMDB")
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString(db)))
            {
                var output = cnn.Query<ServiceTask>(query, new DynamicParameters());
                return await Task.FromResult(output);
            }
        }

        private static string LoadConnectionString(string id)
        {
            return $"Data Source=.\\{id}.db;Version=3;";
        }
    }
}
