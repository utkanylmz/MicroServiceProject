using BasketService.Api.Core.Domain.Models;

namespace BasketService.Api.Core.Application.Repository
{
    public interface IBasketRepository
    {
        //Id'ye göre basket getrien
        Task<CustomerBasket> GetBasketAsync(string customerId);
        //redisin içerisinde bulunan bütün kullanıcıları getiren (basketi bulunan)
        IEnumerable<string> GetUsers();
        //Sepette bir güncellem yapıldığı zaman onu update eden
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        //Sepeti boşaltan metot
        Task<bool> DeleteBasketAsync(string id);
    }
}
