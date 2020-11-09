using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AWEPayUserAPI.App_Code
{
    public class clsCommonBO
    {
    }
    public class BOCommon
    {
        #region UserDtls
        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Age { get; set; }
        public string DBName { get; set; }
        #endregion
    }

}