namespace GithubDashboard.Data
{
    public class ProductOrder
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}