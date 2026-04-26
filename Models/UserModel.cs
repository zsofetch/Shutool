using Postgrest.Attributes;
using Postgrest.Models;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Shutool.Models
{
    [Table("users")]
    public class UserModel : BaseModel
    {
        [PrimaryKey("id", false)] // False because Supabase Auth generates the UUID
        public string Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("role")]
        public string Role { get; set; }

        [Column("student_id")]
        public string StudentId { get; set; }

        [Column("avatar_id")]
        public int AvatarId { get; set; }
    }
}