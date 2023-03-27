using NGSShop.Interfaces;
using NGSShop.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NGSShop.Utils
{
    public class Paginator
    {
        public long ChatId {get; set;}
        public long AuthorId {get; set;}
        public List<Page> Pages {get; set;}
        private int CurrentPage {get; set;} = 0;
        private Message Message {get; set;}
        public TimeSpan Time {get; set;}
        private Thread Thread  {get; set;}
        private DateTime LastUpdate {get; set;} = DateTime.Now;
        public async Task Start(ITelegramBotClient bot)
        {
            Page page = Pages[CurrentPage];

            var menu = new InlineKeyboardMarkup(new[]
                   {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("⬅️", "pre"),
                            InlineKeyboardButton.WithCallbackData("➡️", "next"),
                        }
                    });
            Message = await bot.SendTextMessageAsync(ChatId, page.Text);
            if (Pages.Count != 1)
            {
                await bot.EditMessageTextAsync(ChatId, Message.MessageId, Pages[CurrentPage].Text, replyMarkup: menu);
                Thread = new(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(Time);

                        if (DateTime.Now > LastUpdate + Time)
                        {
                            var b = ICommand.Events[Enums.EventType.OnButton].Find(x=> x==OnButton);
                            ICommand.Events[Enums.EventType.OnButton].Remove(b);
                            await bot.EditMessageTextAsync(ChatId, Message.MessageId, Pages[CurrentPage].Text);
                            break;
                        }
                    }
                });

                Thread.Start();
                
                ICommand.RegisterEventListeners(Enums.EventType.OnButton, OnButton);
            }
        }

        public async Task OnButton(ITelegramBotClient bot, object arg)
        {
            CallbackQuery result = (CallbackQuery) arg;

            if (result.Message.MessageId == Message.MessageId & result.From.Id == AuthorId)
            {
                if (result.Data == "next")
                {
                    await NextPageAsync(bot);
                }
                else
                {
                    await PrePageAsync(bot);
                }
            }
        }

        async Task NextPageAsync(ITelegramBotClient bot)
        {
            CurrentPage++;
            if (CurrentPage > Pages.Count())
            {
                CurrentPage = 0;
            }
            
            await UpdateMessageAsync(bot);
        }
        
        async Task PrePageAsync(ITelegramBotClient bot)
        {
            CurrentPage--;
            if (CurrentPage < 0)
            {
                CurrentPage = Pages.Count() - 1;
            }
            
            await UpdateMessageAsync(bot);
        }

        async Task UpdateMessageAsync(ITelegramBotClient bot)
        {
            Page page = Pages[CurrentPage];

            var menu = new InlineKeyboardMarkup(new[]
                   {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("⬅️", "pre"),
                            InlineKeyboardButton.WithCallbackData("➡️", "next"),
                        }
                    });

            await bot.EditMessageTextAsync(ChatId, Message.MessageId, page.Text, replyMarkup: menu);
            LastUpdate = DateTime.Now;
        }
    }
}