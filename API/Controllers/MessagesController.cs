using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using AutoMapper;
using API.Helpers;
using System.Collections.Generic;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public MessagesController(IUserRepository userRepository,
         IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            this._mapper = mapper;
        }

        [HttpPost]
        //we have to return the MessageDto to the user
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if(username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient == null)
                return NotFound("Could not find a recipient");

            var message = new Message {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                //tracks automatically by EF according to Foreign Keys definitions
                //SenderId =
                //RecipientId = 
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync())
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                return Ok(messageDto);
            }

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.CurrentUsername = User.GetUsername();

            if(string.IsNullOrEmpty(messageParams.BoxType))
                messageParams.BoxType = "Unread";

            var messages = await _messageRepository.GetMessagesForUserAsync(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);
        }

        //we have to use specific enpoint (not just "messages")
        [HttpGet("thread/{username}")] //{username} another user (should have the same parameter name)
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThreadAsync(currentUsername, username));
        }

        [HttpDelete("{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId) 
        {
            var username = User.GetUsername();
            var message = await _messageRepository.GetMessageAsync(messageId);

            //before deleting - bunch of checks

            if(message.Sender.UserName != username &&
                message.Recipient.UserName != username)
                return Unauthorized();

            if (message.Sender.UserName == username) message.SenderDeleted = true;

            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            //deleting the message
            if (message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message");        }

    }
}