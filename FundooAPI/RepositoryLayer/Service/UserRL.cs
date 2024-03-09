using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.CustomException;
using RepositoryLayer.Interface;
using System.Data;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Service;

public class UserRL : IUserRL
{
    private readonly AppDbContext _appDbContext;

    public UserRL(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<bool> RegisterUser(UserRegistrationDto userRegistrationDto)
    {
        if (!isValidEmail(userRegistrationDto.Email))
            throw new InvalidEmailFormatException("Invalid email format");

        var query = "INSERT INTO Users (FirstName, LastName, Email, Password) VALUES" +
            "(@fName, @lName, @email, @password);" +
            "SELECT CAST(SCOPE_IDENTITY() as int);";

        var parameters = new DynamicParameters();

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);

        parameters.Add("fName", userRegistrationDto.FirstName, DbType.String);
        parameters.Add("lName", userRegistrationDto.LastName, DbType.String);
        parameters.Add("email", userRegistrationDto.Email, DbType.String);
        parameters.Add("password", hashedPassword, DbType.String);

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

    public async Task<bool> LoginUser(UserLoginDto userLoginDto)
    {
        var query = "SELECT Email, Password FROM Users WHERE Email = @email";

        var parameters = new DynamicParameters();

        //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userLoginDto.Password);        

        parameters.Add("email", userLoginDto.Email, DbType.String);
        //parameters.Add("password", hashedword, DbType.String);

        using (var connection = _appDbContext.CreateConnection())
        {
            var user = await connection.QueryFirstOrDefaultAsync<UserLoginDto>(query, parameters);

            if (user == null)
                throw new InvalidCredentialsException("Invalid email or password");

            string hashedPassword = user.Password;

            bool result = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, hashedPassword);

            if (result)
                return true;
            else
                throw new InvalidCredentialsException("Invalid email or password");
        }
    }

    public bool isValidEmail(string input)
    {
        string pattern = @"^[a-zA-Z]([\w]*|\.[\w]+)*\@[a-zA-Z0-9]+\.[a-z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }
}
