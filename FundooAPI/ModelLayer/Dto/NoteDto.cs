using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class CreateNoteDto
{
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Colour { get; set; } = string.Empty;
}



public class DeleteNoteDto
{
    [Required]
    public string Title { get; set; }
    public bool IsDeleted { get; set; } = true;

}

public class ArchieveNoteDto
{
    [Required]
    public string Title { get; set; }
    public bool IsArchived { get; set; } = true;
}

public class UpdateNoteDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Colour { get; set; } = string.Empty;
}

public class GetNoteDto
{
    public int NoteId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Colour { get; set; }
    public bool IsArchived { get; set; }
    public bool IsDeleted { get; set; }

}