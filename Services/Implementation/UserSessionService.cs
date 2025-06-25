using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using webjooneli.Models.Entities;
using webjooneli.Repository.Interfaces;

namespace webjooneli.Services.Implementation
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public class UserSessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionRepository _sessionRepository;
        private const string SessionKey = "UserSession";

        public UserSessionService(IHttpContextAccessor accessor, ISessionRepository sessionRepository)
        {
            _httpContextAccessor = accessor;
            _sessionRepository = sessionRepository;
        }
        public async Task<UserSessionsModel> EnsureSession()
        {
            var context = _httpContextAccessor?.HttpContext;
            var session = context?.Session;

            if (session == null)
            {
                throw new InvalidOperationException("Session is not available.");
            }

            var userData = session.GetObject<UserSessionsModel>(SessionKey);

            if (userData == null)
            {
                userData = new UserSessionsModel
                {
                    UserId = Guid.NewGuid().ToString(),
                    SessionToken = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30).ToString("o"),
                    LastVisited = DateTime.UtcNow
                };
                await _sessionRepository.CreateSessionAsync(userData);
            }
            else
            {
                userData.LastVisited = DateTime.UtcNow;
                await _sessionRepository.UpdateSessionAsync(userData.UserId, userData);
            }

            session.SetObject(SessionKey, userData);

            return userData;
        }
    }
}
