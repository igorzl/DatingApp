using System;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }

        //Sender properties
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }

        //Recepient properties
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }

        //Message specific properties
        public string Content { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
        public DateTime? DateRead { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}