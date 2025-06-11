
using AutoMapper;
using NokhbeganApi.Model;

namespace NokhbeganApi.Mapping
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<T_Notification,ShowNotificationVM>();
        }
    }
}
