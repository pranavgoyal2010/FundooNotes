﻿using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class CreateNoteDto
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
}



/*public class DeleteNoteDto
{
    //[Required]
    public int NoteId { get; set; }
    public bool IsDeleted { get; set; } = true;

}

public class ArchieveNoteDto
{
    //[Required]
    public int NoteId { get; set; }
    public bool IsArchived { get; set; } = true;
}*/

public class UpdateNoteDto
{
    //[Required]
    //public int NoteId { get; set; }
    //[Required]
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
}

public class GetNoteDto
{
    public int NoteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public bool IsArchived { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

}