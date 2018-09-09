using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzrealBLL.DBHelper
{
    public sealed partial class MySqlHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 批量操作批次数
        /// </summary>
        private static int BatchSize = 5000;

        /// <summary>
        /// 数据库连接超时时间
        /// </summary>
        private static int CommandTimeOut = 500;

        public MySqlHelper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        #region ExecuteNonQuery 返回影响的行数
        /// <summary>
        ///  执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText,params MySqlParameter[] parms)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
            return ExecuteNonQuery(conn,null, CommandType.Text, commandText, parms);
            }
        }

        /// <summary>
        ///  执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            using(MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                return ExecuteNonQuery(conn,null, commandType, commandText, param);
            }
        }

        public int ExecuteNonQuery(MySqlTransaction transaction,CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            return ExecuteNonQuery(transaction.Connection, transaction, commandType, commandText, param);
        }

        /// <summary>
        /// 执行SQL，返回受影响的行数
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType">命令类型(存储过程,命令文本,, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="param"></param>
        /// <returns></returns>
        private static int ExecuteNonQuery(MySqlConnection conn, MySqlTransaction transaction,CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            MySqlCommand cmd = new MySqlCommand();
            PretreatmentCommand(conn, cmd, transaction, commandType, commandText, param);
            int result = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return result;
        }
        #endregion

        #region ExecuteScalar 返回结果集中的第一行第一列
        public T ExecuteScalar<T>(string commandText, params MySqlParameter[] param)
        {
            object result = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                result = ExecuteScalar(connection, null, CommandType.Text, commandText, param);
            }
            if (result != null)
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default(T);
        }

        public T ExecuteScalar<T>(CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            object result = null;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                result = ExecuteScalar(connection, null, commandType, commandText, param);
            }
            if (result != null)
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default(T);
        }

        public T ExecuteScalar<T>(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            object result = ExecuteScalar(transaction.Connection, transaction, commandType, commandText, param);
            if (result != null)
            {
                return (T)Convert.ChangeType(result, typeof(T));
            }
            return default(T);
        }

        private static object ExecuteScalar(MySqlConnection conn,MySqlTransaction transaction,CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            MySqlCommand command = new MySqlCommand();
            PretreatmentCommand(conn, command, transaction, commandType, commandText, param);
            object result = command.ExecuteScalar();
            command.Parameters.Clear();
            return result;
        }
        #endregion

        #region ExecuteDataReader  返回只读数据集DataReader
        public MySqlDataReader ExecuteDataReader(string commandText,params MySqlParameter[] param)
        {
            using(MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                return ExecuteDataReader(conn, null, CommandType.Text, commandText, param);
            }
        }

        public MySqlDataReader ExecuteDataReader(CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                return ExecuteDataReader(conn, null, commandType, commandText, param);
            }
        }

        public MySqlDataReader ExecuteDataReader(MySqlTransaction transaction, CommandType commandType,string commandText, params MySqlParameter[] param)
        {
            return ExecuteDataReader(transaction.Connection, transaction, CommandType.Text, commandText, param);
        }

        private static MySqlDataReader ExecuteDataReader(MySqlConnection conn,MySqlTransaction transaction,CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            MySqlCommand command = new MySqlCommand();
            PretreatmentCommand(conn, command, transaction, commandType, commandText, param);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        #endregion

        #region ExecuteDataTable 返回结果集中的第一个数据表
        public DataTable ExecuteDataTable(string commandText,params MySqlParameter[] param)
        {
            return ExecuteDataSet(ConnectionString, CommandType.Text, commandText, param).Tables[0];
        }

        public DataTable ExecuteDataTable(CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            return ExecuteDataSet(ConnectionString, commandType, commandText, param).Tables[0];
        }

        public DataTable ExecuteDataTable(MySqlTransaction transaction,CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            return ExecuteDataSet(transaction,commandType, commandText, param).Tables[0];
        }
        #endregion

        #region  ExecuteDataRow 执行SQL语句,返回结果集中的第一行
        public DataRow ExecuteDataRow(string commandText,params MySqlParameter[] param)
        {
            DataTable dt = ExecuteDataTable(commandText, param);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataRow ExecuteDataRow(CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            DataTable dt = ExecuteDataTable(commandType,commandText, param);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public DataRow ExecuteDataRow(MySqlTransaction transaction, CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            DataTable dt = ExecuteDataTable(transaction,commandType, commandText, param);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        #endregion

        #region ExecuteDataSet 返回结果集
        public static DataSet ExecuteDataSet(string conn,string commandText,params MySqlParameter[] param)
        {
            using(MySqlConnection connection = new MySqlConnection(conn))
            {
                return ExecuteDataSet(connection, null, CommandType.Text, commandText, param);
            }
        }

        public static DataSet ExecuteDataSet(string conn, CommandType commandType, string commandText, params MySqlParameter[] param)
        {
            using (MySqlConnection connection = new MySqlConnection(conn))
            {
                return ExecuteDataSet(connection, null, commandType, commandText, param);
            }
        }

        public static DataSet ExecuteDataSet(MySqlTransaction transaction,CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            return ExecuteDataSet(transaction.Connection, transaction, commandType, commandText, param);
        }

        private static DataSet ExecuteDataSet(MySqlConnection conn,MySqlTransaction transaction,CommandType commandType,string commandText,params MySqlParameter[] param)
        {
            MySqlCommand cmd = new MySqlCommand();
            PretreatmentCommand(conn, cmd, transaction, commandType, commandText, param);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            adapter.Fill(ds);

            if (commandText.IndexOf("@") > 0)
            {
                commandText = commandText.ToLower();
                int index = commandText.IndexOf("where ");
                if (index < 0)
                {
                    index = commandText.IndexOf("\nwhere");
                }
                if (index > 0)
                {
                    ds.ExtendedProperties.Add("SQL", commandText.Substring(0, index - 1));
                }
                else
                {
                    ds.ExtendedProperties.Add("SQL", commandText);
                }
            }
            else
            {
                ds.ExtendedProperties.Add("SQL", commandText);
            }

            foreach(DataTable dt in ds.Tables)
            {
                dt.ExtendedProperties.Add("SQL", ds.ExtendedProperties["SQL"]);
            }

            cmd.Parameters.Clear();
           return ds;
        }
        #endregion

        #region 批量插入
        public void BatchInsert(List<string> sql,List<MySqlParameter[]> listparam)
        {
            BatchInsert(ConnectionString, sql, listparam);
        }
        #endregion

        #region 批量更新
        public void BatchUpdate(DataTable dt)
        {
            BatchUpdate(ConnectionString,dt);
        }
        #endregion

        /// <summary>
        /// 批量插入实例方法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="listsql"></param>
        /// <param name="listparam"></param>
        private static void BatchInsert(string conn,List<string> listsql,List<MySqlParameter[]> listparam)
        {
            MySqlConnection connect = new MySqlConnection(conn);
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandTimeout = CommandTimeOut;
            cmd.CommandType = CommandType.Text;

            MySqlTransaction transaction = null;
            try
            {
                connect.Open();
                transaction = connect.BeginTransaction();
                cmd.Transaction = transaction;
                for(int i = 0; i < listsql.Count; i++)
                {
                    cmd.CommandText = listsql[i];
                    cmd.Parameters.AddRange(listparam[i]);
                    cmd.ExecuteNonQuery();
                }
                transaction.Commit();
            }catch(Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw ex;
            }
            finally
            {
                connect.Close();
                connect.Dispose();
            }
        }

        /// <summary>
        /// 批量更新实例方法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        private static void BatchUpdate(string conn,DataTable dt)
        {
            MySqlConnection connection = new MySqlConnection(conn);
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandTimeout = CommandTimeOut;
            cmd.CommandType = CommandType.Text;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter);
            cb.ConflictOption = ConflictOption.OverwriteChanges;

            MySqlTransaction transaction = null;
            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();
                adapter.UpdateBatchSize = BatchSize;
                adapter.SelectCommand.Transaction = transaction;

                if(dt.ExtendedProperties["SQL"]!= null)
                {
                    adapter.SelectCommand.CommandText = dt.ExtendedProperties["SQL"].ToString();
                }
                adapter.Update(dt);
                transaction.Commit();
            }catch(Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                throw ex;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /// <summary>
        /// 预处理MySQL操作命令
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="command"></param>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="param"></param>
        private static void PretreatmentCommand(MySqlConnection conn, MySqlCommand command,MySqlTransaction transaction,CommandType commandType,string commandText,MySqlParameter[] param)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            command.Connection = conn;
            command.CommandTimeout = CommandTimeOut;
            command.CommandText = commandText;

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            command.CommandType = commandType;
            if(param != null && param.Length > 0)
            {
                //预处理输入参数，将参数为null的赋值为 DBNull.Value
                foreach (MySqlParameter item in param)
                {
                    if((item.Direction == ParameterDirection.InputOutput || item.Direction == ParameterDirection.Input) && (item.Value == null))
                    {
                        item.Value = DBNull.Value;
                    }
                }
                command.Parameters.AddRange(param);
            }
        }
    }
}
