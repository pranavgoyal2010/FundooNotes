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

    public async Task<GetNoteDto> CreateNote(CreateNoteDto createNoteDto, int userId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("title", createNoteDto.Title, DbType.String);
        parameters.Add("description", string.IsNullOrEmpty(createNoteDto.Description) ? null : createNoteDto.Description, DbType.String);
        parameters.Add("colour", string.IsNullOrEmpty(createNoteDto.Colour) ? null : createNoteDto.Colour, DbType.String);
        parameters.Add("isArchived", 0, DbType.Boolean);
        parameters.Add("isDeleted", 0, DbType.Boolean);
        parameters.Add("userId", userId, DbType.Int32);

        var insertQuery = "INSERT INTO Notes ([Title], Description, Colour, IsArchived, IsDeleted, UserId) VALUES" +
            "(@title, @description, @colour, @isArchived, @isDeleted, @userId);" +
            "SELECT * FROM Notes WHERE NoteId = SCOPE_IDENTITY() AND UserId = @userId;";
        //"SELECT CAST(SCOPE_IDENTITY() as int);";

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
                        [Title] VARCHAR(100),  
                        Description VARCHAR(100),      
                        Colour VARCHAR(100),                          
                        IsArchived BIT DEFAULT(0),
                        IsDeleted BIT DEFAULT(0),
                        UserId INT FOREIGN KEY REFERENCES Users(UserId)
                 );");
            }


            return await connection.QuerySingleAsync<GetNoteDto>(insertQuery, parameters);


            /*var Id = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
            return new GetNoteDto
            {
                NoteId = Id,
                Title = createNoteDto.Title,
                Description = createNoteDto.Description,
                Colour = createNoteDto.Colour,
                //IsArchived = false,
                //IsDeleted = false
            };*/
        }





        //if (!result)
        //{
        //  throw new NoteNotCreatedException("Error occured while creating note");
        //}

        //var selectQuery = "SELECT * FROM Notes WHERE UserId=@userId AND IsDeleted=0 AND IsArchived=0";

        //var allNotes = await connection.QueryAsync<GetNoteDto>(selectQuery, parameters);
        //return allNotes.Reverse().ToList();

        //return true;
        //}

    }

    public async Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {
        //var query = "SELECT * FROM Notes WHERE UserId=@userId AND IsDeleted=0 AND IsArchived=0";
        var selectQuery = "SELECT * FROM Notes WHERE UserId=@userId";
        using (var connection = _appDbContext.CreateConnection())
        {

            var allNotes = await connection.QueryAsync<GetNoteDto>(selectQuery, new { userId });

            //if (allNotes != null)
            return allNotes.Reverse().ToList();
            //else
            //    return Enumerable.Empty<GetNoteDto>();                        
        }
    }

    public async Task<GetNoteDto> GetNoteById(int userId, int noteId)
    {
        var selectQuery = "SELECT * FROM Notes WHERE UserId=@userId AND NoteId=@noteId";
        using (var connection = _appDbContext.CreateConnection())
        {

            var note = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });

            if (note != null)
                return note;
            else
                throw new NoteDoesNotExistException("Note does not exist due to wrong noteId");
        }
    }

    public async Task<GetNoteDto> UpdateNote(UpdateNoteDto updateNoteDto, int userId, int noteId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("noteId", noteId, DbType.Int32);
        parameters.Add("title", string.IsNullOrEmpty(updateNoteDto.Title) ? null : updateNoteDto.Title, DbType.String);
        parameters.Add("description", string.IsNullOrEmpty(updateNoteDto.Description) ? null : updateNoteDto.Description, DbType.String);
        parameters.Add("colour", string.IsNullOrEmpty(updateNoteDto.Colour) ? null : updateNoteDto.Colour, DbType.String);
        parameters.Add("userId", userId, DbType.Int32);

        var updateQuery = "UPDATE Notes " +
                    "SET Title=@title, " +
                    "Description=@description, " +
                    "Colour=@colour WHERE UserId=@userId AND NoteId=@noteId;";

        using (var connection = _appDbContext.CreateConnection())
        {

            int result = await connection.ExecuteAsync(updateQuery, parameters);

            if (result == 0)
                throw new UpdateFailException("Update failed please try again due to wrong NoteId");


            var selectQuery = "SELECT * FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

            var updatedNote = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });
            return updatedNote;

        }
    }

    public async Task<bool> TrashNote(int userId, int noteId)
    {
        var updateQuery = "UPDATE Notes SET IsDeleted = CASE WHEN IsDeleted = 1 THEN 0 ELSE 1 END " +
            "WHERE NoteId=@noteId AND UserId=@userId;";

        var selectQuery = "SELECT IsDeleted FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            int result = await connection.ExecuteAsync(updateQuery, new { userId, noteId });

            if (result == 0)
                throw new UpdateFailException("Move to trash failed please try again due to wrong NoteId");


            // Fetch the updated note            
            var selectQueryResult = await connection.QuerySingleOrDefaultAsync<bool>(selectQuery, new { userId, noteId });
            return selectQueryResult;

        }

    }

    public async Task<bool> ArchiveNote(int userId, int noteId)
    {
        var isDeletedQuery = "SELECT IsDeleted FROM Notes WHERE NoteId=@noteId AND UserId=@userId;";

        var updateQuery = "UPDATE Notes SET IsArchived = CASE WHEN IsArchived = 1 THEN 0 ELSE 1 END " +
            "WHERE NoteId=@noteId AND UserId=@userId AND IsDeleted=0;";

        var isArchivedQuery = "SELECT IsArchived FROM Notes WHERE NoteId = @noteId AND UserId = @userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool isDeletedQueryResult = await connection.QuerySingleOrDefaultAsync<bool>(isDeletedQuery, new { userId, noteId });
            if (isDeletedQueryResult)
                throw new ArchiveFailException("Move to archive failed because note is in trash");

            int updateQueryResult = await connection.ExecuteAsync(updateQuery, new { userId, noteId });
            if (updateQueryResult == 0)
                throw new UpdateFailException("Move to archive failed please try again due to wrong NoteId");


            // Fetch the updated note            
            var isArchivedQueryResult = await connection.QuerySingleOrDefaultAsync<bool>(isArchivedQuery, new { userId, noteId });
            return isArchivedQueryResult;

        }

    }

    public async Task<bool> DeleteNote(int userId, int noteId)
    {
        var deleteQuery = "DELETE FROM Notes WHERE NoteId=@noteId AND UserId=@userId";

        using (var connection = _appDbContext.CreateConnection())
        {
            int result = await connection.ExecuteAsync(deleteQuery, new { userId, noteId });

            if (result == 0)
                throw new DeleteFailException("Delete failed please try again due to wrong NoteId");

            return true;
        }
    }

}
