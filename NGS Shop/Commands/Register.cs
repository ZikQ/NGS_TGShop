using NGSShop.Interfaces;
using NGSShop.Models;
using Telegram.Bot;

namespace NGSShop.Commands
{
    public class RegisterCommand : ICommand
    {
        public string Name { get; set;} = "register";
        public string Description { get; set;} = "Регистрация пользователя";
        public bool IsAdmin { get; set; } = false;
        public int CooldownTime {get; set;} = 10;
        public bool IsAuthUser {get; set;} = false;
        public bool IsOnlyPrivate {get; set;} = true;
        public bool IsSuperAdmin {get; set;} = false;
        public async Task Execute(ITelegramBotClient bot, CommandContext ctx)
        {
            if (ctx.Message.From.IsBot | await User.IsRegister(ctx.Message.From.Id)) return;
            
            await User.RegisterAsync(ctx.Message.From.Id, ctx.Message.From.FirstName, ctx.Message.From.LastName);
            await bot.SendTextMessageAsync(ctx.Message.Chat.Id, "Поздравляю! Вы успешно зарегистрировались");
        }

        public void LoadModule()
        {
            
        }
    }
}