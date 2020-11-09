using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace AWEPayUserAPI.App_Code
{
    public class clsCommonDL : IDisposable
    {
        DataTable _dtMst;
        private bool disposed = false;
        private Component Components = new Component();
        private IntPtr handle;

        #region "Dispose"

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!this.disposed)
            {

                if (disposing)
                {
                    if (Components != null)
                    {
                        Components.Dispose();
                    }

                }

                CloseHandle(handle);
                handle = IntPtr.Zero;


            }
            disposed = true;
        }


        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr h);


        ~clsCommonDL()
        {

            Dispose(false);
        }
        #endregion


        #region InsertUserDtls
        public DataTable InsertUserDtls(BOCommon objBO)
        {
            _dtMst = new DataTable();
            try
            {

                using (clsDbSqlServer objdbsql = new clsDbSqlServer(clsDbSqlServer.ConnType.ActivityLog))
                {
                    string query1 = "[spInsertUser]";
                    using (SqlCommand cmd = new SqlCommand(query1))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FullName", objBO.FullName);
                        cmd.Parameters.AddWithValue("@Email", objBO.Email);
                        cmd.Parameters.AddWithValue("@Phone", objBO.Phone);
                        cmd.Parameters.AddWithValue("@Age", objBO.Age);
                        _dtMst = objdbsql.ExecuteDataTable(cmd, "InsertUserDtls");
                        return _dtMst;
                    }
                }
            }
            catch (Exception ex)
            {
                using (clsDbSqlServer objdbsql = new clsDbSqlServer())
                {
                    objdbsql.WriteMotorErrorLog("API_" + this.GetType().Name + "_" + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.StackTrace.Split(':')[ex.StackTrace.Split(':').Length - 1], ex.Message.ToString(), "", "");
                }
                return null;
            }
        }
        #endregion

        #region UpdateUserDtls
        public DataTable UpdateUserDtls(BOCommon objBO)
        {
            _dtMst = new DataTable();
            try
            {

                using (clsDbSqlServer objdbsql = new clsDbSqlServer(clsDbSqlServer.ConnType.ActivityLog))
                {
                    string query1 = "[spUpdateUser]";
                    using (SqlCommand cmd = new SqlCommand(query1))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", objBO.UserID);
                        cmd.Parameters.AddWithValue("@FullName", objBO.FullName);
                        cmd.Parameters.AddWithValue("@Email", objBO.Email);
                        cmd.Parameters.AddWithValue("@Phone", objBO.Phone);
                        cmd.Parameters.AddWithValue("@Age", objBO.Age);
                        _dtMst = objdbsql.ExecuteDataTable(cmd, "UpdateUserDtls");
                        return _dtMst;
                    }
                }
            }
            catch (Exception ex)
            {
                using (clsDbSqlServer objdbsql = new clsDbSqlServer())
                {
                    objdbsql.WriteMotorErrorLog("API_" + this.GetType().Name + "_" + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.StackTrace.Split(':')[ex.StackTrace.Split(':').Length - 1], ex.Message.ToString(), "", "");
                }
                return null;
            }
        }
        #endregion


        #region SearchUserDtls
        public DataTable SearchUserDtls(BOCommon objBO)
        {
            _dtMst = new DataTable();
            try
            {

                using (clsDbSqlServer objdbsql = new clsDbSqlServer(clsDbSqlServer.ConnType.ActivityLog))
                {
                    string query1 = "[spSearchUser]";
                    using (SqlCommand cmd = new SqlCommand(query1))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", objBO.Email);
                        cmd.Parameters.AddWithValue("@Phone", objBO.Phone);
                        _dtMst = objdbsql.ExecuteDataTable(cmd, "SearchUserDtls");
                        return _dtMst;
                    }
                }
            }
            catch (Exception ex)
            {
                using (clsDbSqlServer objdbsql = new clsDbSqlServer())
                {
                    objdbsql.WriteMotorErrorLog("API_" + this.GetType().Name + "_" + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.StackTrace.Split(':')[ex.StackTrace.Split(':').Length - 1], ex.Message.ToString(), "", "");
                }
                return null;
            }
        }
        #endregion


        #region DeleteUserDtls
        public DataTable DeleteUserDtls(BOCommon objBO)
        {
            _dtMst = new DataTable();
            try
            {

                using (clsDbSqlServer objdbsql = new clsDbSqlServer(clsDbSqlServer.ConnType.ActivityLog))
                {
                    string query1 = "[spDeleteUser]";
                    using (SqlCommand cmd = new SqlCommand(query1))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", objBO.UserID);
                        _dtMst = objdbsql.ExecuteDataTable(cmd, "DeleteUserDtls");
                        return _dtMst;
                    }
                }
            }
            catch (Exception ex)
            {
                using (clsDbSqlServer objdbsql = new clsDbSqlServer())
                {
                    objdbsql.WriteMotorErrorLog("API_" + this.GetType().Name + "_" + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.StackTrace.Split(':')[ex.StackTrace.Split(':').Length - 1], ex.Message.ToString(), "", "");
                }
                return null;
            }
        }
        #endregion

    }
}