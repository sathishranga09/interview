using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Runtime.InteropServices;

namespace AWEPayUserAPI.App_Code
{
    public class clsCommonBL : IDisposable
    {
        private bool disposed = false;
        private Component Components = new Component();
        private IntPtr handle;
        clsCommonDL _objDL;

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


        ~clsCommonBL()
        {

            Dispose(false);
        }
        #endregion


        #region InsertUserDtls
        public DataTable InsertUserDtls(BOCommon objBO)
        {
            using (_objDL = new clsCommonDL())
            {
                return _objDL.InsertUserDtls(objBO);
            }
        }
        #endregion


        #region UpdateUserDtls
        public DataTable UpdateUserDtls(BOCommon objBO)
        {
            using (_objDL = new clsCommonDL())
            {
                return _objDL.UpdateUserDtls(objBO);
            }
        }
        #endregion

        #region SearchUserDtls
        public DataTable SearchUserDtls(BOCommon objBO)
        {
            using (_objDL = new clsCommonDL())
            {
                return _objDL.SearchUserDtls(objBO);
            }
        }
        #endregion

        #region DeleteUserDtls
        public DataTable DeleteUserDtls(BOCommon objBO)
        {
            using (_objDL = new clsCommonDL())
            {
                return _objDL.DeleteUserDtls(objBO);
            }
        }
        #endregion
    }
}