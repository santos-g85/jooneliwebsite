using webjooneli.Models.Entities;

namespace webjooneli.Repository.Interfaces
{
    public interface IMessagesRepository
    {
        Task<List<MessagesModel>> GetAllMessagesAsync();
        Task<MessagesModel> GetMessagesByIdAsync(string id);
        Task<List<MessagesModel>> GetAllMessagesSortedByDateAsync();

        Task CreateMessageAsync(MessagesModel messages);

        Task DeleteMessagesAsync(string id);
    }
}
