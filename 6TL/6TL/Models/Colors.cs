namespace _6TL.Models
{
    public class Colors
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }

        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

    }
}
