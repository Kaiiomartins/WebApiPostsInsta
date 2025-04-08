using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsWebApi.Models
{
    [Table("Posts")]
    public class Posts
    {
        
            [Key]
            public int Id { get; set; }

            [ForeignKey("User")]
            public int UserId { get; set; }

            public string? Description { get; set; }

            public string? ImagemUrl { get; set; }

            public string? PostType { get; set; }

            public DateTime PostDate { get; set; }

            public virtual Users? User { get; set; }

        
        }
}
