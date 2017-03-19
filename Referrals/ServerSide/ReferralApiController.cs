using Microsoft.Practices.Unity;
using App.Web.Domain;
using App.Web.Models.Requests;
using App.Web.Models.Responses;
using App.Web.Services;
using App.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App.Web.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/referral")]
    public class ReferralApiController : ApiController
    {
        //Route to get referral from the current user

        [Dependency]
        public IReferralService _ReferralService { get; set; }

        [Route("post"), HttpPost]
        public HttpResponseMessage APIPostReferral(ReferralCodeRequest model)
        {
            // if the Model does not pass validation, there will be an Error response returned with errors
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string SenderUserId = UserService.GetCurrentUserId();
            _ReferralService.SendReferrals(SenderUserId, model);

            ItemResponse<bool> response = new ItemResponse<bool>();
            response.Item = true;
            return Request.CreateResponse(response);

        }

        [Route("get") HttpGet]
        public HttpResponseMessage getAllReferrals()
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemsResponse<Token> response = new ItemsResponse<Token>();
            List<Token> list = TokenService.getAllToken();
            response.Items = TokenService.getAllToken();
            return Request.CreateResponse(response);

        }
    

        [Route("getUserId")]
        public HttpResponseMessage getReferralsByUserIdAndCouponType()
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            ReferralTokenRequest referralTokenModel = new ReferralTokenRequest();

            ItemsResponse<Token> response = new ItemsResponse<Token>();
            string UserId = UserService.GetCurrentUserId();
            response.Items = TokenService.getTokenByUserIdAndTokenType(UserId);
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        //Used for the paginating data 
        [Route("getPag"), HttpGet]
        public HttpResponseMessage GetAllReferralPagination([FromUri] PaginatedRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            string UserId = UserService.GetCurrentUserId();
            PaginatedItemsResponse<Token> response = TokenService.getTokenByUserIdAndTokenTypePagination(UserId, model);
            response.CurrentPage = model.CurrentPage;
            response.ItemsPerPage = model.ItemsPerPage;

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("delete/{id:int}") HttpDelete]
        public HttpResponseMessage deleteReferral(int Id)
        {
            ItemResponse<int> response = new ItemResponse<int>();
            WebsiteSettingsServices.Delete(Id);
            return Request.CreateResponse(response);
        }
    }

}

