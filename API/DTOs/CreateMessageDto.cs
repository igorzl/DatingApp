namespace API.DTOs
{
    //we know sender user all data after login
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }
}