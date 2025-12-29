using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

namespace uni_swap.Models
{
    public class UserService
    {
        private readonly IConfiguration _configuration ;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateUser(string Email,  string Password)
        {
            string sql = "SELECT PasswordHash from Student where email = @Email";
            PasswordHasher<string> pw = new PasswordHasher<string>();
            SqlDataReader dataReader = null;

            string passwordDb = "";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("connectionString")))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    conn.Open();

                    dataReader = cmd.ExecuteReader();
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            passwordDb = dataReader["PasswordHash"].ToString();
                        }
                    }
                }
            }
            var verificationResult = pw.VerifyHashedPassword(Email, passwordDb, Password);
            if (verificationResult == PasswordVerificationResult.Success)
                return true;
            else
                return false;
        }

    }
}
