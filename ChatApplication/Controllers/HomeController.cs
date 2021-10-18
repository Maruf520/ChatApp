using ChatApplication.Database;
using ChatApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    [Authorize]
  
    public class HomeController : Controller
    {

        private AppDbContext appDbContext;
        public HomeController(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public IActionResult Index()
        {
            var chats = appDbContext.Chats
                .Include(x => x.Users)
                .Where(x => !x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)).ToList();
            return View(chats);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {

                Name = name,
                Type = ChatType.Room
            };
            chat.Users.Add(new ChatUser
            {
                Role = UserRole.Admin,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            }
            ); ;
            appDbContext.Chats.Add(chat);
            
            await appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> Chat(int id)
        {
            if (id == 0)
            {
                var x = appDbContext.Chats;
                return View(x);

            }
            var chats = await  appDbContext.Chats.FirstOrDefaultAsync(x=>x.Id == id);
            var Msg =  appDbContext.Messages.Where(x=>x.ChatId == id);
            chats.Messages = Msg.ToList();
            return View(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int roomId, string message)
        {
            var chat = await appDbContext.Chats.Include(x=>x.Messages).FirstOrDefaultAsync(x=>x.Id==roomId);
            var Message = new Message
            {
                ChatId = roomId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now,
                Chat = chat
            };
            appDbContext.Messages.Add(Message);
            await appDbContext.SaveChangesAsync();
            return RedirectToAction("Chat",new { id = roomId});
        }

        public async Task<IActionResult> JoinRoom(int id)
        {
            var chatuser = new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };
            appDbContext.ChatUsers.Add(chatuser);
            await appDbContext.SaveChangesAsync();
            return RedirectToAction("Chat","Home", new { id = id});
        }
    }
}
