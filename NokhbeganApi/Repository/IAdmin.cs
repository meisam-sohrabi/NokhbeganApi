using NokhbeganApi.Model;

namespace NokhbeganApi.Repository
{
    public interface IAdmin
    {
        Task<ResponseVM> GetAllVerifyAsync(int page, int pageSize);
        Task<ResponseVM> GetAllPendingAsync();
        Task<ResponseVM> GetAllFilterAsync(string? search);
        Task<ResponseVM> StudentTerms(string studentId, StudentTermVM term);
        Task<ResponseVM> GetTermTime(string studentId);
        Task<ResponseVM> UpdateTerm(Guid termId, StudentTermUpdate term);
        Task<ResponseVM> DeleteTerm(Guid termId);
        Task<ResponseVM> StudentNotification(string studentId, CreateNotificationVM model);
        Task<ResponseVM> AdminInfo(string adminId, string userRole);
        Task<ResponseVM> CreateDiscountValue(DiscountLevelVM discountLevel);
        Task<ResponseVM> UpdateDiscountValue(int level, DiscountLevelVM discountLevel);
        Task<ResponseVM> AddMaxDiscount(double maxPercent);
        Task<ResponseVM> ShowMaxDiscount();
        Task<ResponseVM> ShowDiscount();
    }
}
