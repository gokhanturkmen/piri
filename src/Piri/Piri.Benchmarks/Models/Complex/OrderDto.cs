namespace Piri.Benchmarks.Models.Complex
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public CustomerDto Customer { get; set; } = default!;
        public List<OrderItemDto> Items { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public AddressDto ShippingAddress { get; set; } = default!;
        public AddressDto BillingAddress { get; set; } = default!;
        public bool IsShipped { get; set; }
    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
