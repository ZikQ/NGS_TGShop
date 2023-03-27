using Telegram.Bot;
using NGSShop.Models;
using Telegram.Bot.Types;
using NGSShop.Enums;

namespace NGSShop.Interfaces
{
    public interface ICommand
    {
                
        public static List<ICommand> Commands = new();
        public static Dictionary<EventType, List<Func<ITelegramBotClient, object, Task>>> Events = new()
        {
            {EventType.OnMessage, new()},
            {EventType.OnButton, new()}
        };        
        public string Name {get; set;}
        public string Description {get; set;}
        public bool IsAdmin {get; set;}
        public int CooldownTime {get; set;}
        public bool IsAuthUser {get; set;}
        public bool IsOnlyPrivate {get; set;}
        public bool IsSuperAdmin {get; set;}
        public void LoadModule();
        public Task Execute(ITelegramBotClient bot, CommandContext ctx);

        public static void Register<T>() where T : ICommand, new()
        {
            T obj = new T();
            Commands.Add(obj);
            obj.LoadModule();
        }
        public static ICommand FindByName(string name)
        {
            return Commands.Find(x=>x.Name == name);
        }
        public static void RegisterEventListeners(EventType type, Func<ITelegramBotClient, object, Task> func)
        {
            Events[type].Add(func);
        }
        public static string GetCommandName(string content)
        {
            var data = content.Split(" ");

            return data[0].Replace("/", "");
        }
    }
}