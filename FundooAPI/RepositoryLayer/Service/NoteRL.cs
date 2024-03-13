using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using System.Data;

namespace RepositoryLayer.Service;

public class NoteRL : INoteRL
{
    private readonly AppDbContext _appDbContext;

    public NoteRL(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<GetNoteDto>> CreateNote(CreateNoteDto createNoteDto, int userId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("title", createNoteDto.Title, DbType.String);
        parameters.Add("description", createNoteDto.Description, DbType.String);
        parameters.Add("colour", createNoteDto.Colour, DbType.String);
        parameters.Add("isArchived", false, DbType.Boolean);
        parameters.Add("isDeleted", false, DbType.Boolean);
        parameters.Add("userId", userId, DbType.Int32);

        var insertQuery = "INSERT INTO Notes ([Title], Description, Colour, IsArchived, IsDeleted, UserId) VALUES" +
            "(@title, @description, @colour, @isArchived, @isDeleted, @userId);" +
            "SELECT CAST(SCOPE_IDENTITY() as int);";
        //"SELECT * FROM Notes WHERE UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool tableExists = await connection.QueryFirstOrDefaultAsync<bool>
             ("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Notes'");

            if (!tableExists)
            {
                // Create table if it doesn't exist
                await connection.ExecuteAsync(@"
                    CREATE TABLE Notes (      
                        NoteId INT PRIMARY KEY IDENTITY(1,1),     
                        [Title] VARCHAR(100) NOT NULL,  
                        Description VARCHAR(100),      
                        Colour VARCHAR(100),                          
                        IsArchived BIT DEFAULT(0),
                        IsDeleted BIT DEFAULT(0),
                        UserId INT FOREIGN KEY REFERENCES Users(UserId)
                    );");
            }


            bool result = await connection.QuerySingleAsync<bool>(insertQuery, parameters);

            if (!result)
            {
                throw new ArgumentNullException("Values cannot be null");
            }

            var selectQuery = "SELECT * FROM Notes WHERE UserId = @userId";

            var notes = await connection.QueryAsync<GetNoteDto>(selectQuery, parameters);
            return notes.Reverse().ToList();
        }
    }
}
