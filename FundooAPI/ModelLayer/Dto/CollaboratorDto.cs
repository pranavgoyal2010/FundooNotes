using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Dto;

public class AddCollaboratorDto
{
    [Required]
    public string CollaboratorEmail { get; set; }
}

public class GetCollaboratorDto
{
    public int CollaborateId { get; set; }
    public int NoteId { get; set; }
    public string CollaboratorEmail { get; set; }
}

public class RemoveCollaboratorDto
{
    [Required]
    public string CollaboratorEmail { get; set; }
}