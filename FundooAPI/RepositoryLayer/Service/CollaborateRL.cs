using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.CustomException;
using RepositoryLayer.Interface;
using System.Data;
using System.Text.RegularExpressions;

namespace RepositoryLayer.Service;

public class CollaborateRL : ICollaborateRL
{
    private readonly AppDbContext _appDbContext;

    public CollaborateRL(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<bool> AddCollaborator(int userId, int noteId, AddCollaboratorDto addCollaboratorDto)
    {
        if (!isValidEmail(addCollaboratorDto.CollaboratorEmail))
            throw new InvalidCredentialsException("Invalid email format");

        var parameters = new DynamicParameters();

        parameters.Add("userId", userId, DbType.Int32);
        parameters.Add("noteId", noteId, DbType.Int32);
        parameters.Add("collaboratorEmail", addCollaboratorDto.CollaboratorEmail, DbType.String);

        //var insertQuery = "INSERT INTO (UserId, NoteId, CollaboratorEmail) VALUES" +
        //    " (@userId, @noteId, @collaboratorEmail)";

        // Adjust the insert query to include a conditional expression that returns true if at least one row is inserted,
        // and false otherwise, ensuring the returned value reflects the success of the insertion operation
        // hence select query is used.

        var insertQuery = "INSERT INTO Collaborators(UserId, NoteId, CollaboratorEmail) VALUES(@userId, @noteId, @collaboratorEmail);" +
            "SELECT * FROM Collaborators WHERE CollaborateId = SCOPE_IDENTITY();";

        var sameEmailQuery = "SELECT Email FROM Users WHERE UserId=@userId";

        var emailExistsQuery = "SELECT COUNT(*) FROM Users WHERE Email=@collaboratorEmail";

        var noteExistsQuery = "SELECT COUNT(*) FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>
             ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Collaborators'");

            if (!tableExists)
            {
                // Create table if it doesn't exist
                await connection.ExecuteAsync(@"
                    CREATE TABLE Collaborators (      
                        CollaborateId INT PRIMARY KEY IDENTITY(1,1),     
                        UserId INT,  
                        NoteId INT,      
                        CollaboratorEmail VARCHAR(100),
                        CONSTRAINT FK_UserId FOREIGN KEY (UserId) REFERENCES Users (UserId),
                        CONSTRAINT FK_NoteId FOREIGN KEY (NoteId) REFERENCES Notes (NoteId),
                        CONSTRAINT FK_CollaboratorEmail FOREIGN KEY (CollaboratorEmail) REFERENCES Users (Email)
                    );"
                );
            }



            bool emailExists = await connection.QueryFirstOrDefaultAsync<bool>(emailExistsQuery, parameters);
            if (!emailExists)
                throw new InvalidCredentialsException("Email id is not registered");

            string sameEmail = await connection.QueryFirstAsync<string>(sameEmailQuery, parameters);
            if (sameEmail.Equals(addCollaboratorDto.CollaboratorEmail))
                throw new InvalidCredentialsException("user cannot collaborate with itself");

            bool noteExists = await connection.QueryFirstOrDefaultAsync<bool>(noteExistsQuery, parameters);
            if (!noteExists)
                throw new NoteDoesNotExistException("Note Not found with provided note Id");

            return await connection.QueryFirstOrDefaultAsync<bool>(insertQuery, parameters);
        }
    }

    public bool isValidEmail(string input)
    {
        string pattern = @"^[a-zA-Z]([\w]*|\.[\w]+)*\@[a-zA-Z0-9]+\.[a-z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }
}
