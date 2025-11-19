using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace SmartFitness.Models;

[Table("workout_plan")]
public class WorkoutPlan : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("location")]
    public string? Location { get; set; }

    [Column("equipment")]
    public List<string>? Equipment { get; set; }

    [Column("duration_minutes")]
    public int? DurationMinutes { get; set; }

    [Column("goals")]
    public List<string>? Goals { get; set; }

    [Column("experience_level")]
    public string? ExperienceLevel { get; set; }

    [Column("days")]
    public List<string>? Days { get; set; }

    [Column("weeks")]
    public int? Weeks { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

}