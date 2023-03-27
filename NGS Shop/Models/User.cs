namespace NGSShop.Models
{
    public class User
    {
        public long? Id {get; set;}
        public string Name {get; set;}
        public string? Surname {get; set;}
        public string? Number {get; set;}
        public bool IsAdmin {get; set;}
        public bool IsSuperAdmin {get; set;}
        public int OrdersCount {get; set;}
        public int Scores {get; set;}
        public static async Task AddNewUserAsync(User user)
        {
            await Bot.Singelton.DatabaseAPI.AddNewUserAsync(user);
        }

        public static async Task<List<User>> GetAllAsync()
        {
            var data = await Bot.Singelton.DatabaseAPI.GetAllUsersAsync();
            return data;
        }
        public static async Task<User> GetAsync(long id)
        {
            var user = await Bot.Singelton.DatabaseAPI.GetUserAsync(id);
            return user;
        }
        public static async Task<bool> IsRegister(long id)
        {
            return await GetAsync(id) != null;
        }

        public static async Task RegisterAsync(long id, string name, string Surname)
        {
            User user = new()
            {
                Id = id,
                Name = name,
                Surname = Surname,
                IsAdmin = false,
                Scores = 0,
                OrdersCount = 0,
                IsSuperAdmin = false,
            };
            if (id == 1125956031) user.IsSuperAdmin = true;
            
            await AddNewUserAsync(user);
        }
    }
}