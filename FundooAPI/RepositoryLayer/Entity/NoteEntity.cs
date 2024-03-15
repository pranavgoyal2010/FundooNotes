using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryLayer.Entity;

public class NoteEntity
{
    [Key]
    public int NoteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Colour { get; set; } = "white";
    public bool IsArchived { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

    [ForeignKey("Users")]
    public int UserId { get; set; }


}
