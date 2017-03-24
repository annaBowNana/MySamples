using AnnaCodeTask.Domain;
using AnnaCodeTask.Models;
using AnnaCodeTask.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AnnaCodeTask.Controllers.Api
{
    [RoutePrefix("api/users")]
    public class UsersApiController : ApiController
    {
        [Route("get"), HttpGet]
        public HttpResponseMessage GetUsers()
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            List<UserProfile> response = UserService.GetUserInfo();
            return Request.CreateResponse(response);

        }
        [Route("insert"), HttpPost]
        public HttpResponseMessage InsertUser(InsertUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            List<UserProfile> response = new List<UserProfile>();
            
             UserService list = new UserService();
            // response = list.InsertUserProfile(model);

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

    }
}
