using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json; 

namespace SmartFitness.Models
{
    [Table("plan_workouts")]
    public class PlanWorkout : BaseModel
    {
        [PrimaryKey("id", true)]
        [Column("id")]
        public string Id { get; set; }
        

        [Column("plan_id")]
        public string PlanId { get; set; }

        [Column("workout_id")]
        public string WorkoutId { get; set; }
        
        [Column("day")]
        public string Day { get; set; } 

       
        [Column("user_id")]
        public string UserId { get; set; }

    }

}