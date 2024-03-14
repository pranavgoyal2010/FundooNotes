using Dapper;
using ModelLayer.Dto;
using RepositoryLayer.Context;
using RepositoryLayer.CustomException;
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
        parameters.Add("isArchived", 0, DbType.Boolean);
        parameters.Add("isDeleted", 0, DbType.Boolean);
        parameters.Add("userId", userId, DbType.Int32);

        var insertQuery = "INSERT INTO Notes ([Title], Description, Colour, IsArchived, IsDeleted, UserId) VALUES" +
            "(@title, @description, @colour, @isArchived, @isDeleted, @userId);" +
            "SELECT CAST(SCOPE_IDENTITY() as int);";

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

            var selectQuery = "SELECT * FROM Notes WHERE UserId=@userId AND IsDeleted=0 AND IsArchived=0";

            var allNotes = await connection.QueryAsync<GetNoteDto>(selectQuery, parameters);
            return allNotes.Reverse().ToList();
        }
    }

    public async Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {
        var query = "SELECT * FROM Notes WHERE UserId=@userId AND IsDeleted=0 AND IsArchived=0";

        using (var connection = _appDbContext.CreateConnection())
        {
            var allNotes = await connection.QueryAsync<GetNoteDto>(query, new { userId });

            if (allNotes != null)
                return allNotes.Reverse().ToList();
            else
                return Enumerable.Empty<GetNoteDto>();
        }

    }

    public async Task<IEnumerable<GetNoteDto>> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("noteId", noteId, DbType.Int32);
        parameters.Add("title", updateNoteDto.Title, DbType.String);
        parameters.Add("description", updateNoteDto.Description, DbType.String);
        parameters.Add("colour", updateNoteDto.Colour, DbType.String);
        parameters.Add("userId", userId, DbType.Int32);

        var updateQuery = "UPDATE Notes " +
                    "SET Title=@title, " +
                    "Description=@description, " +
                    "Colour=@colour WHERE UserId=@userId AND NoteId=@noteId";

        using (var connection = _appDbContext.CreateConnection())
        {
            //possibility of zero or more rows getting updated due to QuerySingleOrDefaultAsync
            //bool result = await connection.QuerySingleOrDefaultAsync<bool>(updateQuery, parameters);

            //bool result = await connection.QuerySingleAsync<bool>(updateQuery, parameters);

            //QuerySingleOrDefaultAsync<bool> method is typically
            //used to retrieve a single boolean value from the database,
            //not to execute update operations.
            //To fix this, you should use ExecuteAsync instead of QuerySingleOrDefaultAsync<bool>


            int result = await connection.ExecuteAsync(updateQuery, parameters);

            if (result == 0)
                throw new UpdateFailException("Update failed please try again");

            var selectQuery = "SELECT * FROM Notes WHERE UserId=@userId AND IsDeleted=0 AND IsArchived=0";

            //QueryAsync returns a collection of rows. It's useful when you expect multiple rows to be returned from the database.
            var allNotes = await connection.QueryAsync<GetNoteDto>(selectQuery, parameters);
            return allNotes.Reverse().ToList();
        }
    }
}
