using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace SmartFitness.Models
{
    [Table("diet_preferences")]
    public class DietPreferences : BaseModel
    {

        [PrimaryKey("id", false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; } // UUID-hoz string


        [Column("user_id")]
        public string UserId { get; set; } // UUID-hoz string


        [Column("food_intolerance")]
        public string FoodIntolerance { get; set; } 


        [Column("disliked_foods")]
        public string DislikedFoods { get; set; } 


        [Column("diet_goal")]
        public string DietGoal { get; set; }


        [Column("cookingtime")]
        public int CookingTime { get; set; } 

    }
}