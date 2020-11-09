using AWEPayUserAPI.App_Code;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWEPayUserAPI.Controllers
{
    public class UserInfoController : ApiController
    {
        clsCommonBL _objBL;
        BOCommon _objBO;
        string Json;
        object output;
        // GET api/<controller>/5
        public IHttpActionResult Get([FromBody] object UserDtls)
        {
            _objBO = new BOCommon();
            try
            {
                var jsonString = UserDtls.ToString();
                dynamic json = JValue.Parse(jsonString);
                _objBO.Email = json.Email != null ? json.Email : "";
                _objBO.Phone = json.Phone != null ? json.Phone : "";
                //if ((_objBO.Email == "") && (_objBO.Phone == ""))
                //{
                //    return Ok(JToken.Parse("{ 'Error': 'Please searchby Email or Phone' }"));
                //}
                //else
                //{
                using (_objBL = new clsCommonBL())
                {
                    Json = JsonConvert.SerializeObject(_objBL.SearchUserDtls(_objBO));
                }
                output = JToken.Parse(Json != "[]" ? Json : "{ 'Result': 'No Results' }");
                return Ok(output);
                //}
            }
            catch (Exception ex)
            {
                _objBO.Email = "";
                _objBO.Phone = "";
                using (_objBL = new clsCommonBL())
                {
                    Json = JsonConvert.SerializeObject(_objBL.SearchUserDtls(_objBO));
                }
                output = JToken.Parse(Json != "[]" ? Json : "{ 'Result': 'No Results' }");
                return Ok(output);
            }
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] object UserDtls)
        {
            try
            {
                var jsonString = UserDtls.ToString();
                dynamic json = JValue.Parse(jsonString);
                _objBO = new BOCommon();
                _objBO.FullName = json.FullName != null ? json.FullName : "";
                _objBO.Email = json.Email != null ? json.Email : "";
                _objBO.Phone = json.Phone != null ? json.Phone : "";
                _objBO.Age = json.Age != null ? json.Age : "";

                if (_objBO.FullName == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'Full Name is Required' }"));
                }
                else if (_objBO.Email == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'Email ID is Required' }"));
                }
                else
                {
                    using (_objBL = new clsCommonBL())
                    {
                        Json = JsonConvert.SerializeObject(_objBL.InsertUserDtls(_objBO));
                    }
                    output = JToken.Parse(Json != "[]" ? Json : "{ 'Error': 'Record not Inserted' }");
                    return Ok(output);
                }
            }
            catch (Exception ex)
            {
                output = JToken.Parse("{ 'Error': 'Please verify the Data' }");
                return Ok(output);
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] object UserDtls)
        {
            try
            {
                var jsonString = UserDtls.ToString();
                dynamic json = JValue.Parse(jsonString);
                _objBO = new BOCommon();
                _objBO.UserID = json.UserID != null ? json.UserID : "";
                _objBO.FullName = json.FullName != null ? json.FullName : "";
                _objBO.Email = json.Email != null ? json.Email : "";
                _objBO.Phone = json.Phone != null ? json.Phone : "";
                _objBO.Age = json.Age != null ? json.Age : "";

                if (_objBO.UserID == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'UserID is Required' }"));
                }
                else if (_objBO.FullName == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'Full Name is Required' }"));
                }
                else if (_objBO.Email == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'Email ID is Required' }"));
                }
                else
                {
                    using (_objBL = new clsCommonBL())
                    {
                        Json = JsonConvert.SerializeObject(_objBL.UpdateUserDtls(_objBO));
                    }
                    output = JToken.Parse(Json != "[]" ? Json : "{ 'Error': 'Record not Updated' }");
                    return Ok(output);
                }
            }
            catch (Exception ex)
            {
                output = JToken.Parse("{ 'Error': 'Please verify the Data' }");
                return Ok(output);
            }
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete([FromBody] object UserDtls)
        {
            try
            {
                var jsonString = UserDtls.ToString();
                dynamic json = JValue.Parse(jsonString);
                _objBO = new BOCommon();
                _objBO.UserID = json.UserID != null ? json.UserID : "";

                if (_objBO.UserID == "")
                {
                    return Ok(JToken.Parse("{ 'Error': 'User ID is Required' }"));
                }
                else
                {
                    using (_objBL = new clsCommonBL())
                    {
                        Json = JsonConvert.SerializeObject(_objBL.DeleteUserDtls(_objBO));
                    }
                    output = JToken.Parse(Json != "[]" ? Json : "{ 'Error': 'Record not Deleted' }");
                    return Ok(output);
                }
            }
            catch (Exception ex)
            {
                output = JToken.Parse("{ 'Error': 'Please verify the Data' }");
                return Ok(output);
            }
        }
    }
}