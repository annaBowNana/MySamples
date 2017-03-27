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
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace App.Web.Controllers.Api
{
    [RoutePrefix("api/reports")]
    [Authorize(Roles = "Administrator")]

    public class AdminReportsApiController : ApiController
    {

        [Dependency]
        public IAdminReportsService _AdminReportService { get; set; }

        [Authorize] 
        [Route("getFilter"), HttpGet]
        public HttpResponseMessage GetFilteredReport([FromUri] PaginatedRequest model)
        {
            if(model.querystartdate == null)
        {
              model = new paginatedrequest();
              model.querystartdate = datetime.now.adddays(-14);
              model.queryenddate = datetime.now;


        }
            bool forRender = _AdminReportService.ForRender(model);
            bool forCsv = _AdminReportService.ForCsv(model);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);


            PaginatedItemsResponse<RegistrationReport> response = _AdminReportService.GetReportByDateAndWebId(model);
            result = Request.CreateResponse(HttpStatusCode.OK, response);

            return result;

        }
		//for downloading and exporting to excel 
        [Route("getCsv"), HttpGet]

        public HttpResponseMessage GetCsv([FromUri] PaginatedRequest model)
        {

            string exportResponse = _AdminReportService.ExportCsv(model);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new StringContent(exportResponse, Encoding.UTF8, "text/csv");
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
            result.Content.Headers.ContentDisposition.FileName = "Reports.csv";

            return result;

        }

    }
}
