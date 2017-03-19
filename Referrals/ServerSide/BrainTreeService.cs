using Braintree;
using Microsoft.Practices.Unity;
using App.Web.Domain;
using App.Web.Enums;
using App.Web.Models.Requests;
using App.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace App.Web.Services
{
    public class BrainTreeService : BaseService, IBrainTreeService
    {

        [Dependency]
        public ITransactionLogService _TransactionLogService { get; set; }
        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }
        [Dependency]
        public IActivityLogService _ActivityLogService { get; set; }
        [Dependency]
        public IUserCreditsService _CreditsService { get; set; }


        //My contribution starts at line 130


        private static BraintreeGateway _Gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            MerchantId = ConfigService.BraintreeMerchantId,
            PublicKey = ConfigService.BraintreePublicKey,
            PrivateKey = ConfigService.BraintreePrivateKey
        };


        public bool CompleteTransaction(Job job, ActivityLogAddRequest add)
        {
            bool success = false;

            List<ActivityLog> list = _ActivityLogService.GetByJobId(add.JobId);
            foreach (var activity in list)
            {

                int currentStatus = activity.TargetValue;

                if (currentStatus == (int)JobStatus.BringgOnTheWay)
                {
                    _timeCreated = activity.IdCreated;
                }
                if (currentStatus == (int)JobStatus.BringgDone)
                {
                    _timeCompleted = activity.IdCreated;
                }
            }
            TimeSpan timeDifference = _timeCompleted.Subtract(_timeCreated);
            double toMinutes = timeDifference.TotalMinutes;

            CreditCardService cardService = new CreditCardService();
            PaymentRequest payment = new PaymentRequest();
            BrainTreeService brainTreeService = new BrainTreeService();

            List<string> Slugs = new List<string>();

            Slugs.Add("base-price");
            Slugs.Add("price-per-minute");
            Slugs.Add("minimum-job-duration");
            Slugs.Add("website-pricing-model");

            Dictionary<string, WebsiteSettings> dict = WebsiteSettingsServices.getWebsiteSettingsDictionaryBySlug(job.WebsiteId, Slugs);

            WebsiteSettings pricingModel = (dict["website-pricing-model"]);
            WebsiteSettings basePrice = (dict["base-price"]);
            WebsiteSettings pricePerMin = (dict["price-per-minute"]);
            WebsiteSettings jobDuration = (dict["minimum-job-duration"]);

            // - Switch statement to calculate service cost depending on the website's pricing model

            int pricingModelValue = Convert.ToInt32(pricingModel.SettingsValue);
            switch (pricingModelValue)
            {
                case 1:
                    _basePrice = Convert.ToDouble(basePrice.SettingsValue);
                    _totalPrice = _basePrice;
                    break;

                case 2:
                    _webPrice = Convert.ToDouble(pricePerMin.SettingsValue);
                    _minJobDuration = Convert.ToDouble(jobDuration.SettingsValue);

                    if (toMinutes <= _minJobDuration)
                    {
                        _totalPrice = _webPrice * _minJobDuration;
                    }
                    else
                    {
                        _totalPrice = _webPrice * toMinutes;
                    }

                    break;

                case 3:
                    _webPrice = Convert.ToDouble(pricePerMin.SettingsValue);
                    _basePrice = Convert.ToDouble(basePrice.SettingsValue);
                    _totalPrice = _webPrice + _basePrice;
                    break;
            }


            JobsService.UpdateJobPrice(add.JobId, _totalPrice);

            if (job.UserId != null)
            {
                payment.UserId = job.UserId;
            }
            else
            {
                payment.UserId = job.Phone;
            }


            payment.ExternalCardIdNonce = job.PaymentNonce;
            payment.ItemCost = (decimal)_totalPrice;


            brainTreeService.AdminPaymentService(payment, job.Id);

            //This is where my contribution begins:

            
            //once the payment goes through, insert the referral code for user A. Existance of a TokenHash will determine if we need to award an userA.
            //NOTE: User A is the initial friend who referred User B.

            String TokenHash = _UserProfileService.GetTokenHashByUserId(job.UserId);
            //string TokenHash = "CED28811-C2DF-4629-8D2B-AE3C478A5A82"; --FOR TESTING PURPOSES 
            Guid TokenGuid;
            Guid.TryParse(TokenHash, out TokenGuid);


            if (TokenHash != null)
            {
                bool TokenUsed = TokenService.isTokenUsedReferral(TokenHash);

                Token GetUserA = TokenService.userGetByGuid(TokenGuid);

                string UserAId = GetUserA.UserId;
                int CouponReferral = GetUserA.TokenType;
                TokenType referral = (TokenType)CouponReferral; //parsing the int into an enum

                if (UserAId != null && referral == TokenType.Invite && TokenUsed == false) 
                {

                    //give User A a credit of 25 dollars
                    CouponsDomain userCoupon = TokenService.GetReferralTokenByGuid(TokenHash);

                    UserCreditsRequest insertUserACredits = new UserCreditsRequest();
                    insertUserACredits.Amount = userCoupon.CouponValue;
                    insertUserACredits.TransactionType = "Add";
                    insertUserACredits.UserId = UserAId;

                    int forTargetValue = _CreditsService.InsertUserCredits(insertUserACredits);


                    //then update the activity log for USER A to tell them that their friend completed their first order and that they were rewarded credits
                    ActivityLogAddRequest addCreditFriend = new ActivityLogAddRequest();

                    addCreditFriend.ActivityType = ActivityTypeId.CreditsFriend;
                    addCreditFriend.JobId = job.Id;
                    addCreditFriend.TargetValue = forTargetValue;
                    addCreditFriend.RawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(insertUserACredits);
                    _ActivityLogService.Insert(UserAId, addCreditFriend);

                    //update user B's activity log to show that they used the credits for their first payment
                    ActivityLogAddRequest addCredit = new ActivityLogAddRequest();

                    addCredit.ActivityType = ActivityTypeId.Credits;
                    addCredit.JobId = job.Id;
                    addCredit.TargetValue = forTargetValue;
                    addCredit.RawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(insertUserACredits);
                    _ActivityLogService.Insert(UserAId, addCredit);
                }
            }

            bool successpay = AdminPaymentService(payment, job.Id);

            if(successpay)
            {
                JobsService.UpdateJobStatus(JobStatus.Complete, job.ExternalJobId);

                ActivityLogAddRequest log = new ActivityLogAddRequest();
                log.JobId = job.Id;
                log.TargetValue = (int)JobStatus.Complete;
                log.ActivityType = ActivityTypeId.BringgTaskStatusUpdated;

                _ActivityLogService.Insert((job.UserId == null) ? job.Phone : job.UserId, log);
            }
            else
            {
                success = false;
            }

            return success;
        }

    }
}