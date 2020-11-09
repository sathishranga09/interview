/*System Namespaces*/
#region namespaces
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;

#endregion

namespace AWEPayUserAPI.App_Code
{

    public class clsDbSqlServer : IDisposable
    {
        private Component Components = new Component();
        // Track whether Dispose has been called.
        private bool disposed = false;
        // Pointer to an external unmanaged resource.
        private IntPtr handle;
        #region "Global Variables"

        /**************************************************************
     ****** Summary : Sql connection string.                 ****** 
     *************************************************************/

        private string _strSqlConnection = string.Empty;

        /**************************************************************
         ****** Summary : Errorlog Path.                         ****** 
         *************************************************************/

        private string _strErrorLogPath = string.Empty;

        /**************************************************************
         ****** Summary :  Track current userid                  ****** 
         *************************************************************/

        private string _strUserID = string.Empty;


        /**************************************************************
        ****** Summary :  SQLConnection object                  ****** 
        *************************************************************/
        private SqlConnection _objSQLConnection = null;


        /**************************************************************
         ****** Summary :  Input Sql Parameter Array             ****** 
         *************************************************************/
        private SqlParameter[] _objInput_parameters = { };

        /**************************************************************
         ****** Summary :   Output Sql Parameter                 ****** 
         *************************************************************/
        public SqlParameter[] _objOutput_Parameters = { };

        /**************************************************************
         ****** Summary :    Sql Parameter return value          ****** 
         *************************************************************/
        public SqlParameter _objReturnParameter;

        /**************************************************************
         ****** Summary :    SqlParameter Object                 ****** 
         *************************************************************/
        public SqlParameter _objSQLParameter;
        /// <summary>
        /// Input Sql Parameter Array
        /// </summary>
        /// Private Input_parameters(0) As SqlParameter
        private SqlParameter[] Input_parameters = { };
        /// <summary>
        /// Output Sql Parameter
        /// </summary>
        public SqlParameter[] Output_Parameters = { };

        //public clsCommon _objClsCommon;
        #endregion

        public enum ConnType
        {
            ActivityLog
        }

        public clsDbSqlServer([Optional] ConnType ct)
        {
            switch (ct)
            {
                case ConnType.ActivityLog:
                    _strSqlConnection = ConfigurationManager.ConnectionStrings["ActivityConnection"].ConnectionString;
                    break;
                //default:
                //    _strSqlConnection = ConfigurationManager.ConnectionStrings["ExConnection"].ConnectionString;
                //  break;
            }
        }

