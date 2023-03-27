namespace NGSShop.Commands
{
    using NGSShop.Utils;
    using System.Threading.Tasks;
    using NGSShop.Enums;
    using NGSShop.Interfaces;
    using NGSShop.Models;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.ReplyMarkups;
    using NGSShop.Utils;

    public class AdminPanelCommand : ICommand
    {
        public string Name { get; set; } = "admin_panel";
        public string Description { get; set; } = "Управление ботом, базой данных и т.д";
        public bool IsAdmin {get; set;} = true;
        public bool IsAuthUser {get; set;} = true;
        public bool IsOnlyPrivate {get; set;} = false;
        public bool IsSuperAdmin {get; set;} = false;
        public int CooldownTime {get; set;} = 10;
        public void LoadModule()
        {
            ICommand.RegisterEventListeners(EventType.OnMessage, OnMessage);
            ICommand.RegisterEventListeners(EventType.OnButton, OnButton);
        }
        public async Task Execute(ITelegramBotClient bot, CommandContext ctx)
        {
            NGSShop.Models.User user = await NGSShop.Models.User.GetAsync(ctx.Message.From.Id);
            if (user.IsSuperAdmin)
            {
                var menu = new InlineKeyboardMarkup(new[]
                   {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Глобальный бан", "global_ban"),
                            InlineKeyboardButton.WithCallbackData("Добавить продукт", "add_product"),
                            InlineKeyboardButton.WithCallbackData("Сделать рассылку", "spam"),
                            InlineKeyboardButton.WithCallbackData("Добавить админа", "add_admin"),
                        }, 
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Получить ID пользователя", "get_id"),
                            InlineKeyboardButton.WithCallbackData("test", "test"),
                        }
                    });
                await bot.SendTextMessageAsync(ctx.Message.Chat.Id, "Выберите, что вас интересует:", replyMarkup: menu);
            }
            
        }

        public async Task OnMessage(ITelegramBotClient bot, object message)
        {
            var msg = (Message)message; 
        }

        public async Task OnButton(ITelegramBotClient bot, object result)
        {
            CallbackQuery arg = (CallbackQuery) result;
            if (arg.Data == "global_ban")
            {
                var msg = await bot.SendTextMessageAsync(arg.From.Id, "Оправьте (id | причина) без скобок. У вас 30 секунд", replyToMessageId: arg.Message.MessageId);
                
                async Task Wait(Message message)
                {   
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);

                    var data = message.Text.Split(" | ");

                    long id = long.Parse(data[0]);
                    string reason = data[1];

                    await Ban.AddBanAsync(reason, message.From.Id, id);
                }

                await MessageWaited.WaitFor(TimeSpan.FromSeconds(10), Wait, arg.From.Id);
            }

            if (arg.Data == "get_id")
            {
                var msg = await bot.SendTextMessageAsync(arg.From.Id, "Перешлите сообщение с этим пользователем, чтобы получить ID. У вас 30 секунд", replyToMessageId: arg.Message.MessageId);
                
                async Task Wait(Message message)
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, $"ID юзера - **{message.ForwardFrom.Id}**");
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                }
                async Task FailedFunc()
                {
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);

                }
                await MessageWaited.WaitFor(TimeSpan.FromSeconds(30), Wait, arg.From.Id, FailedFunc);
            }

            if (arg.Data == "spam")
            {
                var msg = await bot.SendTextMessageAsync(arg.From.Id, "Введите текст, который надо всем отправить. У вас 30 секунд", replyToMessageId: arg.Message.MessageId);
                
                async Task Wait(Message message)
                {
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                    await bot.DeleteMessageAsync(msg.Chat.Id, message.MessageId);

                    var users = await Models.User.GetAllAsync();
                    foreach (var i in users)
                    {
                        try
                        {
                            await bot.SendTextMessageAsync(i.Id, message.Text, entities: message.Entities);
                        }
                        catch 
                        {

                        }
                    }
                    
                }

                await MessageWaited.WaitFor(TimeSpan.FromSeconds(30), Wait, arg.From.Id);
            }

            if (arg.Data == "add_admin")
            {
                var msg = await bot.SendTextMessageAsync(arg.From.Id, "Введите id человека, которому надо дать админа. У вас 30 секунд", replyToMessageId: arg.Message.MessageId);

                async Task Wait(Message message)
                {
                    long id = long.Parse(message.Text);
                    
                    await bot.SendTextMessageAsync(msg.Chat.Id, id.ToString());

                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                }
                async Task FailedFunc()
                {
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                }

                await MessageWaited.WaitFor(TimeSpan.FromSeconds(30), Wait, arg.From.Id, FailedFunc);
            }
            if (arg.Data == "add_product")
            {
                var msg = await bot.SendTextMessageAsync(arg.From.Id, "Введите (название | описание | цена | город + адрес) без скобок У вас 30 секунд", replyToMessageId: arg.Message.MessageId);

                async Task Wait(Message message)
                {   
                    var data = message.Text.Split(" | ");
                    
                    string name = data[0];
                    string description = data[1];
                    float price = float.Parse(data[2]);
                    string adress = data[3];

                    await bot.SendTextMessageAsync(message.Chat.Id, $"{name} {description} {price} {adress}");
                    
                    await Product.AddAsync(name, description, adress, price);

                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                }

                async Task FailedFunc()
                {
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                }

                await MessageWaited.WaitFor(TimeSpan.FromSeconds(30), Wait, arg.From.Id, FailedFunc);
            }
            if (arg.Data == "test")
            {
                List<Page> pages = new() {};

                for (int i = 1; i < 10; i++)
                {
                    pages.Add(new(){Text = i.ToString()});
                }
                Paginator paginator = new()
                {
                    ChatId = arg.Message.Chat.Id,
                    AuthorId = arg.From.Id,
                    Pages = pages,
                    Time = TimeSpan.FromSeconds(3)
                };

                await paginator.Start(bot);
            }
        }
        
    }
}