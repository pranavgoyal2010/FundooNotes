using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using System.Data;

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
        var query = "INSERT INTO Users (FirstName, LastName, Email, Password) VALUES" +
            "(@fName, @lName, @email, @password);" +
            "SELECT CAST(SCOPE_IDENTITY() as int);";


        var parameters = new DynamicParameters();

        parameters.Add("fName", userRegistrationDto.FirstName, DbType.String);
        parameters.Add("lName", userRegistrationDto.LastName, DbType.String);
        parameters.Add("email", userRegistrationDto.Email, DbType.String);
        parameters.Add("password", userRegistrationDto.Password, DbType.String);

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
                        FirstName VARCHAR(45) NOT NULL,  
                        LastName VARCHAR(45) NOT NULL,      
                        Email VARCHAR(45) NOT NULL,  
                        Password VARCHAR(45) NOT NULL                             
                    );");
            }
            return await connection.QuerySingleAsync<bool>(query, parameters);
        }
        //return true;

    }

    public async Task<bool> LoginUser(UserLoginDto userLoginDto)
    {
        var query = "SELECT COUNT(*) FROM Users WHERE Email = @email AND Password = @password";

        var parameters = new DynamicParameters();

        parameters.Add("email", userLoginDto.Email, DbType.String);
        parameters.Add("password", userLoginDto.Password, DbType.String);

        using (var connection = _appDbContext.CreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<bool>(query, parameters);
        }
    }
}
