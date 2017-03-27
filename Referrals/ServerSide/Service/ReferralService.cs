using Microsoft.Practices.Unity;
using App.Data;
using App.Web.Domain;
using App.Web.Enums;
using App.Web.Models;
using App.Web.Models.Requests;
using App.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace App.Web.Services
{
    public class ReferralService : BaseService, IReferralService
    {
       //inject the dependency for the following interfaces:
        [Dependency]
        public IUserEmailService _EmailService { get; set; }

        [Dependency]
        public ICouponService _CouponService { get; set; }

        public void SendReferrals(string SenderUserId, ReferralCodeRequest model) //pass in the sender's userId and the request model
        {
            //Use GetCouponByUserIdAndType method 
            CouponsDomain referralCoupon = _CouponService.GetCouponByUserIdAndType(SenderUserId, CouponType.referral);
            //referralCoupon now contains the information about the coupon, based on the UserId and the coupon emun referral
            
            //initially couponId is set to zero, but will get over-ridden as it goes in the if statement
            int couponId = 0;

            //If there is no referral coupon, generate a new referral coupon code for the sender.
            //If a new person registers, initially they will not have this code.
            //Therefore, this null check will check and they will have a code generated for them
            if (referralCoupon == null)
            {
               
                couponId = _CouponService.GenerateReferralCode(SenderUserId);

            }
            else //If the user already has a coupon code:
            {
                couponId = referralCoupon.Id;
            }

            //for each email, generate a token 
            //token needs: couponId, friend's email address, and the userId of the current user
            //once generated, token emailed to new user via emailservice: either sendgrid or mandrill...depending on the unity.config

            //do a loop through model.emails... loop is required because user can send more than one referral email 
            foreach (var email in model.InviteEmail)
            {
                //Checks the following:
                //Does invitation email exist in the token database? meaning have they already been sent an email?
                //Go through the database and see if there is an email that matches the var email ^^
                //if there is NO match (not a current user || invitee), bool = false and therefore send the invitation
                //if there IS a match (current user || already invited), bool = true, and therefore DO NOT send out the invitation
                {

                    bool checkUser = UserService.IsUser(email.Email);

                    //this var is null-checking if a invitation email already exists
                    ReferralTokenRequest alreadyInvited = TokenService.getAllEmailsByTokenType(TokenType.Invite, email.Email);

                    //if that email does NOT exist in the database, 
                    //AND if that email has NOT been sent an invitation....send the referral email:
                    if (checkUser == false && alreadyInvited == null)
                    {
                        //new instance of the ReferralTokenRequest model to get token:
                        ReferralTokenRequest referralTokenModel = new ReferralTokenRequest();

                        referralTokenModel.Email = email.Email;
                        referralTokenModel.CouponId = couponId;
                        referralTokenModel.UserId = SenderUserId;

                        //Create a unique token for each valid email:
                        Guid CreateTokenForInvites = TokenService.InsertReferralToken(referralTokenModel);

                        //Finally, send out invitation email
                        _EmailService.ReferralEmail(CreateTokenForInvites, email.Email);
                    }

                }


            }
        }

        //use this to delete the referral's 
        public void DeleteReferralById (int Id)
        {

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Token_DeleteById",
             inputParamMapper: delegate (SqlParameterCollection paramCollection)
             {
                 paramCollection.AddWithValue("@Id", Id);
             });

        }

    }
}