namespace NGSShop.Models
{
    public class Order
    {
        public int Id {get; set;}
        public long AuthorId {get; set;}
        // product - count
        public Dictionary<int, int> Products {get; set;}
        
    }
}