using System;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        //Sender properties
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }

        //Recepient properties
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }

        //Message specific properties
        public string Content { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime? DateRead { get; set; }
    }
}