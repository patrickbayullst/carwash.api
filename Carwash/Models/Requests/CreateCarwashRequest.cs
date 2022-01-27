namespace Carwash.Models.Requests
{
    public class CreateCarwashRequest
    {
        public string Address { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
