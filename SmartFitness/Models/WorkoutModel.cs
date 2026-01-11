using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace SmartFitness.Models
{
    [Table("workout_preferences")]
    public class WorkoutModel : BaseModel
    {

        [PrimaryKey("id", false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }


        [Column("user_id")]
        public string UserId { get; set; } // Foreign key a User táblára


        [Column("workoutlocation")]
        public string WorkoutLocation { get; set; }


        [Column("workouttype")]
        public string WorkoutType { get; set; }


        [Column("workoutduration")]
        public int WorkoutDuration { get; set; } 


        [Column("workout_days")]
        public string WorkoutDays { get; set; } 


        [Column("workout_goal")]
        public string WorkoutGoal { get; set; }


        [Column("workout_experience")]
        public string WorkoutExperience { get; set; }
    }
}