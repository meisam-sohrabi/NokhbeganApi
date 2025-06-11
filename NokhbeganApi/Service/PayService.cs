using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Context;
using NokhbeganApi.Model;
using NokhbeganApi.Repository;

namespace NokhbeganApi.Service
{
    public class PayService : IPay
    {
        private readonly NokhbeganDbContext _context;


        public PayService( NokhbeganDbContext context)
        {
            _context = context;
        }

       
        public async Task<ResponseVM> GetPayment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var error = new ResponseVM
                {
                    Message = "There is a problem with parameter.",
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Data = null
                };
                return error;

            }
            var payment = await _context.payments.FirstOrDefaultAsync(p => p.UserPaidId == id);

            if (payment != null)
            {
                var success = new ResponseVM
                {
                    Data = payment

                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {

                    Data = null

                };
                return error;
            }

        }

        public async Task<ResponseVM> OnlinePay(string id)
        {
            #region PaymentPage


            var user = await _context.Users.Include(c => c.StudentTerms).Where(c => c.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                var term = user.StudentTerms.FirstOrDefault(c => !c.IsActive);
                if (term != null)
                {
                    var price = term.Price;
                    var termId = term.TermId;
                    var discount = await DiscountPayment(user.Id, price);
                    // نکته ای که باید تست کنم اگر فرد هیج کس عضو نکنه مقدار چی میاد؟
                    var payment = new T_Payment
                    {
                        UserPaidId = user.Id,
                        Message = "درحال پرداخت شهریه",
                        Amount = price,
                        PayWithDiscount = (long)discount.Data,
                        Description = "Zibal Payment",
                        IsSuccess = false,
                        TermId = termId,
                        CreatedAt = DateTime.UtcNow,
                    };
                    _context.Add(payment);
                    await _context.SaveChangesAsync();

                    var success = new ResponseVM
                    {
                        Message = "ترم با موفقیت ثبت شد",
                        isSuccess = true,
                        StatusCode = StatusCodes.Status200OK,
                        Data = payment,
                    };
                    return success;
                }
                else
                {
                    var error = new ResponseVM
                    {
                        Message = "ترمی برای پرداخت یافت نشد",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return error;
                }
            }

            else
            {
                var error = new ResponseVM
                {
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return error;
            }


            #endregion
        }

        public async Task<ResponseVM> UpdatePayment(string id, bool isSuccess, string message, string status)
        {
            if (string.IsNullOrEmpty(id))
            {
                var error = new ResponseVM
                {
                    Message = "There is a problem with parameter.",
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Data = null
                };
                return error;

            }
            var payment = await _context.payments.FirstOrDefaultAsync(p => p.UserPaidId == id);
            if (payment == null)
            {
                return new ResponseVM
                {
                    isSuccess = false,
                    Message = "پرداخت یافت نشد"
                };
            }
            payment.IsSuccess = isSuccess;
            payment.PaidAt = DateTime.Now;
            payment.Message = message;
            payment.paymentStatusCode = status;
            var term = await _context.studentTerms.Where(c => c.TermId == payment.TermId && c.IsActive == false).FirstOrDefaultAsync();
            if (payment.IsSuccess == false)
            {
                term.IsActive = false;

            }
            else
            {
                term.IsActive = true;

            }
            if (payment != null)
            {
                _context.Update(payment);
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {

                    isSuccess = true

                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {

                    isSuccess = false

                };
                return error;
            }
        }


        public async Task UpdateTrackIdAndOrderId(T_Payment payment, long trackId, string orderId)
        {
            payment.TrackId = trackId;
            payment.OrderId = orderId;
            await _context.SaveChangesAsync();
        }


        private async Task<ResponseVM> DiscountPayment(string studentId, long termPrice)
        {
            var user = await _context.Users.Include(c => c.InvitedUsers) // level 1
                .ThenInclude(c => c.InvitedUsers) // level 2
                .ThenInclude(c => c.InvitedUsers) // level 3
                .ThenInclude(c => c.InvitedUsers) // level 4
                .FirstOrDefaultAsync(c => c.Id == studentId); // root user

            

            var discountLevels = await _context.discount.ToListAsync();
            var maxDiscount = await _context.GlobalConfig.FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.InvitedUsers.Count > 3)
                {
                    int level1 = user.InvitedUsers.Count();
                    int level2 = user.InvitedUsers.Sum(c => c.InvitedUsers.Count());
                    int level3 = user.InvitedUsers.Sum(c => c.InvitedUsers.Sum(c => c.InvitedUsers.Count()));
                    int level4 = user.InvitedUsers.Sum(c => c.InvitedUsers.Sum(c => c.InvitedUsers.Sum(c => c.InvitedUsers.Count())));
                    var discountDic = discountLevels.ToDictionary(c => c.Level, d => d.DiscountPercent);
                    double dis1 = discountDic.GetValueOrDefault(1);
                    double dis2 = discountDic.GetValueOrDefault(2);
                    double dis3 = discountDic.GetValueOrDefault(3);
                    double dis4 = discountDic.GetValueOrDefault(4);
                    double discount = (level1 * dis1) + (level2 * dis2) + (level3 * dis3) + (level4 * dis4);
                    double discountLevel1 = level1 * dis1;
                    double discountLevel2 = level2 * dis2;
                    double discountLevel3 = level3 * dis3;
                    double discountLevel4 = level4 * dis4;
                    var discountPercent = Math.Min(discount, maxDiscount.MaxGlobalDiscountPercent);
                    var finalAmount = (long)Math.Round(termPrice * (1 - (discountPercent / 100)));
                    var success = new ResponseVM
                    {
                        Data = finalAmount
                    };
                    string discountText = $"discount = {success.Data}";

                    //TotalDiscount = discount,
                    //        Level1 = discountLevel1,
                    //        Level2 = discountLevel2,
                    //        Level3 = discountLevel3,
                    //        Level4 = discountLevel4
                    Console.WriteLine("///////////////////////////////////////////////////////////////////////////");
                    Console.WriteLine(discountText);
                    return success;
                }
                else
                {
                    var success = new ResponseVM
                    {
                        Message = "برای دریافت تخفیف باید بیشتر از 3 نفر عضو داشته باشد",
                        Data = termPrice
                    };
                    return success;

                }
            }
            else
            {
                var error = new ResponseVM
                {
                    Data = null
                };
                return error;
            }

        }

    }
}
