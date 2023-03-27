namespace NGSShop.Utils
{
    using MongoDB;
    using MongoDB.Driver;
    using NGSShop.Models;

    public class DatabaseAPI
    {
        public string? UrlConnection {get; set; }
        public IMongoClient? Client;
        public IMongoDatabase? Database;
        public IMongoCollection<Order> OrdersCollection;
        public IMongoCollection<User> UsersCollection;
        public IMongoCollection<Product> ProductsCollection;
        public IMongoCollection<Ban> BansCollection;
        public void Init()
        {
            Client = new MongoClient(UrlConnection);
            Database = Client.GetDatabase("NGS_SHOP");

            OrdersCollection = Database.GetCollection<Order>("OrdersCollection");
            UsersCollection = Database.GetCollection<User>("UsersCollection");
            BansCollection = Database.GetCollection<Ban>("BansCollection");
            ProductsCollection = Database.GetCollection<Product>("ProductsCollection");
        }

        public async Task AddNewUserAsync(User user)
        {
            await UsersCollection.InsertOneAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var documents = await UsersCollection.FindAsync(Builders<User>.Filter.Empty);
            return documents.ToList();
        }

        public async Task<User> GetUserAsync(long id)
        {
            var data = await UsersCollection.FindAsync(x=>x.Id == id);
            return data.FirstOrDefault();
        }
        public async Task<List<Ban>> GetAllBans()
        {
            var documents = await BansCollection.FindAsync(Builders<Ban>.Filter.Empty);
            return documents.ToList();
        }
        public async Task<List<Product>> GetAllProducts()
        {
            var documents = await ProductsCollection.FindAsync(Builders<Product>.Filter.Empty);
            return documents.ToList();
        }
        public async Task AddBanAsync(Ban ban)
        {
            await BansCollection.InsertOneAsync(ban);
        }
        public async Task AddProductAsync(Product product)
        {
            await ProductsCollection.InsertOneAsync(product);
        }
        public async Task<Ban> GetLastBanAsync()
        {
            var all = await GetAllBans();
            all.OrderBy(x=>x.Id);
            all.Reverse();
            
            Ban result = null;
            
            if (all.Count() > 0)
            {
                result = all[0];
            }

            return result;
        }

        public async Task<Ban> GetBanByTargetIdAsync(long TargetId)
        {
            var data = await BansCollection.FindAsync(x=>x.TargetId == TargetId);
            return await data.FirstOrDefaultAsync();
        }
        public async Task<Ban> GetBanByIdAsync(long id)
        {
            var data = await BansCollection.FindAsync(x=>x.Id == id);
            return await data.FirstOrDefaultAsync();
        }
        public async Task<Product> GetLastProductAsync()
        {
            var all = await GetAllProducts();
            all.OrderBy(x=>x.Id);
            all.Reverse();
            
            Product result = null;
            
            if (all.Count() > 0)
            {
                result = all[0];
            }

            return result;
        }
    }
}