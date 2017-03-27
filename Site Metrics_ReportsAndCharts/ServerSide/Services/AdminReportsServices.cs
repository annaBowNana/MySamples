using Microsoft.Practices.Unity;
using App.Data;
using App.Web.Domain;
using App.Web.Models.Requests;
using App.Web.Models.Responses;
using App.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace App.Web.Services
{
    public class AdminReportsService : BaseService, IAdminReportsService
        
    {
        public PaginatedItemsResponse<RegistrationReport> GetReportByDateAndWebId(PaginatedRequest model)
        {
            List<RegistrationReport> reportList = null;
            PaginatedItemsResponse<RegistrationReport> response = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Reports_UserRegistrationReport_GetByDateAndWebId_v2"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                 {
                     paramCollection.AddWithValue("@QueryWebsiteId", model.QueryWebsiteId);
                     paramCollection.AddWithValue("@QueryStartDate", model.QueryStartDate);
                     paramCollection.AddWithValue("@QueryEndDate", model.QueryEndDate);

                 }, map: delegate (IDataReader reader, short set)
                 {
                     if (set == 0)

                     {
                         RegistrationReport report = new RegistrationReport();
                         int startingIndex = 0;

                         report.Date = reader.GetSafeDateTime(startingIndex++);
                         report.WebsiteId = reader.GetSafeInt32(startingIndex++);
                         report.TotalRegistered = reader.GetSafeInt32(startingIndex++);
                         report.TotalReferrals = reader.GetSafeInt32(startingIndex++);
                         report.Name = reader.GetSafeString(startingIndex++);
                         
                         if (reportList == null)
                         {
                             reportList = new List<RegistrationReport>();
                         }
                         reportList.Add(report);
                     }
                     else if (set == 1)
                     {
                         response = new PaginatedItemsResponse<RegistrationReport>();
                         response.TotalItems = reader.GetSafeInt32(0);
                     }
                 }

            );
            
            response.Items = reportList;
            return response;
        }

        public string ExportCsv(PaginatedRequest model)
        {
            PaginatedItemsResponse<RegistrationReport> response = GetReportByDateAndWebId(model);

            StringBuilder sb = new StringBuilder();
            string csvHeader = "Date, Website Id, Website Name, Total Registered, Total Referrals"; //this is the header for the excel columns 

            sb.AppendLine(csvHeader);
             
            foreach (var item in response.Items) // for each item in the response, it will build a line of strings within the columns of the csvHeader
            {
                sb.AppendLine(String.Format("{0},{1},{2},{3},{4}", item.Date, item.WebsiteId, item.Name, item.TotalRegistered, item.TotalReferrals));

            }

            return sb.ToString(); //appends every line into a string
        }

        public bool ForRender(PaginatedRequest model)
        {
            bool forRender = false;
             if (GetReportByDateAndWebId(model) != null)
            {
                forRender = true;
            }
            else
            {
                forRender = false;
            }

            return forRender;

        }

        public bool ForCsv(PaginatedRequest model)
        {
            bool forCsv = false;
            if (ExportCsv(model) != null)
            {
                forCsv = true;
            }
            else
            {
                forCsv = false;
            }

            return forCsv;
        }

    }
}
