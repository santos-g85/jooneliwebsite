using webjooneli.Models.Entities;
using webjooneli.Services.Interfaces;

namespace webjooneli.Repository.Interfaces
{
    public interface INewsRepository
    {
        Task<List<NewsModel>> GetAllNewsAsync();

        Task<List<NewsModel>> GetNewsByDateAsync();
        Task<NewsModel> GetNewsByIdAsync(string id);
        Task CreateNewsAsync(NewsModel news,IFormFile imageFile);
        Task UpdateNewsAsync(string id, NewsModel updatedNews);
        Task DeleteNewsAsync(string id);
    }
}
