using Sabio.Web.Domain;
using Sabio.Web.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services.Interfaces
{
    public interface IUserProfileService
    {
        void CreateUserProfile(string userId, CreateUserRequest model);
        UserProfile GetUserById(string userId);
        UserProfile GetUserByEmail(string Email);
        void UpdateProfileTable(string userId, CreateUserRequest model);
        void UpdateAspNetUserTable(string userId, CreateUserRequest model);
        void UpdateUserMediaId(string userId, int mediaId);
        //void UpdateExternalId(string UserId, string ExternalUserId);
        string GetExternalId(string UserId);
       // bool GetUserByEmailAndPhoneNumber(string PhoneNumber, string Email);
        void DeleteUserProfileByUserId(string Id);
        void DeleteUserWebsiteByUserId(string Id);
        void DeleteAspNetUserByUserId(string Id);
        UserProfile GetUserByPhoneNumber(string PhoneNumber);
        List<UserWebsite> GetWebsiteId();
        bool GetUserByEmailAndPhoneNumber(CreateUserRequest model);

        string GetTokenHashByUserId(string UserId);

    }
}