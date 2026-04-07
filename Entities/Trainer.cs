using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Trainer.cs
[Table("trainers")]
public class Trainer
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }
    

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}