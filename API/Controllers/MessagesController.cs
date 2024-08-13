
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;




public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, 
    IMapper mapper) : BaseApiController
{
    
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.GetUsername();

        if (username == createMessageDTO.RecipientUsername.ToLower())
        {
            return BadRequest("You cannot message yourself");
        }

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if(recipient == null || sender == null)
        {
            return BadRequest("cannot send message at this time");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        messageRepository.AddMessage(message);

         if (await messageRepository.SaveAllAsync())
         {
            return Ok(mapper.Map<MessageDTO>(message));
         }

         return BadRequest("Failed to save message");
    }
    

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParams )
    {
        messageParams.Username = User.GetUsername();

        var messages = await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages);

        return messages;
    }


    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername, username));
    }
}