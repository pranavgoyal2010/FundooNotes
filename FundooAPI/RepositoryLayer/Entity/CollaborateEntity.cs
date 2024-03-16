using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;

public class CollaborateEntity
{
    [Key]
    public int CollaborateId { get; set; }

    [ForeignKey("Users")]
    public int UserId { get; set; }

    [ForeignKey("Notes")]
    public int NoteId { get; set; }

    [ForeignKey("Users")]
    public string CollaboratorEmail { get; set; }
}
