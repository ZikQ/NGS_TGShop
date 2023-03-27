namespace NGSShop.Models
{
    public class Product
    {
        public int Id {get; set;}
        public List<string> Images {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public int Count {get; set;}
        public float Price {get; set;}
        public int Category {get; set;}
        public List<int> Tags {get; set;}
        public string Adress {get; set;}
        public static async Task AddAsync(string name, string description, string adress, float price)
        {
            Product product = new()
            {
                Id = 1,
                Name = name, 
                Description = description,
                Adress = adress,
                Price = price
            };

            var last = await GetLastAsync();
            if (last != null)
            {
                product.Id = last.Id + 1;
            }

            await Bot.Singelton.DatabaseAPI.AddProductAsync(product);
        }
        public static async Task<Product> GetLastAsync()
        {
            var data = await Bot.Singelton.DatabaseAPI.GetLastProductAsync();

            return data;
        }

        public static async Task<List<Product>> GetAllAsync()
        {
            return await Bot.Singelton.DatabaseAPI.GetAllProducts();
        }
    }
}