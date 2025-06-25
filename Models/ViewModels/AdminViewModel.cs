using webjooneli.Models.Entities;

namespace webjooneli.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<UserSessionsModel> UserSession { get; set; } = new List<UserSessionsModel>();

    }
}
