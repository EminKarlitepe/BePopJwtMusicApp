namespace Core.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? Sku { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Level { get; set; } = 1;           // 1=Ücretsiz, 2=Gold, 3=Premium
        public string? Features { get; set; }
        public bool IsActive { get; set; } = true;
    }
}