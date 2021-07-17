namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        //logged in user
        public string CurrentUsername { get; set; }
        public string BoxType { get; set; } = "Unread";
    }
}