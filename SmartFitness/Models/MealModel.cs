using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace SmartFitness.Models
{
    [Table("meals")]
    public class Meal : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }


        [Column("user_id")]
        public string UserId { get; set; } // UUID-hoz string


        [Column("type")]
        public string Type { get; set; }


        [Column("title")]
        public string Title { get; set; }


        [Column("cooking_time")]
        public int CookingTime { get; set; }


        [Column("nutrition")]
        public Dictionary<string, int> Nutrition { get; set; }


        [Column("ingredients")]
        public string[] Ingredients { get; set; } 


        [Column("directions")]
        public string Directions { get; set; }


        [Column("tutorial_url")]
        public string TutorialUrl { get; set; }


        [Column("image_url")]
        public string ImageUrl { get; set; }


        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        [Column("author_name")]
        public string AuthorName { get; set; }


        // Getterek a Nutrition kulcsokhoz
        [JsonIgnore] // Ezeket nem kuldjuk vissza JSON-ban az adatbazisba
        public int Calories => Nutrition?.ContainsKey("calories") == true ? Nutrition["calories"] : 0;
        [JsonIgnore]
        public int Protein => Nutrition?.ContainsKey("protein") == true ? Nutrition["protein"] : 0;
        [JsonIgnore]
        public int Carbs => Nutrition?.ContainsKey("carbs") == true ? Nutrition["carbs"] : 0;
        [JsonIgnore]
        public int Fat => Nutrition?.ContainsKey("fat") == true ? Nutrition["fat"] : 0;

    }
}
