using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SmartFitness.Models
{
    [Table("weight_history")]
    public class WeightHistoryModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("weight")]
        public float Weight { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }


    }
}
