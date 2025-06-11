using AutoMapper;
using NokhbeganApi.Model;

namespace NokhbeganApi.Mapping
{
    public class StudentMapper : Profile
    {
        public StudentMapper()
        {
            CreateMap<T_CustomUser, GetAllStudentsVM>()
           .ForMember(dest => dest.InvitedUsers, opt => opt.MapFrom(src => src.InvitedUsers));

            CreateMap<T_CustomUser, UserInfoUpdateVM>();
            //CreateMap<UserInfoUpdateVM, T_CustomUser>();
            CreateMap<T_CustomUser, StudentInfoVM>();
            CreateMap<T_StudentTerm, TermInfo>();
            CreateMap<StudentTermUpdate, T_StudentTerm>();
        }
    }
}
