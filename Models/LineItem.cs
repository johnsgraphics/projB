namespace CabinetDocProWpf.Models
{
    public class LineItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; } = 0;
        public decimal Total => Quantity * UnitPrice;
    }
}
