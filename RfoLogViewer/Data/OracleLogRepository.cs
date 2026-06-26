using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using RfoLogViewer.Models;

namespace RfoLogViewer.Data
{
    public sealed class OracleLogRepository : IDisposable
    {
        private readonly OracleConnection _connection;

        private readonly string login;
        private readonly string dataSource;

        public OracleLogRepository(string dataSource, string user, string password)
        {
            this.login = user;
            this.dataSource = dataSource;
            var builder = new OracleConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = user,
                Password = password
            };
            builder["Persist Security Info"] = true;
            this._connection = new OracleConnection(builder.ConnectionString);
            this._connection.Open();
        }

        public long GetUserId(string login)
        {
            using (var cmd = this.CreateCommand(LogQueries.GetUserId))
            {
                cmd.Parameters.Add("login", OracleDbType.Varchar2, login, ParameterDirection.Input);
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    throw new InvalidOperationException("Login not found in cd_users. Please verify the user name.");
                }
                return Convert.ToInt64(result);
            }
        }

        public string GetCurrentConnectionString()
        {
            return $"{this.login}@{this.dataSource}";
        }

        public void OpenContext(long contextId, long userId, string module = "RFoLogViewer")
        {
            using (var cmd = this.CreateCommand(LogQueries.OpenContext))
            {
                cmd.Parameters.Add("contextId", OracleDbType.Int64, contextId, ParameterDirection.Input);
                cmd.Parameters.Add("userId", OracleDbType.Int64, userId, ParameterDirection.Input);
                cmd.Parameters.Add("module", OracleDbType.Varchar2, module, ParameterDirection.Input);
                cmd.ExecuteNonQuery();
            }
        }

        public long GetCurrentContextId()
        {
            using (var cmd = this.CreateCommand(LogQueries.GetCurrentContextId))
            {
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        public IList<ContextEntry> GetAccessibleContexts()
        {
            var items = new List<ContextEntry>();
            using (var cmd = this.CreateCommand(LogQueries.GetAccessibleContexts))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new ContextEntry
                    {
                        ContextId = reader.GetInt64(reader.GetOrdinal("context_id")),
                        ReportingDate = reader.GetDateTime(reader.GetOrdinal("reporting_date")),
                        Workspace = reader.IsDBNull(reader.GetOrdinal("workspace_name"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("workspace_name")),
                        Description = reader.IsDBNull(reader.GetOrdinal("description"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("description"))
                    });
                }
            }

            return items;
        }

        public IList<RootLogKeyNodeInfo> GetRootLogKeys(DateTime begin, DateTime end)
        {
            var items = new List<RootLogKeyNodeInfo>();
            using (var cmd = this.CreateCommand(LogQueries.RootLogKeys))
            {
                AddDateParameter(cmd, "beginDate", begin);
                AddDateParameter(cmd, "endDate", end);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new RootLogKeyNodeInfo
                        {
                            Label = reader.GetString(0),
                            RootLogKey = reader.GetString(1),
                            Status = LogNodeStatusHelper.FromCode(reader.GetString(3))
                        });
                    }
                }
            }
            return items;
        }

        public LogNodeStatus GetPeriodStatus(DateTime begin, DateTime end)
        {
            return this.ReadStatusCode(LogQueries.PeriodStatus, begin, end);
        }

        public LogNodeStatus GetOrphanStatus(DateTime begin, DateTime end)
        {
            return this.ReadStatusCode(LogQueries.OrphanStatus, begin, end);
        }

        public LogNodeStatus GetLogSessionStatus(long logStructId)
        {
            using (var cmd = this.CreateCommand(LogQueries.LogSessionStatus))
            {
                cmd.Parameters.Add("logStructId", OracleDbType.Int64, logStructId, ParameterDirection.Input);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return LogNodeStatus.Normal;
                    }
                    var errors = reader.GetDecimal(reader.GetOrdinal("total_errors"));
                    var warnings = reader.GetDecimal(reader.GetOrdinal("total_warnings"));
                    return LogNodeStatusHelper.FromCounts(errors, warnings);
                }
            }
        }

        public int GetLogSessionPictureIndex(long logStructId)
        {
            using (var cmd = this.CreateCommand(LogQueries.LogSessionPictureIndex))
            {
                cmd.Parameters.Add("logStructId", OracleDbType.Int64, logStructId, ParameterDirection.Input);
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return LogTreePictureIndex.SessionOk;
                }

                return Convert.ToInt32(result);
            }
        }

        private LogNodeStatus ReadStatusCode(string sql, DateTime begin, DateTime end)
        {
            using (var cmd = this.CreateCommand(sql))
            {
                AddDateParameter(cmd, "beginDate", begin);
                AddDateParameter(cmd, "endDate", end);
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return LogNodeStatus.Normal;
                }
                return LogNodeStatusHelper.FromCode(Convert.ToString(result));
            }
        }

        public IList<LogSessionNodeInfo> GetRootLogSessions(DateTime begin, DateTime end, string rootLogKey)
        {
            return this.ReadLogSessionNodes(LogQueries.RootLogSessions, cmd =>
            {
                AddDateParameter(cmd, "beginDate", begin);
                AddDateParameter(cmd, "endDate", end);
                cmd.Parameters.Add("rootLogKey", OracleDbType.Varchar2, rootLogKey, ParameterDirection.Input);
            });
        }

        public IList<LogSessionNodeInfo> GetChildLogSessions(long parentLogStructId, decimal? rootDuration)
        {
            return this.ReadLogSessionNodes(LogQueries.ChildLogSessions, cmd =>
            {
                cmd.Parameters.Add("parentLogStructId", OracleDbType.Int64, parentLogStructId, ParameterDirection.Input);
                if (rootDuration.HasValue)
                {
                    cmd.Parameters.Add("rootDuration", OracleDbType.Decimal, rootDuration.Value, ParameterDirection.Input);
                }
                else
                {
                    cmd.Parameters.Add("rootDuration", OracleDbType.Decimal, DBNull.Value, ParameterDirection.Input);
                }
            });
        }

        public string GetLogSessionLabel(long logStructId, decimal? rootDuration)
        {
            using (var cmd = this.CreateCommand(LogQueries.UpdateLogSessionLabel))
            {
                cmd.Parameters.Add("logStructId", OracleDbType.Int64, logStructId, ParameterDirection.Input);
                if (rootDuration.HasValue)
                {
                    cmd.Parameters.Add("rootDuration", OracleDbType.Decimal, rootDuration.Value, ParameterDirection.Input);
                }
                else
                {
                    cmd.Parameters.Add("rootDuration", OracleDbType.Decimal, DBNull.Value, ParameterDirection.Input);
                }
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public string GetLogQuery(long logId)
        {
            using (var cmd = this.CreateCommand(LogQueries.GetLogQuery))
            {
                cmd.Parameters.Add("logId", OracleDbType.Int64, logId, ParameterDirection.Input);
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    if (!reader.Read() || reader.IsDBNull(0))
                    {
                        return null;
                    }

                    if (reader.GetFieldType(0) == typeof(string))
                    {
                        return reader.GetString(0);
                    }

                    var clob = reader.GetOracleClob(0);
                    if (clob == null || clob.IsNull)
                    {
                        return null;
                    }

                    return clob.Value;
                }
            }
        }

        public DataTable LoadLogStruct(string whereClause)
        {
            var sql = string.Format(LogQueries.LogStructGrid, whereClause);
            using (var adapter = new OracleDataAdapter(sql, this._connection))
            {
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        public IList<LogEntry> LoadLogTable(string whereClause)
        {
            var sql = string.Format(LogQueries.LogTableMain, whereClause);
            var entries = new List<LogEntry>();
            using (var cmd = this.CreateCommand(sql))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    entries.Add(MapLogEntry(reader));
                }
            }
            return entries;
        }

        private IList<LogSessionNodeInfo> ReadLogSessionNodes(string sql, Action<OracleCommand> bind)
        {
            var items = new List<LogSessionNodeInfo>();
            using (var cmd = this.CreateCommand(sql))
            {
                bind(cmd);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LogSessionNodeInfo
                        {
                            Label = reader.GetString(reader.GetOrdinal("label")),
                            LogStructId = reader.GetInt64(reader.GetOrdinal("log_struct_id")),
                            ParentLogStructId = reader.IsDBNull(reader.GetOrdinal("parent_log_struct_id"))
                                ? (long?)null
                                : reader.GetInt64(reader.GetOrdinal("parent_log_struct_id")),
                            RootDurationSeconds = reader.IsDBNull(reader.GetOrdinal("duration_seconds"))
                                ? (decimal?)null
                                : reader.GetDecimal(reader.GetOrdinal("duration_seconds")),
                            HasChildren = reader.GetString(reader.GetOrdinal("has_children")) == "Y",
                            PictureIndex = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("picture_index"))),
                            Status = LogNodeStatusHelper.FromCounts(
                                reader.GetDecimal(reader.GetOrdinal("total_errors")),
                                reader.GetDecimal(reader.GetOrdinal("total_warnings")))
                        });
                    }
                }
            }
            return items;
        }

        private static LogEntry MapLogEntry(IDataRecord reader)
        {
            return new LogEntry
            {
                SessionId = GetNullableLong(reader, "session_id"),
                LogId = reader.GetInt64(reader.GetOrdinal("log_id")),
                LogKey = GetString(reader, "log_key"),
                LogType = GetString(reader, "log_type"),
                Severity = GetNullableInt(reader, "severity"),
                TechFunc = GetString(reader, "tech_func"),
                Depth = GetNullableInt(reader, "depth"),
                DateTime = GetNullableDateTime(reader, "datetime"),
                ProductName = GetString(reader, "product_name"),
                Function = GetString(reader, "function"),
                Step = GetString(reader, "step"),
                Message = GetString(reader, "message"),
                Parameters = GetString(reader, "parameters"),
                NumMsg = GetNullableLong(reader, "nummsg"),
                ProcessDesc = GetString(reader, "process_desc"),
                TaskId = GetNullableLong(reader, "task_id"),
                IsMySessionId = GetString(reader, "is_my_session_id"),
                RootTaskId = GetNullableLong(reader, "root_task_id"),
                Machine = GetString(reader, "machine"),
                Cooperator = GetString(reader, "cooperator"),
                LogStructId = GetNullableLong(reader, "log_struct_id"),
                RootLogKey = GetString(reader, "root_log_key"),
                PartitionKey = GetString(reader, "partition_key")
            };
        }

        private OracleCommand CreateCommand(string sql)
        {
            var cmd = this._connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.BindByName = true;
            return cmd;
        }

        private static void AddDateParameter(OracleCommand cmd, string name, DateTime value)
        {
            cmd.Parameters.Add(name, OracleDbType.Date, value.Date, ParameterDirection.Input);
        }

        private static string GetString(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        private static long? GetNullableLong(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (long?)null : Convert.ToInt64(reader.GetValue(ordinal));
        }

        private static int? GetNullableInt(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (int?)null : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static DateTime? GetNullableDateTime(IDataRecord reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        public void Dispose()
        {
            this._connection?.Dispose();
        }
    }

    public sealed class RootLogKeyNodeInfo
    {
        public string Label { get; set; }
        public string RootLogKey { get; set; }
        public LogNodeStatus Status { get; set; }
    }

    public sealed class LogSessionNodeInfo
    {
        public string Label { get; set; }
        public long LogStructId { get; set; }
        public long? ParentLogStructId { get; set; }
        public decimal? RootDurationSeconds { get; set; }
        public bool HasChildren { get; set; }
        public int PictureIndex { get; set; }
        public LogNodeStatus Status { get; set; }
    }
}
