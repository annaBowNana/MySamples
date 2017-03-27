using App.Web.Domain;
using App.Web.Models.Requests;
using App.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Services.Interfaces
{
    public interface IAdminReportsService
    {
        PaginatedItemsResponse<RegistrationReport> GetReportByDateAndWebId(PaginatedRequest model);
        string ExportCsv(PaginatedRequest model);
        bool ForRender(PaginatedRequest model);
        bool ForCsv(PaginatedRequest model);


    }
}