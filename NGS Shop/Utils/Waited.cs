namespace NGSShop.Utils
{
    using Telegram.Bot.Types;
    using NGSShop.Enums;
    using Telegram.Bot;

    public class MessageWaited
    {
        public static Dictionary<EventType, List<MessageWaited>> MessageWaiteds = new()
        {
            {EventType.OnMessage, new()}
        };

        public TimeSpan Time {get; set;}
        public long AuthorId {get; set;}
        public Func<Message, Task> Func {get; set;}
        public static async Task WaitFor(TimeSpan time, Func<Message, Task> func, long authorId, Func<Task> failedFunc=null)
        {
            MessageWaited MessageWaited = new()
            {
                Time = time,
                Func = func,
                AuthorId = authorId,
            };

            MessageWaiteds[EventType.OnMessage].Add(MessageWaited);
            
            Thread t = new(async () =>
            {
                await Task.Delay(time);
                if (MessageWaiteds[EventType.OnMessage].Contains(MessageWaited))
                {
                    MessageWaiteds[EventType.OnMessage].Remove(MessageWaited);
                    if (failedFunc != null)
                    {
                        await failedFunc.Invoke();
                    }
                }
            });

            t.Start();
        }
    }

    public class ButtonWaited
    {
        public static Dictionary<EventType, List<ButtonWaited>> ButtonWaiteds = new()
        {
            {EventType.OnButton, new()}
        };

        public TimeSpan Time {get; set;}
        public long AuthorId {get; set;}
        public Func<CallbackQuery, Task> Func {get; set;}
        public static async Task WaitFor(TimeSpan time, Func<CallbackQuery, Task> func, long authorId, Func<Task> failedFunc=null)
        {
            ButtonWaited MessageWaited = new()
            {
                Time = time,
                Func = func,
                AuthorId = authorId,
            };

            ButtonWaiteds[EventType.OnButton].Add(MessageWaited);
            
            Thread t = new(async () =>
            {
                await Task.Delay(time);
                if (ButtonWaiteds[EventType.OnButton].Contains(MessageWaited))
                {
                    ButtonWaiteds[EventType.OnButton].Remove(MessageWaited);
                    if (failedFunc != null)
                    {
                        await failedFunc.Invoke();
                    }
                }
            });

            t.Start();
        }
    }
}