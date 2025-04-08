using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PostsWebApi.Models
{
    [Table("Followers")]
    public class Follows
    {

        
            [Key, Column(Order = 0)]
            public int SeguidorId { get; set; }

            [Key, Column(Order = 1)]
            public int SeguidoId { get; set; }

            public DateTime DataFollow { get; set; } = DateTime.Now;

            public DateTime? DataUnFollow { get; set; }

           
            [ForeignKey("SeguidorId")]
            public virtual Users? Seguidor { get; set; }

           
            [ForeignKey("SeguidoId")]
            public virtual Users? Seguido { get; set; }
        }
}
