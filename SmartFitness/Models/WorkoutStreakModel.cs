using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace SmartFitness.Models
{
    [Table("workout_streak")]
    public class WorkoutStreakModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("current_streak")]
        public int CurrentStreak { get; set; }

        [Column("last_workout_date")]
        public DateTime? LastWorkoutDate { get; set; }
    }
}
