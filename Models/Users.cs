using System.ComponentModel.DataAnnotations;

namespace PostsWebApi.Models
{
    public class Users
    {

        [Key]
        public int Id { get; set; }

        [MaxLength(11)]
        public string? Cpf { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        public DateTime? DataNascimento { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

       
        public virtual ICollection<Posts>? Posts { get; set; }
    }
}
