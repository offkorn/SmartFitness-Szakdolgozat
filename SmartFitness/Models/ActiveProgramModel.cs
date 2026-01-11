using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace SmartFitness.Models
{
    [Table("active_program")]
    public class ActiveProgramModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("plan_id")]
        public string PlanId { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
