namespace BasketService.Api.Core.Domain.Models
{
    public class CustomerBasket
    {
        //Sepetimiz hangi user/Buyer'ın sepetini olduğu ve sepet içinde hangi ürünler olduğunu tutacağız.
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; } = new();

        public CustomerBasket()
        {
        }

        public CustomerBasket(string customerId)
        {
            BuyerId = customerId;
        }
    }
}
