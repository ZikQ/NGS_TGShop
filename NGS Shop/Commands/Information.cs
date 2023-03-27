namespace NGSShop.Commands
{
    using System.Threading.Tasks;
    using NGSShop.Interfaces;
    using NGSShop.Models;
    using Telegram.Bot;
    using NGSShop.Utils;

    class InformationCommand : ICommand
    {
        public string Name { get; set;} = "products";
        public string Description { get; set;} = "Список доступных товаров для заказа";
        public bool IsAdmin { get; set;} = false;
        public int CooldownTime { get; set;} = 10;
        public bool IsAuthUser { get; set;} = true;
        public bool IsOnlyPrivate { get; set;} = true;
        public bool IsSuperAdmin { get; set;} = false;

        public async Task Execute(ITelegramBotClient bot, CommandContext ctx)
        {
            var texts = await GeneratePages();

            Paginator paginator = new()
            {
                Pages = texts,
                ChatId = ctx.Message.Chat.Id,
                AuthorId = ctx.Message.From.Id,
                Time = TimeSpan.FromSeconds(10)
            };

            await paginator.Start(bot);
        }

        public void LoadModule()
        {
            
        }
        
        async Task<List<Page>> GeneratePages()
        {
            var productsList = await Product.GetAllAsync();

            List<List<Product>> splited = SplitList<Product>(productsList, 5);
            var count_embed = Math.Ceiling((decimal)splited.Count()/5);
            List<Page> texts = new();

            for (var i = 0; i <= count_embed-1; i++)
            {
                texts.Add(new()
                {
                    Text = ""
                });
            }

            for (var i = 0; i<= texts.Count() -1; i++)
            {
                foreach (var g in splited[i])
                {
                    int number = productsList.IndexOf(g) + 1;
                    texts[i].Text += $"{number}) {g.Name} - {g.Price} руб.\n{g.Description}\n\n";
                }
            }
            return texts;
        }
        public static List<List<T>> SplitList<T>(IList<T> source, int n)
            {
                return  source
                    .Select((x, i) => new { Index = i, Value = x })
                    .GroupBy(x => x.Index / n)
                    .Select(x => x.Select(v => v.Value).ToList())
                    .ToList();
            }
    }
}