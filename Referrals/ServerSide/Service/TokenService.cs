using App.Data;
using App.Web.Domain;
using App.Web.Enums;
using App.Web.Models.Requests;
using App.Web.Models.Responses;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace App.Web.Services
{
    public class TokenService : BaseService
    {

        //Line 21 - 185 was written by my team.


        //Starting at line 189 is where my contribution starts.




        public static Guid tokenInsert(string userId)
        {

            //int id = 0;
            Guid tokenHash = Guid.Empty;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Token_Insert"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {

                paramCollection.AddWithValue("@UserId", userId);
                //paramCollection.AddWithValue("@TokenType", 1);

                SqlParameter c = new SqlParameter("@TokenHash", System.Data.SqlDbType.UniqueIdentifier);
                c.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(c);
            },
            returnParameters: delegate (SqlParameterCollection param)
            {
                Guid.TryParse(param["@TokenHash"].Value.ToString(), out tokenHash);
            }
            );
            return tokenHash;
        }





        public static Token userGetByGuid(Guid emailGuid)
        {
            Token c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Token_GetByGuid"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@TokenHash", emailGuid);


              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  c = new Token();
                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.Used = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.TokenHash = reader.GetSafeGuid(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.TokenType = reader.GetSafeInt32(startingIndex++);

              }

           );

            return c;
        }

        public static CouponsDomain GetReferralTokenByGuid(string tokenhash)
        {
            CouponsDomain c = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Coupons_SelectByTokenId"
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@TokenHash", tokenhash);
                }, map: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;

                    c = new CouponsDomain();

                    c.Id = reader.GetSafeInt32(startingIndex++);
                    c.CouponDateCreated = reader.GetSafeDateTime(startingIndex++);
                    c.CouponDateModified = reader.GetSafeDateTime(startingIndex++);
                    c.WebsiteId = reader.GetSafeInt32(startingIndex++);
                    c.CouponType = reader.GetSafeInt32(startingIndex++);
                    c.CouponCode = reader.GetSafeString(startingIndex++);
                    c.CouponValue = reader.GetSafeDecimal(startingIndex++);
                    c.CouponActiveDate = reader.GetSafeDateTime(startingIndex++);
                    c.CouponExpirationDate = reader.GetSafeDateTime(startingIndex++);
                    c.CouponDescription = reader.GetSafeString(startingIndex++);
                    c.UserId = reader.GetSafeString(startingIndex++);

                    Token t = new Token();

                    t.Id = reader.GetSafeInt32(startingIndex++);
                    t.DateCreated = reader.GetSafeDateTime(startingIndex++);
                    t.Used = reader.GetSafeDateTimeNullable(startingIndex++);
                    t.TokenHash = reader.GetSafeGuid(startingIndex++);
                    t.UserId = reader.GetSafeString(startingIndex++);
                    t.TokenType = reader.GetSafeInt32(startingIndex++);
                    t.CouponId = reader.GetSafeInt32(startingIndex++);
                    t.Email = reader.GetSafeString(startingIndex++);

                    c.Token = t;
                });
            return c;
        }

        public static bool isTokenUsedReferral(string tokenhash)
        {
            bool isSuccessful = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Token_Referrals_Update"
                    , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                    {
                        paramCollection.AddWithValue("@TokenHash", tokenhash);

                    }, returnParameters: delegate (SqlParameterCollection param)
                    {
                        isSuccessful = true;
                    });


            return isSuccessful;

        }

        public static bool tokenUsedUpdate(string UserId, string tokenhash)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Token_ASPNetUsers_Update"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@UserId", UserId);
                          paramCollection.AddWithValue("@TokenHash", tokenhash);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });


            return success;
        }

        public static List<Token> getAllToken()
        {
            List<Token> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Token_SelectAll"
              , inputParamMapper: null
              , map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  Token c = new Token();
                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.Used = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.TokenHash = reader.GetSafeGuid(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.TokenType = reader.GetSafeInt32(startingIndex++);
                  c.CouponId = reader.GetSafeInt32(startingIndex++);
                  c.Email = reader.GetSafeString(startingIndex++);

                  if (list == null)
                  {
                      list = new List<Token>();
                  }
                  list.Add(c);
              }
              );
            return list;
        }


        //This is where my contribution starts:
        public static Guid InsertReferralToken(ReferralTokenRequest model) 
        {
            Guid tokenHash = Guid.Empty;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Token_Insert_v2"
            , inputParamMapper: delegate (SqlParameterCollection paramCollection)
            {
                SqlParameter c = new SqlParameter("@TokenHash", System.Data.SqlDbType.UniqueIdentifier);
                c.Direction = System.Data.ParameterDirection.Output;

                paramCollection.Add(c);

                paramCollection.AddWithValue("@UserId", model.UserId);
                paramCollection.AddWithValue("@Email", model.Email);
                paramCollection.AddWithValue("@CouponId", model.CouponId);
                paramCollection.AddWithValue("@TokenType", TokenType.Invite);


            },
            returnParameters: delegate (SqlParameterCollection param)
            {
                Guid.TryParse(param["@TokenHash"].Value.ToString(), out tokenHash);
            }
            );
            return tokenHash;
        }


        public static List<Token> getTokenByUserIdAndTokenType(string UserId)
        {
            List<Token> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Token_SelectAll_v2"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", UserId);
                  paramCollection.AddWithValue("@TokenType", TokenType.Invite);


              }
              , map: delegate (IDataReader reader, short set)
              {

                  int startingIndex = 0;
                  Token c = new Token();
                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.Used = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.TokenType = reader.GetSafeInt32(startingIndex++);
                  c.CouponId = reader.GetSafeInt32(startingIndex++);
                  c.Email = reader.GetSafeString(startingIndex++);

                  if (list == null)
                  {
                      list = new List<Token>();
                  }
                  list.Add(c);
              }


              );

            return list;
        }

        //Update to the Paginated get page
        public static PaginatedItemsResponse<Token> getTokenByUserIdAndTokenTypePagination(string UserId, PaginatedRequest model)
        {
            List<Token> list = null;
            PaginatedItemsResponse<Token> response = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Token_SelectAll_v3"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@UserId", UserId);
                  paramCollection.AddWithValue("@TokenType", TokenType.Invite);
                  paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                  paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);


              }
              , map: delegate (IDataReader reader, short set)
              {

                  if (set == 0)
                  {

                      int startingIndex = 0;
                      Token c = new Token();
                      c.Id = reader.GetSafeInt32(startingIndex++);
                      c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                      c.Used = reader.GetSafeDateTimeNullable(startingIndex++);
                      c.UserId = reader.GetSafeString(startingIndex++);
                      c.TokenType = reader.GetSafeInt32(startingIndex++);
                      c.CouponId = reader.GetSafeInt32(startingIndex++);
                      c.Email = reader.GetSafeString(startingIndex++);

                      if (list == null)
                      {
                          list = new List<Token>();
                      }
                      list.Add(c);
                  }
                  else if (set == 1)

                  {
                      response = new PaginatedItemsResponse<Token>();
                      response.TotalItems = reader.GetSafeInt32(0);

                  }
              }

              );
            response.Items = list;
            return response;
        }

        public static ReferralTokenRequest getAllEmailsByTokenType(TokenType Invite, string email)
        {

            ReferralTokenRequest emailMatch = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Token_GetTokenByEmailAndTokenType"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@TokenType", TokenType.Invite);
                  paramCollection.AddWithValue("@Email", email);

              }
              , map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;
                  Token c = new Token();
                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.DateCreated = reader.GetSafeDateTime(startingIndex++);
                  c.Used = reader.GetSafeDateTime(startingIndex++);
                  c.TokenHash = reader.GetSafeGuid(startingIndex++);
                  c.UserId = reader.GetSafeString(startingIndex++);
                  c.TokenType = reader.GetSafeInt32(startingIndex++);
                  c.CouponId = reader.GetSafeInt32(startingIndex++);
                  c.Email = reader.GetSafeString(startingIndex++);

                  if (c == null)
                  {
                      emailMatch = null;
                  }

              }
              );
            return emailMatch;
        }

    }
}
