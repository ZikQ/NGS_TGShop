namespace NGSShop.Models
{
    public class Ban
    {
        public int Id {get; set;}
        public string Reason {get; set;}
        public long AdministratorId {get; set;}
        public long TargetId {get; set;}
        
        public static async Task<Ban> GetByTargetIdAsync(long TargetId)
        {
            return await Bot.Singelton.DatabaseAPI.GetBanByTargetIdAsync(TargetId);
        }
        public static async Task<Ban> GetByIdAsync(long id)
        {
            return await Bot.Singelton.DatabaseAPI.GetBanByIdAsync(id);
        }

        public static async Task<bool> UserIsBannedAsync(long userId)
        {
            return await GetByTargetIdAsync(userId) != null;
        }
        public static async Task AddBanAsync(string reason, long administratorId, long TargetId)
        {
            Ban ban = new()
            {
                Id = 1,
                Reason = reason,
                AdministratorId = administratorId,
                TargetId = TargetId
            };

            var last = await GetLastAsync();
            if (last != null)
            {
                ban.Id = last.Id + 1;
            }

            await Bot.Singelton.DatabaseAPI.AddBanAsync(ban);
        }

        public static async Task<Ban> GetLastAsync()
        {
            var data = await Bot.Singelton.DatabaseAPI.GetLastBanAsync();

            return data;
        }
    }
}