using AutoMapper;
using NokhbeganApi.Model;

namespace NokhbeganApi.Mapping
{
    public class AdminMapper : Profile
    {
        public AdminMapper()
        {
            CreateMap<T_CustomUser, GetAllStudentsAdminVM>()
                .ForMember(dest => dest.InvitedUsers, opt => opt.MapFrom(src => src.InvitedUsers)); // no need
            CreateMap<StudentTermVM, T_StudentTerm>();
            CreateMap<T_CustomUser, TermInfo>();
            CreateMap<T_StudentTerm, TermInfoAdminVM>();
            CreateMap<T_CustomUser, AdminInfoVM>();
            CreateMap<DiscountLevelVM, T_InvitationLevelDiscount>();
            CreateMap<T_CustomUser, StudentInfoFiltered>();
            CreateMap<T_InvitationLevelDiscount, ShowDiscountLevelVM>();
            CreateMap<T_GlobalInvitationConfig, ShowGlobalConfigVM>();
            CreateMap<T_Payment, ShowStudentPaymentsVM>();
        }
    }
}
