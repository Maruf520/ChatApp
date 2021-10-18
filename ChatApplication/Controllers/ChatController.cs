using ChatApplication.Database;
using ChatApplication.Hubs;
using ChatApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {


        private readonly IHubContext<ChatHub> _chat;
        private readonly AppDbContext appDbContext;
        public ChatController(IHubContext<ChatHub> chat, AppDbContext appDbContext)
        {
            _chat = chat;
            this.appDbContext = appDbContext;
        }

        [HttpPost("[Action]/{connectionId}/{roomName}")]
        public async  Task<IActionResult> JoinRoom(string roomName, string connectionId)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }
        [HttpPost("[Action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string roomName, string connectionId)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }


        public async Task<IActionResult> SendMessage(string message, string roomName, int chatId)
        {
            var Messages = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                Timestamp = DateTime.Now
            };
            appDbContext.Messages.Add(Messages);
            await appDbContext.SaveChangesAsync();

            await _chat.Clients.Group(roomName).SendAsync("ReceiveMessage", Messages);
            return Ok();
        }
    }
}
