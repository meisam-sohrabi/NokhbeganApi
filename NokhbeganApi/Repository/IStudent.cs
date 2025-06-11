using NokhbeganApi.Model;

namespace NokhbeganApi.Repository
{
    public interface IStudent
    {
        Task<ResponseVM> GetAllAsync(string? studentId = null);
        Task<ResponseVM> UserInfo(string studentId);
        Task<ResponseVM> GetTermTime(string studentId);


    }
}
