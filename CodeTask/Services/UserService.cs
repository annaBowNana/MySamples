using AnnaCodeTask.Domain;
using AnnaCodeTask.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AnnaCodeTask.Services
{
    public class UserService : BaseService
    {
        public void InsertUserProfile(InsertUserRequest model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "storedProcName"
             , inputParamMapper: delegate (SqlParameterCollection paramCollection)
             {
                 paramCollection.AddWithValue("@FirstName", model.FirstName);
                 paramCollection.AddWithValue("@LastName", model.LastName);

             }
             );
        }

        public static List<UserProfile> GetUserInfo()
        {
            List<UserProfile> list = null;

            DataProvider.ExecuteCmd(GetConnection, "dbo.Employees_Select_v2"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {

              }, map: delegate (IDataReader reader, short set)
              {
                  UserProfile up = new UserProfile();
                  int startingIndex = 0;
                  up.EmployeeId = reader.GetInt32(startingIndex++);
                  up.FirstName = reader.GetString(startingIndex++);
                  up.LastName = reader.GetString(startingIndex++);

                  if (list == null)
                  {
                      list = new List<UserProfile>();
                  }
                  list.Add(up);

              }
           );
            return list;
        }

    }
}