        #region Open Connection
        private Boolean _blnfnOpenConnection()
        {
            try
            {
                using (_objSQLConnection = new SqlConnection())
                {

                    if (_objSQLConnection.State != ConnectionState.Open)
                    {
                        _objSQLConnection.ConnectionString = _strSqlConnection;
                        _objSQLConnection.Open();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SqlException dbEx)
            {

                fnLogErrorInEvent("clsDbSqlServer_fnOpenConnection", dbEx.Message, dbEx.Source);
                return false;
            }
            catch (Exception ex)
            {
                fnLogError("clsDbSqlServer_fnOpenConnection", ex.Message, "");
                return false;
            }
            finally
            {

            }
        }
        #endregion

        #region close connection
        private Boolean _blnfnCloseConnection()
        {
            try
            {
                //  _objSQLConnection = new SqlConnection();
                if (_objSQLConnection.State != ConnectionState.Closed)
                {
                    _objSQLConnection.Close();
                    _objSQLConnection.Dispose();
                    return true;
                }
                else if (_objSQLConnection.State == ConnectionState.Closed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException dbEx)
            {
                fnLogErrorInEvent("clsDbSqlServer_fnCloseConnection", dbEx.Message, dbEx.Source);
                return false;
            }
            catch (Exception ex)
            {
                fnLogError("clsDbSqlServer_fnCloseConnection", ex.Message, "");
                return false;
            }

        }
        #endregion

        #region "Add Input Parameter"
        /// <summary>
        /// To add Input Parameter
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="objParamValue">Parameter Value</param>
        public void fnAddInputParam(string paramName, object objParamValue)
        {
            try
            {
                _objSQLParameter = new SqlParameter(paramName, objParamValue);
                Array.Resize(ref Input_parameters, Input_parameters.Length + 1);
                Input_parameters[Input_parameters.Length - 1] = _objSQLParameter;
            }
            catch (SqlException dbEx)
            {
                fnLogErrorInEvent("clsDbSqlServer_fnAddInputParam", dbEx.Message, dbEx.Message);
            }
            catch (Exception ex)
            {
                fnLogError("clsDbSqlServer_fnAddInputParam", ex.Message, "");
            }
            finally
            {
                _objSQLParameter = null;
            }

        }

        #endregion

        #region "Add Output Parameter"
        /// <summary>
        /// To add Output Parameter
        /// </summary>
        /// <param name="paramName">Parameter Name</param>
        /// <param name="sqlDataType">Parameter Type</param>
        /// <param name="size">Parameter Size</param>
        public void fnAddOutputParam(string paramName, SqlDbType sqlDataType, int size = 0)
        {
            try
            {
                _objSQLParameter = new SqlParameter(paramName, sqlDataType);
                _objSQLParameter.Direction = ParameterDirection.Output;

                if (size > 0) //Default value is zero
                {
                    _objSQLParameter.Size = size;
                }

                Array.Resize(ref Output_Parameters, Output_Parameters.Length + 1);
                Output_Parameters[Output_Parameters.Length - 1] = _objSQLParameter;

            }
            catch (SqlException dbEx)
            {
                fnLogErrorInEvent("clsDbSqlServer_fnAddOutputParam", dbEx.Message, dbEx.Message);
            }
            catch (Exception ex)
            {
                fnLogError("clsDbSqlServer_fnAddOutputParam", ex.Message, "");
            }
            finally
            {
                _objSQLParameter = null;
            }

        }

        #endregion

        #region Execute Dataset Without passing parameters
        public DataSet GetData(SqlCommand cmd, int pageIndex = 0, string FunctionName = "")
        {
            DataSet ds1 = null;
            DataTable dtErrror = null;
            //SqlCommand cmdDbLog = null;

            try
            {
                using (SqlConnection con = new SqlConnection(_strSqlConnection))
                {
                    ds1 = new DataSet();
                    dtErrror = new DataTable();
                    dtErrror.TableName = "tblDbErrorLog";
                    dtErrror.Columns.Add("SystemError", typeof(string));
                    //End code .

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        //long startSQLTime = System.DateTime.Now.Ticks;
                        //long endSQLTime;
                        //long SQLtimeTaken;

                        cmd.Connection = con;
                        cmd.CommandTimeout = 0;
                        sda.SelectCommand = cmd;

                        using (DataSet ds = new DataSet())
                        {
                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }
                            sda.Fill(ds, "Saved");


                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrorCode = fnLogError(cmd.CommandText + "-" + FunctionName, ex.Message, "");
                dtErrror.Rows.Add(strErrorCode);
                ds1.Tables.Add(dtErrror);
                return ds1;
            }
            finally
            {
                if (ds1 != null)
                {
                    ds1.Dispose();
                }
                if (dtErrror != null)
                {
                    dtErrror.Dispose();
                }
            }
        }

        public DataSet fnExecuteDataset(string spName)
        {
            try
            {
                //    _objSQLConnection = new SqlConnection();
                if (spName != "")
                {
                    //if (fnOpenConnection())
                    //{
                    using (SqlConnection con = new SqlConnection(_strSqlConnection))
                    {
                        using (SqlCommand cmd = new SqlCommand(spName, con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0; // This is to MAke Sure There is never a SQLConnection Timeout
                            if (Input_parameters.Length > 0)
                            {
                                //_objSQLCommand.Parameters.AddRange(Input_parameters);
                                foreach (SqlParameter Param in Input_parameters)
                                {
                                    cmd.Parameters.Add(Param);
                                }
                            }
                            if (Output_Parameters.Length > 0)
                            {
                                //_objSQLCommand.Parameters.AddRange(Output_Parameters);
                                foreach (SqlParameter OutParam in Output_Parameters)
                                {
                                    cmd.Parameters.Add(OutParam);
                                }
                            }
                            using (DataSet ds = new DataSet())
                            {
                                using (SqlDataAdapter da = new SqlDataAdapter())
                                {
                                    if (con.State != ConnectionState.Open)
                                    {
                                        con.Open();
                                    }
                                    long startSQLTime = System.DateTime.Now.Ticks;
                                    long endSQLTime;
                                    long SQLtimeTaken;
                                    da.SelectCommand = cmd;
                                    da.Fill(ds);
                                    //Code for SQL Logger
                                    endSQLTime = DateTime.Now.Ticks;
                                    SQLtimeTaken = (endSQLTime - startSQLTime) / 10000000;

                                }
                                return ds;

                            }
                        }
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (SqlException dbEx)
            {
                fnLogErrorInEvent("clsDbSqlServer_fnExecuteStoredProcedure", dbEx.Message + " - SP Name : " + spName, dbEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                fnLogError("clsDbSqlServer_fnExecuteStoredProcedure", ex.Message, "");
                return null;
            }
            finally
            {

            }
        }
        #endregion

        public DataSet ExecuteDataset(SqlCommand cmd, string FunctionName = "")
        {
            DataSet ds1 = null;
            DataTable dtErrror = null;

            dtErrror = new DataTable();
            dtErrror.TableName = "tblDbErrorLog";
            dtErrror.Columns.Add("SystemError", typeof(string));

            try
            {
                using (SqlConnection con = new SqlConnection(_strSqlConnection))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        cmd.CommandTimeout = 0;
                        sda.SelectCommand = cmd;

                        using (DataSet ds = new DataSet())
                        {
                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }
                            sda.Fill(ds, "Saved");

                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrorCode = fnLogError(cmd.CommandText + "-" + FunctionName, ex.Message, "");
                dtErrror.Rows.Add(strErrorCode);
                ds1.Tables.Add(dtErrror);
                return ds1;
            }
            finally
            {
                if (ds1 != null)
                {
                    ds1.Dispose();
                }
                if (dtErrror != null)
                {
                    dtErrror.Dispose();
                }
            }
        }


        public DataTable ExecuteDataTable(SqlCommand cmd, string FunctionName = "")
        {
            DataTable ds1 = null;
            DataTable dtErrror = null;

            dtErrror = new DataTable();
            dtErrror.TableName = "tblDbErrorLog";
            dtErrror.Columns.Add("SystemError", typeof(string));

            try
            {
                using (SqlConnection con = new SqlConnection(_strSqlConnection))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        cmd.CommandTimeout = 0;
                        sda.SelectCommand = cmd;

                        using (DataTable ds = new DataTable())
                        {
                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }
                            sda.Fill(ds);

                            return ds;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrorCode = fnLogError(cmd.CommandText + "-" + FunctionName, ex.Message, "");
                dtErrror.Rows.Add(strErrorCode);
                return dtErrror;
            }
            finally
            {
                if (ds1 != null)
                {
                    ds1.Dispose();
                }
                if (dtErrror != null)
                {
                    dtErrror.Dispose();
                }
            }
        }

        public string ExecuteScalar(SqlCommand cmd, string FunctionName = "")
        {
            string result = "";
            DataSet ds1 = null;
            DataTable dtErrror = null;
            dtErrror = new DataTable();
            dtErrror.TableName = "tblDbErrorLog";
            dtErrror.Columns.Add("SystemError", typeof(string));
            try
            {
                using (SqlConnection con = new SqlConnection(_strSqlConnection))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        cmd.CommandTimeout = 0;
                        sda.SelectCommand = cmd;

                        using (DataSet ds = new DataSet())
                        {
                            if (con.State != ConnectionState.Open)
                            {
                                con.Open();
                            }

                            object strObject = cmd.ExecuteScalar();
                            if (strObject != null)
                            {
                                result = strObject.ToString();
                            }
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strErrorCode = fnLogError(cmd.CommandText + "-" + FunctionName, ex.Message, "");
                dtErrror.Rows.Add(strErrorCode);
                ds1.Tables.Add(dtErrror);
                return result;
            }
            finally
            {
                if (ds1 != null)
                {
                    ds1.Dispose();
                }
                if (dtErrror != null)
                {
                    dtErrror.Dispose();
                }
            }
        }

        #region Log Error Functions
        public string fnLogError(string Source, string errDescription, string UserID)
        {
            try
            {
                string MainQuery = String.Empty;

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ExServiceConnection"].ConnectionString))
                {
                    errDescription = errDescription.Replace("'", "");

                    MainQuery = "SpUpdErrorLog";

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Parameters.AddWithValue("@Location", Source);
                        cmd.Parameters.AddWithValue("@ErrorDesc", errDescription);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Connection = con;
                        cmd.CommandText = MainQuery;

                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            sda.SelectCommand = cmd;
                            using (DataSet ds = new DataSet())
                            {
                                if (con.State != ConnectionState.Open)
                                {
                                    con.Open();
                                }
                                sda.Fill(ds, "tblDbErrorLog");
                                if (ds != null ? ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["Error"].ToString() != "" : false : false : false)
                                {
                                    return ds.Tables[0].Rows[0]["Error"].ToString();
                                }

                            }
                        }
                    }

                }
                return "System error please contact IT support. ";

            }
            catch (SqlException dbEx)
            {

                fnLogErrorInEvent(Source, errDescription + " - " + dbEx.Message, UserID);
                return "System error please contact IT support.";
            }

            finally
            {

            }

        }
        public Boolean fnLogErrorInEvent(string Source, string errDescription, string SQLQuery)
        {


            try
            {
                if (!EventLog.SourceExists(Source))
                    EventLog.CreateEventSource(Source, "ExcelratorLog");// Source is error location
                EventLog.WriteEntry(Source, "Error:'" + errDescription + "',SQLQUERY:'" + SQLQuery + "'");
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region Function Calling for Errorlog in Jquery
        public string FnErrorLog(string Location, string ErrorDesc, string UserID)
        {
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                using (clsDbSqlServer objdbsql = new clsDbSqlServer())
                {
                    string query = "[SpUpdErrorLog]";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Location", Location);
                        cmd.Parameters.AddWithValue("@ErrorDesc", ErrorDesc.Replace("'", "").Replace(@"""", ""));
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        ds = objdbsql.GetData(cmd, 1);
                        if (ds != null ? ds.Tables.Count > 0 ? ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["Error"].ToString() != "" : false : false : false)
                        {
                            return ds.Tables[0].Rows[0]["Error"].ToString();
                        }
                        else
                        {
                            return "Error : System error please contact IT support ";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
            }
        }
        #endregion

        #region Function Calling for Errorlog
        public void WriteMotorErrorLog(string Module_MethodName, string strLineNo, string ErrorMsg, string CreateBy, string strQuoID = "")
        {
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                using (clsDbSqlServer objdbsql = new clsDbSqlServer(clsDbSqlServer.ConnType.ActivityLog))
                {
                    string query = "[spUpd_ErrorLog]";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ip_ModuleName", Module_MethodName);
                        cmd.Parameters.AddWithValue("@ip_ErrorDesc", ErrorMsg);
                        cmd.Parameters.AddWithValue("@ip_CreateBy", CreateBy);
                        cmd.Parameters.AddWithValue("@ip_ErrorLine", strLineNo);
                        cmd.Parameters.AddWithValue("@ip_QuoID", strQuoID);
                        ds = objdbsql.GetData(cmd, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
            }
        }
        #endregion

        #region "Dispose"
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    if (Components != null)
                    {
                        // Dispose managed resources.
                        Components.Dispose();
                    }

                }

                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;
        }

        // This is a platform invoke prototype. SetLastError is true, which allows 
        // the GetLastWin32Error method of the Marshal class to work correctly.    
        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr h);


        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~clsDbSqlServer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

    }

}