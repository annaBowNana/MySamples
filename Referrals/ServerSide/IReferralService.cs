using Sabio.Web.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services.Interfaces
{
    public interface IReferralService
    {
        void SendReferrals(string SenderUserId, ReferralCodeRequest model);
        void DeleteReferralById(int Id);

    }
}