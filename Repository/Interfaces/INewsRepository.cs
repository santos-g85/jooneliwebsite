using webjooneli.Models.Entities;
using webjooneli.Services.Interfaces;

namespace webjooneli.Repository.Interfaces
{
    public interface INewsRepository
    {
        Task<List<NewsModel>> GetAllNewsAsync();

        Task<List<NewsModel>> GetNewsByDateAsync();
        Task<NewsModel> GetNewsByIdAsync(string id);
        Task CreateNewsAsync(NewsModel news);
        Task UpdateNewsAsync(NewsModel updatedNews);
        Task DeleteNewsAsync(string id);
        Task CreateSubscription(NewsSubscriptionModel usermail);
    }
}
