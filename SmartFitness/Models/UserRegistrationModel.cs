using System;
using System.Collections.Generic;

namespace SmartFitness.Models
{
    public class UserRegistrationModel
    {
        public string UserId { get; set; } = string.Empty;  // foreign key for workout and diet tables


        // Alapadatok
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        public required string Gender { get; set; } = "";
        public required DateTime BirthDate { get; set; } = DateTime.UtcNow;
        public required double Height { get; set; } = 0;
        public required double Weight { get; set; } = 0;



        // Edzés preferenciák
        public required string WorkoutLocation { get; set; } = ""; // gym/home
        public List<string> WorkoutTypes { get; set; } = new List<string>(); // weights/bodyweight/machine
        public int WorkoutDuration { get; set; } = 0; // percekben
        public int WorkoutDaysPerWeek { get; set; } = 0; 
        public List<string> Goal { get; set; } = new List<string>(); // strength, muscle, cardio, be healthier
        public required string ExperienceLevel { get; set; } = ""; // beginner, intermediate, advanced
                                                                           

        // Táplálkozási preferenciák
        public List<string> Intolerances { get; set; } = new List<string>();
        public List<string> DislikedFoods { get; set; } = new List<string>();
        public required string DietGoal { get; set; } = ""; // bulk, lose fat, maintain muscle
        public int CookingTime { get; set; } = 0; // per nap/hét

        public UserRegistrationModel()
        {
            WorkoutTypes = new List<string>();
            Goal = new List<string>();
            Intolerances = new List<string>();
            DislikedFoods = new List<string>();
        }
    }
}
