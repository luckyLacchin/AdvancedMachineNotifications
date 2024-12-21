using System.ComponentModel.DataAnnotations.Schema;

namespace DemoAPIBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [ForeignKey("Role")]
        public string RoleId { get; set; }
        public Role Role { get; set; }
    }
}
