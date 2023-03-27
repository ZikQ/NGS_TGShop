using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using NGSShop.Utils;
using System.Reflection;
using NGSShop.Commands;
using NGSShop.Interfaces;
using NGSShop.Models;
using NGSShop.Enums;
using NGSShop;

public class Bot
{
    public string Token {get; set;}
    public ITelegramBotClient Client;
    public static Bot Singelton;
    public DatabaseAPI DatabaseAPI;
    public void Init()
    {
        DatabaseAPI = new()
        {
            UrlConnection = "mongodb://localhost/"
        };

        DatabaseAPI.Init();
        
        Client = new TelegramBotClient(Token);
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        
        Client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        
        ICommand.Register<AdminPanelCommand>();
        ICommand.Register<RegisterCommand>();
        ICommand.Register<InformationCommand>();

        Singelton = this;
        Console.ReadLine();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Type == UpdateType.Message)
            {
                Message message = update.Message;
                
                if (await Ban.UserIsBannedAsync(message.From.Id)) return;

                foreach (var i in ICommand.Events[EventType.OnMessage])
                {
                    await i.Invoke(bot, message);
                }
                
                for (int i = 0; i < MessageWaited.MessageWaiteds[EventType.OnMessage].Count; i++)
                {
                    var element = MessageWaited.MessageWaiteds[EventType.OnMessage][i];
                    if (message.From.Id == element.AuthorId)
                    {
                        await element.Func.Invoke(message);
                        MessageWaited.MessageWaiteds[EventType.OnMessage].Remove(element);
                    } 
                }
                if (message.Text != null)
                {
                    if (message.Text.ToLower() == "/start")
                    {
                        await bot.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                        return;
                    }
                    
                    if (message.Text.StartsWith("/"))
                    {
                        ICommand command = ICommand.FindByName(ICommand.GetCommandName(message.Text));
                        if (command == null)
                        {
                            await bot.SendTextMessageAsync(message.Chat, "❌Ошибка! Такой команды нет");
                            return;
                        }

                        CommandContext ctx = new()
                        {
                            Message = message
                        };

                        try
                        {
                            bool flag = CooldownSystem.CheckCooldown(message.From.Id, command.Name);
                            if (!flag) return;

                            bool IsRegister = await NGSShop.Models.User.IsRegister(message.From.Id);

                            if (command.IsAuthUser & !IsRegister | command.IsAdmin & !IsRegister | command.IsSuperAdmin & !IsRegister)
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, "Эту команду можно использовать только зарегистрированнм пользователям! /register");
                                return;
                            };
                            if (message.Chat.Type != ChatType.Private & command.IsOnlyPrivate) return;
                            if (IsRegister)
                            {
                                NGSShop.Models.User user = await NGSShop.Models.User.GetAsync(message.From.Id);
                                if (!user.IsAdmin & !user.IsSuperAdmin & command.IsAdmin) return;
                                if (!user.IsSuperAdmin & command.IsSuperAdmin) return;
                            }
                            CooldownSystem.SetCooldownCommand(message.From.Id, command.Name, command.CooldownTime);
                            await command.Execute(bot, ctx);
                            
                        }
                        catch (Exception e)
                        {
                            await bot.SendTextMessageAsync(1125956031, e.Message.ToString());
                        }
                        return;
                    }
                }
                
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                CallbackQuery result = update.CallbackQuery;
                for (int i = 0; i <= ICommand.Events[EventType.OnButton].Count() - 1; i++)
                {
                    var func = ICommand.Events[EventType.OnButton][i];
                    await func.Invoke(bot, result);
                }
                // foreach (var i in ICommand.Events[EventType.OnButton])
                // {
                //     await i.Invoke(bot, result);
                // }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }
    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        await botClient.SendTextMessageAsync(1125956031, exception.ToString());
        
    }
}