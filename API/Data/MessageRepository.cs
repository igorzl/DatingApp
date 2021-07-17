using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            this._context = context;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            //problem with delete message
            //return await _context.Messages.FindAsync(id);
            return await _context.Messages
             .Include(u => u.Sender)
             .Include(u => u.Recipient)
             .SingleOrDefaultAsync(x => x.Id == id);
        }

        //see like in "UserRepository" -> GetMembersAsync
        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync([FromQuery] MessageParams messageParams)
        {
            var query = _context.Messages
             .OrderByDescending(x => x.DateSent)
             .AsQueryable();

            query = messageParams.BoxType switch
            {
                //all received by logged in user messages
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.CurrentUsername
                    && u.RecipientDeleted == false),
                //all sent by logged in user messages
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.CurrentUsername
                    && u.SenderDeleted == false),
                //all received, but unread by logged in user messages
                "Unread" => query.Where(u => u.RecipientUsername == messageParams.CurrentUsername
                     && u.DateRead == null
                     && u.RecipientDeleted == false),
                //default, same as "Unread"  
                _ => query.Where(u => u.RecipientUsername == messageParams.CurrentUsername
                     && u.DateRead == null
                     && u.RecipientDeleted == false)
            };

            //need mapping to return as MessageDto -> see constructor with injected IMapper
            var queryDto = query
             .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
             .AsNoTracking(); //readonly

             return await PagedList<MessageDto>.CreateAsync(queryDto,
              messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(string currentUsername, 
            string recipientUsername)
        {
            var messages = await _context.Messages
                //Eagerly loading (we are not projecting): 
                //ThenInclude see all ICollection types inside AppUser

                //Sender as AppUser plus its photos data
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                //Recipient as AppUser plus its photos data
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                )
                .OrderBy(m => m.DateSent)
                .ToListAsync(); 

            //all unread messages should be marked as read
            var unreadMessages = messages.Where(m => m.DateRead == null
                                    && m.RecipientUsername == currentUsername)
                                    .ToList(); //to loop over
            
            if(unreadMessages.Any()) //at least one unread
            {
                foreach (var item in unreadMessages)
                {
                    item.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}