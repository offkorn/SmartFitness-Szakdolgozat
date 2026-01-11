using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFitness.Models
{
    [Table("saved_meals")]
    public class SavedMeal : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("meal_id")]
        public string MealId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
