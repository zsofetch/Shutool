using Postgrest.Attributes;
using Postgrest.Models;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Shutool.Models
{
    [Table("requests")]
    public class RequestModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; }

        [Column("rider_id")]
        public string RiderId { get; set; }

        [Column("driver_id")]
        public string DriverId { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}