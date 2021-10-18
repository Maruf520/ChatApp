using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ChatApplication.Models
{
    public class User : IdentityUser
    {
        public ICollection<ChatUser> Chats { get; set; }
    }
}
