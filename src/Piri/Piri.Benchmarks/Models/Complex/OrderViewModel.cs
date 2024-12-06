namespace Piri.Benchmarks.Models.Complex
{
    public class OrderViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string OrderDateFormatted { get; set; } = string.Empty;
        public CustomerViewModel Customer { get; set; } = default!;
        public List<OrderItemViewModel> Items { get; set; } = [];
        public string TotalAmountFormatted { get; set; } = string.Empty;
        public AddressViewModel ShippingAddress { get; set; } = default!;
        public AddressViewModel BillingAddress { get; set; } = default!;
        public string ShippingStatus { get; set; } = string.Empty;
    }

    public class CustomerViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumberFormatted { get; set; } = string.Empty;
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string QuantityAndPrice { get; set; } = string.Empty; // Formatted as "Quantity x Price"
        public string TotalItemPriceFormatted { get; set; } = string.Empty;
    }

    public class AddressViewModel
    {
        public string FullAddress { get; set; } = string.Empty; // Combined "Street, City, State, PostalCode, Country"
    }

}
