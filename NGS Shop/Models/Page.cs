namespace NGSShop.Models
{
    using Telegram.Bot.Types;
    public class Page
    {
        public string Text {get; set;}
        public List<MessageEntity>? Entities {get; set;}
    }
}