using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;

namespace FabView.Utility {
    public class DbHelper {
        private static string dbProviderName = ConfigurationManager.AppSettings["DbHelperProvider"];
        private static string dbConnectionString = ConfigurationManager.AppSettings["DbHelperConnectionString"];

        private DbConnection connection;
        public DbHelper() {
            this.connection = CreateConnection(DbHelper.dbConnectionString);
        }
        public DbHelper(string connectionString) {
            this.connection = CreateConnection(connectionString);
        }
        public static DbConnection CreateConnection() {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = DbHelper.dbConnectionString;
            return dbconn;
        }
        public static DbConnection CreateConnection(string connectionString) {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            return dbconn;
        }

        public DbCommand GetStoredProcCommond(string storedProcedure) {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandText = storedProcedure;
            dbCommand.CommandType = CommandType.StoredProcedure;
            return dbCommand;
        }
        public DbCommand GetSqlStringCommond(string sqlQuery) {
            DbCommand dbCommand = connection.CreateCommand();
            dbCommand.CommandText = sqlQuery;
            dbCommand.CommandType = CommandType.Text;
            return dbCommand;
        }

        #region Add Parameter
        public void AddParameterCollection(DbCommand cmd, DbParameterCollection dbParameterCollection) {
            foreach (DbParameter dbParameter in dbParameterCollection) {
                cmd.Parameters.Add(dbParameter);
            }
        }
        public void AddOutParameter(DbCommand cmd, string parameterName, DbType dbType, int size) {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(dbParameter);
        }
        public void AddInParameter(DbCommand cmd, string parameterName, DbType dbType, object value) {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(dbParameter);
        }
        public void AddReturnParameter(DbCommand cmd, string parameterName, DbType dbType) {
            DbParameter dbParameter = cmd.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(dbParameter);
        }
        public DbParameter GetParameter(DbCommand cmd, string parameterName) {
            return cmd.Parameters[parameterName];
        }

        #endregion

        #region Execute
        public DataSet ExecuteDataSet(DbCommand cmd) {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            dbDataAdapter.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(DbCommand cmd) {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        //xujiyuan 2017/08/15
        public DataTable ExecuteDataTable(String sql) {
            DbCommand cmd = GetSqlStringCommond(sql);
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;

            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DbDataReader ExecuteReader(DbCommand cmd) {
            cmd.Connection.Open();
            DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }
        public int ExecuteNonQuery(DbCommand cmd) {
            cmd.Connection.Open();
            int ret = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            return ret;
        }

        /// <summary>
        /// 执行Delete、Update、Add操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>-1：SQL异常</returns>
        /// <returns>0：受影响行数为0，如执行删除操作时，条件不成立，删除0行</returns>
        /// <returns>>0：受影响行数>0，SQL执行成功</returns>
        public int ExecuteNonQuery(string sql) {
            DbCommand cmd = GetSqlStringCommond(sql);
            cmd.Connection.Open();
            int ret = -1;
            try {
                ret = cmd.ExecuteNonQuery();
            } catch (SqlException e) {
                ret = -1;
            } finally {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return ret;
        }

        /// <summary>
        /// 执行Delete、Update、Add操作
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>-1：SQL异常</returns>
        /// <returns>0：受影响行数为0，如执行删除操作时，条件不成立，删除0行</returns>
        /// <returns>大于1：SQL执行成功</returns>
        public int ExecuteNonQuery(string sql, Result rs) {
            DbCommand cmd = GetSqlStringCommond(sql);
            cmd.Connection.Open();
            int ret = -1;
            try {
                ret = cmd.ExecuteNonQuery();
            } catch (SqlException e) {
                rs.msg += e.ToString() + "\r\n";
                ret = -1;
            } catch (Exception e) {
                rs.msg += e.ToString() + "\r\n";
                ret = -1;
            } finally {
                cmd.Connection.Close();
            }
            cmd.Connection.Close();
            return ret;
        }

        public object ExecuteScalar(DbCommand cmd) {
            cmd.Connection.Open();
            object ret = cmd.ExecuteScalar();
            cmd.Connection.Close();
            return ret;
        }

        public int ExecuteScalar(string sql) {
            DbCommand cmd = GetSqlStringCommond(sql);
            cmd.Connection.Open();
            int ret = int.Parse(cmd.ExecuteScalar().ToString());
            cmd.Connection.Close();
            return ret;
        }
        #endregion

        #region Execute with transaction
        public DataSet ExecuteDataSet(DbCommand cmd, Trans t) {
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            dbDataAdapter.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(DbCommand cmd, Trans t) {
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(DbHelper.dbProviderName);
            DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DbDataReader ExecuteReader(DbCommand cmd, Trans t) {
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            DbDataReader reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            return reader;
        }
        public int ExecuteNonQuery(DbCommand cmd, Trans t) {
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            int ret = cmd.ExecuteNonQuery();
            return ret;
        }

        public object ExecuteScalar(DbCommand cmd, Trans t) {
            cmd.Connection.Close();
            cmd.Connection = t.DbConnection;
            cmd.Transaction = t.DbTrans;
            object ret = cmd.ExecuteScalar();
            return ret;
        }
        #endregion
    }

    public class Trans : IDisposable {
        private DbConnection conn;
        private DbTransaction dbTrans;
        public DbConnection DbConnection {
            get { return this.conn; }
        }
        public DbTransaction DbTrans {
            get { return this.dbTrans; }
        }

        public Trans() {
            conn = DbHelper.CreateConnection();
            conn.Open();
            dbTrans = conn.BeginTransaction();
        }
        public Trans(string connectionString) {
            conn = DbHelper.CreateConnection(connectionString);
            conn.Open();
            dbTrans = conn.BeginTransaction();
        }
        public void Commit() {
            dbTrans.Commit();
            this.Colse();
        }

        public void RollBack() {
            dbTrans.Rollback();
            this.Colse();
        }

        public void Dispose() {
            this.Colse();
        }

        public void Colse() {
            if (conn.State == System.Data.ConnectionState.Open) {
                conn.Close();
            }
        }
    }
}
