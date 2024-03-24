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

        var insertQuery = @"INSERT INTO Notes ([Title], Description, Colour, IsArchived, IsDeleted, UserId) 
                            VALUES (@title, @description, @colour, @isArchived, @isDeleted, @userId);
                            SELECT * FROM Notes WHERE NoteId = SCOPE_IDENTITY() AND UserId = @userId;";


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
                        UserId INT,
                        FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
                 );");
            }


            return await connection.QuerySingleAsync<GetNoteDto>(insertQuery, parameters);

        }

    }

    public async Task<IEnumerable<GetNoteDto>> GetAllNotes(int userId)
    {

        //following query will display all notes including the collaborated notes
        var selectQuery = @"SELECT DISTINCT N.*
                            FROM Notes N
                            LEFT JOIN Collaborators C ON N.NoteId = C.NoteId
                            WHERE N.UserId = @userId OR C.CollaboratorEmail = 
                                (SELECT Email FROM Users U WHERE U.UserId = @userId);
                            ";


        using (var connection = _appDbContext.CreateConnection())
        {

            var allNotes = await connection.QueryAsync<GetNoteDto>(selectQuery, new { userId });

            //return allNotes.Reverse().ToList();
            return allNotes.ToList();
        }
    }

    public async Task<GetNoteDto> GetNoteById(int userId, int noteId)
    {

        //query will allow to get a note that is of this userId and the ones this userId has collaboration access to
        var selectQuery = @"SELECT * FROM Notes 
                            WHERE (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND NoteId = @noteId;";

        using (var connection = _appDbContext.CreateConnection())
        {

            var note = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });

            if (note != null)
            {
                return note;
            }
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

        //query will allow to update a note that is of this userId and the ones this userId has collaboration access to
        var updateQuery = @"
            UPDATE Notes 
            SET Title = @title, 
                Description = @description, 
                Colour = @colour 
            WHERE NoteId = @noteId AND UserId = @userId;

            UPDATE Notes 
            SET Title = @title, 
                Description = @description, 
                Colour = @colour 
            WHERE NoteId = @noteId AND NoteId IN (
                SELECT NoteId FROM Collaborators WHERE CollaboratorEmail = (
                    SELECT Email FROM Users WHERE UserId = @userId
                )
        );";

        //query will allow to get a note that is of this userId and the ones this userId has collaboration access to
        var selectQuery = @"SELECT * FROM Notes 
                            WHERE (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND NoteId = @noteId;";


        using (var connection = _appDbContext.CreateConnection())
        {

            int result = await connection.ExecuteAsync(updateQuery, parameters);

            if (result == 0)
                throw new UpdateFailException("Update failed please try again due to wrong NoteId");

            //Fetch the updated note
            var updatedNote = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });
            return updatedNote;

        }
    }

    public async Task<GetNoteDto> TrashNote(int userId, int noteId)
    {
        //allow to trash any note including the collaborated ones
        var updateQuery = @"UPDATE Notes 
                            SET IsDeleted = CASE WHEN IsDeleted = 1 THEN 0 ELSE 1 END 
                            WHERE NoteId = @noteId AND (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            ));";

        //query will allow to get a note that is of this userId and the ones this userId has collaboration access to
        var selectQuery = @"SELECT * FROM Notes 
                            WHERE (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND NoteId = @noteId;";

        using (var connection = _appDbContext.CreateConnection())
        {
            int result = await connection.ExecuteAsync(updateQuery, new { userId, noteId });

            if (result == 0)
                throw new UpdateFailException("Move to trash failed please try again due to wrong NoteId");


            // Fetch the updated note            
            var selectQueryResult = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });
            return selectQueryResult;

        }

    }

    public async Task<GetNoteDto> ArchiveNote(int userId, int noteId)
    {
        //query to check if note is trahsed
        var isDeletedQuery = @"SELECT IsDeleted FROM Notes 
                            WHERE (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND NoteId = @noteId;";

        //allow to archive any note including the collaborated ones
        var updateQuery = @"UPDATE Notes 
                            SET IsArchived = CASE WHEN IsArchived = 1 THEN 0 ELSE 1 END 
                            WHERE NoteId = @noteId AND (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND IsDeleted=0;";

        //query to return the updated note
        var selectQuery = @"SELECT * FROM Notes 
                            WHERE (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            )) AND NoteId = @noteId;";

        using (var connection = _appDbContext.CreateConnection())
        {
            bool isDeletedQueryResult = await connection.QuerySingleOrDefaultAsync<bool>(isDeletedQuery, new { userId, noteId });
            if (isDeletedQueryResult)
                throw new ArchiveFailException("Move to archive failed because note is in trash");

            int updateQueryResult = await connection.ExecuteAsync(updateQuery, new { userId, noteId });
            if (updateQueryResult == 0)
                throw new UpdateFailException("Move to archive failed please try again due to wrong NoteId");


            // Fetch the updated note
            var selectQueryResult = await connection.QuerySingleOrDefaultAsync<GetNoteDto>(selectQuery, new { userId, noteId });
            return selectQueryResult;

        }

    }

    public async Task<bool> DeleteNote(int userId, int noteId)
    {
        //query to delete any note included the collaborated ones.
        var deleteQuery = @"DELETE FROM Notes 
                            WHERE NoteId = @noteId AND (UserId = @userId OR NoteId IN (
                                SELECT NoteId 
                                FROM Collaborators 
                                WHERE CollaboratorEmail = (SELECT Email FROM Users WHERE UserId=@userId)
                            ));";

        using (var connection = _appDbContext.CreateConnection())
        {
            int result = await connection.ExecuteAsync(deleteQuery, new { userId, noteId });

            if (result == 0)
                throw new DeleteFailException("Delete failed please try again due to wrong NoteId");

            return true;
        }
    }

}
