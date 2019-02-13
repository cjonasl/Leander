using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace AddressBook
{
    public static class Security
    {
        private static void GenerateSaltedHash(string password, out string hash, out string salt)
        {
            var saltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }

        private static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        public static void CreateNewAccount(User user, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string hash, salt, hashedPassword;
                GenerateSaltedHash(user.Password, out hash, out salt);
                hashedPassword = hash + salt;

                string sqlQuery = string.Format("IF NOT EXISTS(SELECT 1 FROM [User] WHERE [Name] = '{0}') BEGIN " +
                                                "INSERT INTO [User]([Name], [Password], CreatedDate) VALUES('{0}', '{1}', '{2}') SELECT 1 END ELSE SELECT 0",
                                                user.Name, 
                                                hashedPassword,
                                                DateTime.Now.ToString("yyyy-MM-dd"));

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    int result = (int)com.ExecuteScalar(); //Return 1 if success, otherwise 0 to indicate that an account with given user name exists already.

                    if (result == 0)
                        errorMessage = string.Format("Error! An account with user name \"{0}\" exists already!", user.Name);
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }

        public static void CheckUser(User user, out int userId, out DateTime createdDate, out bool correctUserName, out bool correctPassword, out string errorMessage)
        {        
            userId = 0;
            createdDate = DateTime.MinValue;
            correctUserName = false;
            correctPassword = false;
            errorMessage = null;

            try
            {
                string hash, salt, hashedPassword;
                GenerateSaltedHash(user.Password, out hash, out salt);
                hashedPassword = hash + salt;

                string sqlQuery = string.Format("SELECT Id, Password, CreatedDate FROM [User] WHERE [Name] = '{0}'", user.Name);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    SqlDataReader reader = com.ExecuteReader();

                    if (!reader.Read())
                    {
                        errorMessage = "The user name does not exist!";
                        return;
                    }
                    else
                        correctUserName = true;

                    userId = int.Parse(reader["Id"].ToString());
                    createdDate = (DateTime)reader["CreatedDate"];
                    string password = reader["Password"].ToString();

                    if (!VerifyPassword(user.Password, password.Substring(0, 344), password.Substring(344)))
                    {
                        errorMessage = "The password is incorrect!";
                        return;
                    }
                    else
                        correctPassword = true;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }

        public static void ChangePassword(string userName, ChangePassword changePassword, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                int userId;
                DateTime createdDate;
                bool correctUserName, correctPassword;
                CheckUser(new User(userName, changePassword.OldPassword, null), out userId, out createdDate, out correctUserName, out correctPassword, out errorMessage);

                if (!correctPassword)
                {
                    errorMessage = "Error! The given old password is incorrect!";
                    return;
                }

                string hash, salt, hashedPassword;
                GenerateSaltedHash(changePassword.NewPassword, out hash, out salt);
                hashedPassword = hash + salt;

                string sqlQuery = string.Format("UPDATE [User] SET Password = '{0}' WHERE Id = {1}", hashedPassword, userId.ToString());

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbAddressBook"].ConnectionString))
                {
                    SqlCommand com = new SqlCommand(sqlQuery, conn);
                    conn.Open();
                    com.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("Error! An exception happened: {0}", e.Message);
                return;
            }
        }
    }
}