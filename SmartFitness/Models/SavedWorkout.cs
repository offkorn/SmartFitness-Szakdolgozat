using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SmartFitness.Models
{
    [Table("saved_workouts")]
    public class SavedWorkout : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("workout_id")]
        public string WorkoutId { get; set; }

        // módosított gyakorlatok (Exercise lista JSON-ben)
        [Column("custom_exercises")]
        public List<Exercise> CustomExercises { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}