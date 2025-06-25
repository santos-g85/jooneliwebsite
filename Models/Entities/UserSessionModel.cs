namespace webjooneli.Models.Entities
{
    public class UserSessionsModel
    {
        public string UserId { get; set; }

        public string SessionToken { get; set; }
         
        public string ExpiresAt { get; set; }

        public DateTime? LastVisited { get; set; }
    }
}
