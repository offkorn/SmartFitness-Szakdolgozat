using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace SmartFitness.Models
{
    [Table("workout_log")]
    public class WorkoutLogModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("workout_name")]
        public string WorkoutName { get; set; }

        [Column("done_at")]
        public DateTime DoneAt { get; set; }
    }
}
