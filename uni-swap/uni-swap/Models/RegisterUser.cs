using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace uni_swap.Models
{
    public class RegisterUser
    {
        
        [Required(ErrorMessage = "First name is required")]
        public string FirstName {  get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public RegisterUser( string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public void CreateUser()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=uni-swap;Persist Security Info=True;User ID=sa;Password=root1;Encrypt=True;Trust Server Certificate=True";
            PasswordHasher<string> pw = new PasswordHasher<string>();

            string sql = "INSERT INTO STUDENT (FirstName, LastName, Email, PasswordHash)  VALUES (@FirstName, @LastName, @Email, @PasswordHash)";
            

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                
                using(SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@PasswordHash", pw.HashPassword(FirstName, Password));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
