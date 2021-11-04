using Application.Common.Interfaces;

namespace Bot.Pooling.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public long UserId => CurrentContext.User.Id;
    }
}