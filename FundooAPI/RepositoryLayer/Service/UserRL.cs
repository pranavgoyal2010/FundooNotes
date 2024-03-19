using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.CustomException;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System.Data;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Service;

public class UserRL : IUserRL
{
    private readonly AppDbContext _appDbContext;
    private readonly IAuthServiceRL _authServiceRL;
    private readonly IMailServiceRL _mailServiceRL;
    public UserRL(AppDbContext appDbContext, IAuthServiceRL authServiceRL, IMailServiceRL mailServiceRL)
    {
        _appDbContext = appDbContext;
        _authServiceRL = authServiceRL;
        _mailServiceRL = mailServiceRL;
    }

    public async Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto)
    {
        if (!isValidEmail(userRegistrationDto.Email))
            throw new InvalidCredentialsException("Invalid email format");

        var parameters = new DynamicParameters();

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);

        parameters.Add("fName", userRegistrationDto.FirstName, DbType.String);
        parameters.Add("lName", userRegistrationDto.LastName, DbType.String);
        parameters.Add("email", userRegistrationDto.Email, DbType.String);
        parameters.Add("password", hashedPassword, DbType.String);

        var query = @"INSERT INTO Users (FirstName, LastName, Email, Password)
                      VALUES (@fName, @lName, @email, @password);
                      SELECT CAST(SCOPE_IDENTITY() as int);";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>
             ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users'");

            if (!tableExists)
            {
                // Create table if it doesn't exist
                await connection.ExecuteAsync(@"
                    CREATE TABLE Users (      
                        UserId INT PRIMARY KEY IDENTITY(1,1),     
                        FirstName VARCHAR(100) NOT NULL,  
                        LastName VARCHAR(100) NOT NULL,      
                        Email VARCHAR(100) UNIQUE NOT NULL,  
                        Password VARCHAR(100) NOT NULL                             
                    );");
            }

            int userExists = await connection.QueryFirstOrDefaultAsync<int>
             ("SELECT COUNT(*) FROM Users WHERE Email = @email", parameters);
            if (userExists > 0)
                throw new UserExistsException("User already exists");

            return await connection.QuerySingleAsync<bool>(query, parameters);
        }

    }

    public async Task<string> LoginUser(UserLoginDto userLoginDto)
    {

        var parameters = new DynamicParameters();
        parameters.Add("email", userLoginDto.Email, DbType.String);

        var query = "SELECT * FROM Users WHERE Email = @email";


        using (var connection = _appDbContext.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(query, parameters);

            if (user == null)
                throw new InvalidCredentialsException("Invalid email");

            string hashedPassword = user.Password;

            bool result = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, hashedPassword);

            if (result)
                return _authServiceRL.GenerateJwtToken(user);
            else
                throw new InvalidCredentialsException("Invalid password");
        }
    }

    public async Task<string> ForgotPassword(string email)
    {
        var query = "SELECT * FROM Users WHERE Email = @email";

        using (var connection = _appDbContext.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(query, new { email });

            if (user == null)
                throw new InvalidCredentialsException("User does not exist");

            string token = _authServiceRL.GenerateJwtToken(user);

            // Generate password reset link
            var url = $"https://localhost:7151/api/user/resetpassword?token={token}";

            // HTML body for the email
            var htmlBody = @"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Password Reset</title>
            </head>
            <body>
                <h1>Password Reset</h1>
                <p>Hello,</p>
                <p>You've requested to reset your password. Please click the link below to proceed:</p>
                <a href='" + url + "'>Reset Password</a>" + @"
                <p>If you didn't request this, you can ignore this email.</p>
                <p>Best regards,<br>Your Application Team</p>
            </body>
            </html>";

            // Send password reset email
            await _mailServiceRL.SendEmail(email, "Reset Password", htmlBody);

            return token;
        }
    }


    public async Task<bool> ResetPassword(string newPassword, int userId)
    {

        var selectQuery = "SELECT * FROM Users WHERE UserId = @userId";

        var updateQuery = "UPDATE Users SET Password = @hashPassword WHERE UserId=@userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<UserEntity>(selectQuery, new { UserId = userId });

            if (user == null)
                throw new InvalidCredentialsException("user does not exist");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            int rowsAffected = await connection.ExecuteAsync(updateQuery, new { hashPassword = hashedPassword, UserId = userId });

            if (rowsAffected == 0)
                throw new UpdateFailException("password reset failed.");

            return true;
        }

    }

    public bool isValidEmail(string input)
    {
        string pattern = @"^[a-zA-Z]([\w]*|\.[\w]+)*\@[a-zA-Z0-9]+\.[a-z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }
}
