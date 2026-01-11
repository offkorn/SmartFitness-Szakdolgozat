using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SmartFitness.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id", true)] 
        [Column("id")]
        public string Id { get; set; }

        [Column("firstname")]
        public string FirstName { get; set; }


        [Column("lastname")]
        public string LastName { get; set; }


        [Column("email")]
        public string Email { get; set; }


        [Column("borndate")]
        public DateTime? BornDate { get; set; }


        [Column("weight")]
        public float? Weight { get; set; }


        [Column("height")]
        public float? Height { get; set; }


        [Column("gender")]
        public string Gender { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}