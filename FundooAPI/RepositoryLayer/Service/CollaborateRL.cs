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

    public async Task<IEnumerable<GetCollaboratorDto>> GetAllCollaborators(int userId)
    {
        var selectQuery = "SELECT * FROM Collaborators WHERE UserId=@userId";
        using (var connection = _appDbContext.CreateConnection())
        {

            var allCollaborators = await connection.QueryAsync<GetCollaboratorDto>(selectQuery, new { userId });

            return allCollaborators.Reverse().ToList();
        }
    }

    public async Task<IEnumerable<GetCollaboratorDto>> GetAllCollaboratorsById(int userId, int noteId)
    {
        var selectQuery = "SELECT * FROM Collaborators WHERE UserId=@userId AND NoteId=@noteId";

        //query to check if the entered noteId exists or not
        var noteExistsQuery = "SELECT COUNT(*) FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool noteExists = await connection.QueryFirstOrDefaultAsync<bool>(noteExistsQuery, new { userId, noteId });
            if (!noteExists)
                throw new NoteDoesNotExistException("Note Not found with provided note Id");

            var allCollaborators = await connection.QueryAsync<GetCollaboratorDto>(selectQuery, new { userId, noteId });

            return allCollaborators.Reverse().ToList();
        }
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
        // hence select query is used just after insertion.

        var insertQuery = "INSERT INTO Collaborators(UserId, NoteId, CollaboratorEmail) VALUES(@userId, @noteId, @collaboratorEmail);" +
            "SELECT * FROM Collaborators WHERE CollaborateId = SCOPE_IDENTITY();";

        //query to check if the email entered is registered email or not
        var emailExistsQuery = "SELECT COUNT(*) FROM Users WHERE Email=@collaboratorEmail";

        //query if the user trys collaborate with itself
        var sameEmailQuery = "SELECT Email FROM Users WHERE UserId=@userId";

        //query to check if the entered noteId exists or not
        var noteExistsQuery = "SELECT COUNT(*) FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        //query to check if the entered email is already collaborator or not.
        var alreadyCollaboratorQuery = "SELECT COUNT(*) FROM Collaborators WHERE NoteId = @noteId AND UserId = @userId AND CollaboratorEmail=@collaboratorEmail";

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
                        CONSTRAINT FK_NoteId FOREIGN KEY (NoteId) REFERENCES Notes (NoteId) ON DELETE CASCADE,
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

            bool alreadyCollaborator = await connection.QueryFirstOrDefaultAsync<bool>(alreadyCollaboratorQuery, parameters);
            if (alreadyCollaborator)
                throw new InvalidCredentialsException("email is already a collaborator for provided noteId");

            return await connection.QueryFirstOrDefaultAsync<bool>(insertQuery, parameters);
        }
    }

    public async Task<bool> RemoveCollaborator(int userId, int noteId, RemoveCollaboratorDto removeCollaboratorDto)
    {
        if (!isValidEmail(removeCollaboratorDto.CollaboratorEmail))
            throw new InvalidCredentialsException("Invalid email format");

        var parameters = new DynamicParameters();

        parameters.Add("userId", userId, DbType.Int32);
        parameters.Add("noteId", noteId, DbType.Int32);
        parameters.Add("collaboratorEmail", removeCollaboratorDto.CollaboratorEmail, DbType.String);

        var deleteQuery = "DELETE FROM Collaborators WHERE UserId=@userId AND NoteId=@noteId AND CollaboratorEmail=@collaboratorEmail;";

        var sameEmailQuery = "SELECT Email FROM Users WHERE UserId=@userId";

        var emailExistsQuery = "SELECT COUNT(*) FROM Users WHERE Email=@collaboratorEmail";

        var noteExistsQuery = "SELECT COUNT(*) FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool emailExists = await connection.QueryFirstOrDefaultAsync<bool>(emailExistsQuery, parameters);
            if (!emailExists)
                throw new InvalidCredentialsException("Email id is not registered");

            string sameEmail = await connection.QueryFirstAsync<string>(sameEmailQuery, parameters);
            if (sameEmail.Equals(removeCollaboratorDto.CollaboratorEmail))
                throw new InvalidCredentialsException("user cannot part with itself");

            bool noteExists = await connection.QueryFirstOrDefaultAsync<bool>(noteExistsQuery, parameters);
            if (!noteExists)
                throw new NoteDoesNotExistException("Note Not found with provided note Id");

            int result = await connection.ExecuteAsync(deleteQuery, parameters);
            if (result == 0)
                throw new DeleteFailException("Provided collaborator email is not collaborated with provided noteId");

            return true;
        }

    }

    public bool isValidEmail(string input)
    {
        string pattern = @"^[a-zA-Z]([\w]*|\.[\w]+)*\@[a-zA-Z0-9]+\.[a-z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }
}
