using DemoAPIBot.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoAPIBot.Models
{
    [PrimaryKey(nameof(UserID))]
    public class RefreshToken
    {
        public string UserID { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }

    }
}
