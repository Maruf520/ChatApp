using ChatApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.ViewCmponents
{
    public class RoomViewComponent : ViewComponent
    {
        private AppDbContext appDbContext;
        public RoomViewComponent(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IViewComponentResult Invoke()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var chats = appDbContext.ChatUsers
                .Include(x => x.Chat)
                .Where(x => x.UserId == userId)
                .Select(x => x.Chat).ToList();
            return View(chats);
        }
    }
}
