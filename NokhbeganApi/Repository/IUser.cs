using Microsoft.AspNetCore.JsonPatch;
using NokhbeganApi.Model;

namespace NokhbeganApi.Repository
{
    public interface IUser
    {
        Task<ResponseVM> CheckNationalIdAsync(CheckNationalIdVM checkNationalId);
        Task<ResponseVM> RegisterAsync(RegisterVM register);
        Task<ResponseVM> LoginAsync(LoginVM login);
        //Task<ResponseVM> UpdatePartialAsync(string userId, JsonPatchDocument<UserInfoUpdateVM> update);
        Task<ResponseVM> Update(string studentId, UserInfoUpdateVM update);
        Task<ResponseVM> UpdateV2(string studentId, UserInfoUpdateV2Class update);
        Task<ResponseVM> ChangePass(string userId, ChangePasswordVM changePass);
        Task<T_CustomUser> GetStudentByCode(string code);
        Task<ResponseVM> ApproveUser(string studentId);
        Task<ResponseVM> RejectUser(string studentId);
        Task<ResponseVM> LoginDataResult(string studentId, string role);
        Task<ResponseVM> NotificationIcon(string studentId);
        Task<ResponseVM> Notification(string studentId, string status);
        Task<ResponseVM> NotificationDetail(Guid id);
        Task<ResponseVM> UploadImage(IFormFile file);
    }
}
