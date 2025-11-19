using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SmartFitness.Models
{
    [Table("diet_preferences")]
    public class DietPreferences : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } // UUID-hoz string

        [Column("user_id")]
        public string UserId { get; set; } // UUID-hoz string

        [Column("food_intolerance")]
        public string FoodIntolerance { get; set; } // Vesszővel elválasztott string, pl. "Dairy,Gluten"

        [Column("disliked_foods")]
        public string DislikedFoods { get; set; } // Vesszővel elválasztott string

        [Column("diet_goal")]
        public string DietGoal { get; set; }

        [Column("cookingtime")]
        public int CookingTime { get; set; } // Percekben
    }
}