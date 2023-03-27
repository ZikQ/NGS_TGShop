namespace NGSShop.Utils
{
    using NGSShop.Models;

    public class CooldownSystem
    {
        public static Dictionary<long, List<CooldownData>> CommandsCooldown = new();

        public static void SetCooldownCommand(long id, string command, int seconds)
        {
            if (!CommandsCooldown.ContainsKey(id))
            {
                CommandsCooldown.Add(id, new());
            }
            
            CooldownData data = new()
            {
                Command = command,
                Time = DateTime.Now.AddSeconds((double)seconds)
            };
            
            CommandsCooldown[id].Add(data);
        }
        public static bool CheckCooldown(long id, string command)
        {
            if (CommandsCooldown.ContainsKey(id))
            {
                var data = CommandsCooldown[id].Find(x=>x.Command == command);
                if (data == null) return true;
                
                if (DateTime.Now > data.Time)
                {   
                    CommandsCooldown[id].Remove(data);
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}