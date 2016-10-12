using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Logging;

namespace DatabaseHandler.Helpers
{
    public class StoredProcedureExecutor
    {
        /// <summary>
        ///     Executes a batch of stored procedures as a transaction against the SQL server
        /// </summary>
        /// <param name="storedProcedures"></param>
        public static bool ExecuteNoQueryAsTransaction(IEnumerable<StoredProcedureBase> storedProcedures)
        {
            var result = true;
            var currentConnection = DatabaseConnection.NewConnection;
            SqlTransaction transaction = null;
            if (storedProcedures == null || currentConnection == null)
            {
                return true;
            }
            transaction = currentConnection.BeginTransaction();
            try
            {
                foreach (var sp in storedProcedures)
                {
                    OperationStatus status = null;

                    var command = new SqlCommand(sp.StoredProcedure.ToString(), currentConnection)
                    {
                        Transaction = transaction,
                        CommandType = CommandType.StoredProcedure
                    };

                    var sqlParameters = SetSqlParameters(sp.Parameters);
                    if (sqlParameters != null)
                    {
                        command.Parameters.AddRange(sqlParameters);
                    }

                    AddStatusOutputParameters(command);

                    // maybe this should not have a retry policy
                    // think of a scenario with 30 procedures in a transaction, each failing 
                    // 30 x #of retries x #seconds between retries = not good experience
                    command.ExecuteNonQuery();


                    status = ExtractStatusOutputParameters(command);

                    if (status.ErrorCode != 0)
                    {
                        throw new Exception("Could not execute one or more commands inside the transaction.");
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                LogHelper.LogException<StoredProcedureExecutor>(ex,
                                                                "An error occurred when executing the stored procedures transaction.");
                transaction.Rollback();
                result = false;
            }
            finally
            {
                currentConnection.Close();
                currentConnection.Dispose();
            }
            return result;
        }

        private static DataTable ExecuteStoredProcedure(StoredProcedureBase sp, out OperationStatus or)
        {
            DataTable result = null;
            or = null;
            var currentConnection = DatabaseConnection.NewConnection;
            if (sp == null || currentConnection == null)
            {
                return null;
            }
            var cmd = new SqlCommand(sp.StoredProcedure.ToString(), currentConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var sqlParameters = SetSqlParameters(sp.Parameters);
            if (sqlParameters != null)
            {
                cmd.Parameters.AddRange(sqlParameters);
            }

            AddStatusOutputParameters(cmd); // add the status output parameters

            var dataAdapter = new SqlDataAdapter(cmd);
            var dataTable = new DataTable();

            try
            {
                var errorOccured = false;
                try
                {
                    // this needs to use the retry policy
                    dataAdapter?.Fill(dataTable);
                    errorOccured = false;
                }
                catch (Exception ex)
                {
                    LogHelper.LogException<StoredProcedureExecutor>(ex);
                    var exceptionMessage = ex.ToString();
                    errorOccured = true;
                }
                if (errorOccured)
                {
                    or = new OperationStatus() { Error = true };
                }
                else
                {
                    if (!dataTable.HasErrors && dataTable.Rows != null)
                    {
                        result = dataTable;
                        or = ExtractStatusOutputParameters(cmd);
                    }
                    else
                    {
                        LogHelper.LogInfo<StoredProcedureExecutor>("No data was found for given stored procedure");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException<StoredProcedureExecutor>(ex, "An error occurred when executing the stored procedure");
                dataTable.Dispose();
            }
            finally
            {
                currentConnection.Close();
                currentConnection.Dispose();
                dataAdapter.Dispose();
                cmd.Dispose();
            }
            return result;
        }

        private static OperationStatus ExtractStatusOutputParameters(SqlCommand cmd)
        {
            OperationStatus result = null;
            if (cmd == null)
            {
                return null;
            }
            if (cmd.Parameters.Count <= 0)
            {
                return null;
            }
            if (cmd.Parameters.Contains(OutputParametersConstants.Error))
            {
                result = new OperationStatus();
                if (cmd.Parameters[OutputParametersConstants.Error].Value != DBNull.Value)
                {
                    var errorParam = cmd.Parameters[OutputParametersConstants.Error];
                    if (errorParam?.Value is bool)
                    {
                        result.Error = (bool)errorParam.Value;
                    }
                    else
                    {
                        result.Error = true;
                    }
                }
            }
            if (cmd.Parameters.Contains(OutputParametersConstants.ErrorCode))
            {
                if (result == null)
                {
                    result = new OperationStatus();
                }
                if (cmd.Parameters[OutputParametersConstants.ErrorCode].Value != DBNull.Value)
                {
                    var errorCodeParam = cmd.Parameters[OutputParametersConstants.ErrorCode];
                    if (errorCodeParam?.Value is int)
                    {
                        result.ErrorCode = (int)errorCodeParam.Value;
                    }
                    else
                    {
                        result.ErrorCode = -70;
                    }
                }
            }
            if (!cmd.Parameters.Contains(OutputParametersConstants.ReturnValue))
            {
                return result;
            }
            if (result == null)
            {
                result = new OperationStatus();
            }
            if (cmd.Parameters[OutputParametersConstants.ReturnValue].Value == DBNull.Value)
            {
                return result;
            }
            var returnValueParam = cmd.Parameters[OutputParametersConstants.ReturnValue];
            if (returnValueParam?.Value is int)
            {
                result.ReturnValue = returnValueParam.Value;
            }
            return result;
        }

        private static void AddStatusOutputParameters(SqlCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }
            var errorFlag = new SqlParameter(OutputParametersConstants.Error, SqlDbType.Bit, sizeof(bool));
            var errorCode = new SqlParameter(OutputParametersConstants.ErrorCode, SqlDbType.Int, sizeof(int));

            var returnValue = new SqlParameter
            {
                ParameterName = OutputParametersConstants.ReturnValue,
                Direction = ParameterDirection.ReturnValue
            };

            errorFlag.Direction = ParameterDirection.Output;
            errorCode.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(errorFlag);
            cmd.Parameters.Add(errorCode);
            cmd.Parameters.Add(returnValue);
        }

        public void ExecuteNoReturnProcedure(StoredProcedureBase sProc, out OperationStatus or)
        {
            or = null;
            if (sProc != null)
            {
                var dTable = ExecuteStoredProcedure(sProc, out or);
                dTable?.Dispose();
            }
            else
            {
                LogHelper.LogException<StoredProcedureExecutor>("Provided StoredProcedureBase is null");
            }
        }

        public static T GetSingleSetResult<T>(StoredProcedureBase sProc, out OperationStatus or) where T : class, new()
        {
            T result = default(T); //null;
            or = null;
            if (sProc == null)
            {
                LogHelper.LogException<StoredProcedureExecutor>("Provided StoredProcedureBase is null");
            }
            else
            {
                var dTable = ExecuteStoredProcedure(sProc, out or);
                try
                {
                    if (dTable?.Rows == null || dTable.Rows.Count <= 0)
                    {
                        return null;
                    }
                    result = Activator.CreateInstance<T>();
                    var properties = result.GetType().GetProperties();
                    object value;
                    for (var i = 0; i < properties.Count(); i++)
                    {
                        if (!dTable.Columns.Contains(properties[i].Name))
                        {
                            continue;
                        }
                        value = dTable.Rows[0][properties[i].Name];
#if DEBUG
                        Debug.WriteLine($"property is {properties[i].Name}");
#endif
                        if (value is DBNull)
                        {
                            properties[i].SetValue(result, null, null);
                        }
                        else
                        {
                            properties[i].SetValue(result, value, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogException<StoredProcedureExecutor>(ex);
                }
                finally
                {
                    dTable?.Dispose();
                }
            }
            return result;
        }

        public static List<T> GetMultipleSetResult<T>(StoredProcedureBase sProc, out OperationStatus or) where T : class, new()
        {
            List<T> result = null;
            if (sProc == null)
            {
                LogHelper.LogException<StoredProcedureExecutor>("Provided StoredProcedureBase is null");
            }
            var dTable = ExecuteStoredProcedure(sProc, out or);
            try
            {
                var rowCount = dTable.Rows.Count;
                if (dTable.Rows == null || rowCount <= 0)
                {
                    return null;
                }
                result = new List<T>();
                var properties = Activator.CreateInstance<T>().GetType().GetProperties();
                for (var count = 0; count < rowCount; count++)
                {
                    var item = Activator.CreateInstance<T>(); //null;
                    object value;
                    for (int i = 0; i < properties.Count(); i++)
                    {
                        if (!dTable.Columns.Contains(properties[i].Name))
                        {
                            continue;
                        }

#if DEBUG
                        Debug.WriteLine($"property is {properties[i].Name}");
#endif
                        value = dTable.Rows[count][properties[i].Name];
                        if (value is DBNull)
                        {
                            properties[i].SetValue(item, null, null);
                        }
                        else
                        {
                            properties[i].SetValue(item, value, null);
                        }
                    }
                    result.Add(item);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogException<StoredProcedureExecutor>(ex);
            }
            finally
            {
                dTable?.Dispose();
            }
            return result;
        }

        private static SqlParameter[] SetSqlParameters(Dictionary<string, object> parameters)
        {
            SqlParameter[] sqlParameters = null;
            if (parameters != null && parameters.Count > 0)
            {
                sqlParameters = parameters.Select(p => new SqlParameter(p.Key, p.Value)).ToArray();
            }
            //TODO: de vazut daca verificarea de null si setarea cu DBNull nu poate fi facuta in parameters.Select-ul de mai sus. Ar trebui sa se poata daca in predicat sunt mai multe cai de return pentru un nou parametru SQL
            if (sqlParameters == null)
            {
                return null;
            }
            foreach (var p in sqlParameters)
            {
                if (p.Value != null)
                {
                    continue;
                }
                p.IsNullable = true;
                p.Value = DBNull.Value;
            }
            return sqlParameters;
        }
    }
}