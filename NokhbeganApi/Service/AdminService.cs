using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Context;
using NokhbeganApi.Model;
using NokhbeganApi.Repository;
using System.Security.Claims;

namespace NokhbeganApi.Service
{
    public class AdminService : IAdmin
    {
        private readonly NokhbeganDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<T_CustomUser> _userManager;

        public AdminService(NokhbeganDbContext context, IMapper mapper, UserManager<T_CustomUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<ResponseVM> StudentNotification(string studentId, CreateNotificationVM model)
        {
            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
            {
                return new ResponseVM
                {
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var notification = new T_Notification
            {
                Title = model.Title,
                Message = model.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                T_User_ID = student.Id,
            };
            _context.Add(notification);
            await _context.SaveChangesAsync();
            var success = new ResponseVM
            {
                Message = "اعلان با موفقیت ثبت شد.",
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = null
            };
            return success;
        }

        public async Task<ResponseVM> GetAllVerifyAsync(int page, int pageSize)
        {
            //var users = await _context.Users.Include(u => u.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED))
            //    .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED))
            //    .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED))
            //    .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED))
            //    .Where(c => c.InvitedById == null && c.Status == StudentStatus.VERIFIED).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            // har 2 ravesh ok bara data larg va perfomance raveshe payin behtare
            var allUsers = await _context.Users.
                Where(c => c.InvitedById == null && c.Status == StudentStatus.VERIFIED)
                .OrderBy(c => c.InvitedDate)
                .Select(c => new GetAllStudentsAdminVM
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Status = c.Status.ToString(),
                    PhoneNumber = c.PhoneNumber,
                    ReferralCode = c.ReferralCode,
                    InvitedUsers = c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)
                    .OrderBy(c => c.InvitedDate)
                    .Select(c => new GetAllStudentsAdminVM
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Status = c.Status.ToString(),
                        PhoneNumber = c.PhoneNumber,
                        ReferralCode = c.ReferralCode,
                        InvitedUsers = c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)
                        .OrderBy(c => c.InvitedDate)
                        .Select(c => new GetAllStudentsAdminVM
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Status = c.Status.ToString(),
                            PhoneNumber = c.PhoneNumber,
                            ReferralCode = c.ReferralCode,
                            InvitedUsers = c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)
                            .OrderBy(c => c.InvitedDate)
                            .Select(c => new GetAllStudentsAdminVM
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Status = c.Status.ToString(),
                                PhoneNumber = c.PhoneNumber,
                                ReferralCode = c.ReferralCode,
                                InvitedUsers = c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)
                            .OrderBy(c => c.InvitedDate)
                            .Select(c => new GetAllStudentsAdminVM
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Status = c.Status.ToString(),
                                PhoneNumber = c.PhoneNumber,
                                ReferralCode = c.ReferralCode,
                                InvitedUsers = new List<GetAllStudentsAdminVM>()
                            }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            if (allUsers.Count != 0)
            {
                var final = _mapper.Map<IEnumerable<GetAllStudentsAdminVM>>(allUsers);
                var success = new ResponseVM
                {
                    Message = "زبان آموز ها و زیرمجموعه های تایید شده با موفقیت لود شدن",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new { response = final }
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "زبان آموزان یافت نشدند",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }

        }
        public async Task<ResponseVM> GetAllPendingAsync()
        {
            var users = await _context.Users.Where(c => c.Status == StudentStatus.PENDING).ToListAsync();
            var students = new List<T_CustomUser>();
            foreach (var user in users)
            {
                var userRole = await _userManager.GetRolesAsync(user);

                if (!userRole.Contains("admin"))
                {
                    students.Add(user);
                }
            }
            if (students.Count != 0)
            {
                var final = _mapper.Map<IEnumerable<GetAllStudentsAdminVM>>(students);
                var success = new ResponseVM
                {
                    Message = "زبان آموزان درحال برسی با موفقیت لود شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new { response = final }
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "زبان آموزان یافت نشدند",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }

        }
        public async Task<ResponseVM> GetTermTime(int page,int size,string studentId)
        {
            var studentTerm = await _context.studentTerms.Where(c => c.UserId == studentId).Skip((page - 1) * page).Take(size).ToListAsync();
            if (!studentTerm.Any())
            {

                var error = new ResponseVM
                {
                    Message = "ترمی برای کاربر ثبت نشده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }
            var result = _mapper.Map<IEnumerable<TermInfoAdminVM>>(studentTerm);
            var success = new ResponseVM
            {
                Message = "ترم های کاربر با موفقیت یافت شد",
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new { response = result }
            };
            return success;
        }
        public async Task<ResponseVM> StudentTerms(string studentId, StudentTermVM term)
        {
            var student = await _context.Users.FindAsync(studentId);
            var hasActiveTerm = await _context.studentTerms
                .AnyAsync(t => t.UserId == studentId && t.EndedAt >= DateTime.UtcNow);
            if (!hasActiveTerm)
            {
                if (student == null)
                {
                    return new ResponseVM
                    {
                        Message = "کاربر یافت نشد",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                if (student.Status == StudentStatus.PENDING)
                {
                    return new ResponseVM
                    {
                        Message = "خطا ابتدا کاربر باید وریفای شود",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                if (term == null)
                {
                    var error = new ResponseVM
                    {
                        Message = "مشکل در اطلاعات ترم زبان آموز",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                    return error;
                }
                term.StartedAt = new DateTime(term.StartedAt.Year, term.StartedAt.Month, term.StartedAt.Day, 0, 0, 0, DateTimeKind.Utc);
                term.EndedAt = new DateTime(term.EndedAt.Year, term.EndedAt.Month, term.EndedAt.Day, 0, 0, 0, DateTimeKind.Utc);
                if (term.EndedAt <= term.StartedAt)
                {
                    var error = new ResponseVM
                    {
                        Message = "پایان ترم باید بعد از شروع ترم باشد",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return error;
                }
                var mappedTerm = _mapper.Map<T_StudentTerm>(term);
                if (mappedTerm.Price <= 0)
                {
                    var error = new ResponseVM
                    {
                        Message = "مبلغ ورودی اشتباه است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                mappedTerm.IsActive = true;
                mappedTerm.UserId = student.Id;
                _context.Add(mappedTerm);
                await _context.SaveChangesAsync();
                await AddPayment(studentId, term.PaymentType,mappedTerm.Price,mappedTerm.TermId);
                var success = new ResponseVM
                {
                    Message = "ترم با موفقیت ثبت شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;

            }
            else
                {
                var error = new ResponseVM
                {
                    Message = "ترم فعالی برای این کاربر در جریان است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return error;
            }
        }

        public async Task<ResponseVM> AdminInfo(string adminId, string userRole)
        {

            if (string.IsNullOrEmpty(adminId))
            {
                return new ResponseVM()
                {
                    Message = ".بازیابی اطلاعات کاربر امکان‌پذیر نیست",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var adminExist = await _userManager.FindByIdAsync(adminId);
            if (adminExist == null)
            {
                return new ResponseVM()
                {
                    Message = ".کاربری با این شناسه وجود ندارد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }
            if (adminExist != null && userRole == "admin")
            {
                var final = _mapper.Map<AdminInfoVM>(adminExist);
                var success = new ResponseVM
                {
                    Message = "ادمین با موفقیت پیدا شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new { response = final }
                };

                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "ادمین یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return error;
            }
        }

        public async Task<ResponseVM> CreateDiscountValue(DiscountLevelVM discountLevel)
        {
            if (discountLevel.Level < 1 || discountLevel.Level > 4)
            {
                var error = new ResponseVM
                {
                    Message = "خارج از محدوده انتخابی سطح باید بین 1 تا 4 باشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return error;
            }
            var levelExist = await _context.discount.FirstOrDefaultAsync(c => c.Level == discountLevel.Level);
            string message;
            if (levelExist != null)
            {
                levelExist.DiscountPercent = discountLevel.DiscountPercent;
                _context.Update(levelExist);
                message = "درصد تخفیف با موفقیت به‌روزرسانی شد";
            }
            else
            {
                var discount = _mapper.Map<T_InvitationLevelDiscount>(discountLevel);
                _context.discount.Add(discount);
                message = "سطح تخفیف جدید با موفقیت ثبت شد";

            }
            await _context.SaveChangesAsync();
            var result = new ResponseVM
            {
                Message = message,
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK
            };
            return result;

        }

        public async Task<ResponseVM> UpdateDiscountValue(int level, DiscountLevelVM discountLevel)
        {
            var discount = await _context.discount.FirstOrDefaultAsync(c => c.Level == level);
            if (discount != null)
            {
                discount.DiscountPercent = discount.DiscountPercent;
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {
                    Message = "سطح با موفقیت آپدیت شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            var error = new ResponseVM
            {
                Message = "سطح یافت نشد",
                isSuccess = false,
                StatusCode = StatusCodes.Status404NotFound
            };
            return error;
        }

        public async Task<ResponseVM> DeleteDiscountValue(int level)
        {
            var discount = await _context.discount.FirstOrDefaultAsync(c => c.Level == level);
            if (discount != null)
            {
                _context.discount.Remove(discount);
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {
                    Message = "سطح با موفقیت حذف شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
            }
            var error = new ResponseVM
            {
                Message = "سطح یافت نشد",
                isSuccess = false,
                StatusCode = StatusCodes.Status404NotFound
            };
            return error;

        }

        public async Task<ResponseVM> UpdateTerm(Guid termId, StudentTermUpdate term)
        {
            var termUpdate = await _context.studentTerms.FirstOrDefaultAsync(c => c.TermId == termId);
            if (termUpdate != null)
            {
                if (term.EndedAt <= term.StartedAt)
                {
                    var error = new ResponseVM
                    {
                        Message = "پایان ترم باید بعد از شروع ترم باشد",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return error;
                }
                _mapper.Map(term, termUpdate);
                termUpdate.StartedAt = new DateTime(term.StartedAt.Year, term.StartedAt.Month, term.StartedAt.Day, 0, 0, 0, DateTimeKind.Utc);
                termUpdate.EndedAt = new DateTime(term.EndedAt.Year, term.EndedAt.Month, term.EndedAt.Day, 0, 0, 0, DateTimeKind.Utc);
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {
                    Message = "ترم با موفقیت به روز رسانی شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "ترم یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> DeleteTerm(Guid termId)
        {
            var termUpdate = await _context.studentTerms.FirstOrDefaultAsync(c => c.TermId == termId);
            if (termUpdate != null)
            {
                _context.Remove(termUpdate);
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {
                    Message = "ترم با موفقیت حذف شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            var error = new ResponseVM
            {
                Message = "ترم یافت نشد",
                isSuccess = false,
                StatusCode = StatusCodes.Status404NotFound,
                Data = null
            };
            return error;
        }

        public async Task<ResponseVM> AddMaxDiscount(double maxPercent)
        {
            var existingMaxPercent = await _context.GlobalConfig.FirstOrDefaultAsync();
            if (existingMaxPercent != null)
            {
                // به‌روزرسانی رکورد موجود
                existingMaxPercent.MaxGlobalDiscountPercent = maxPercent;
                _context.Update(existingMaxPercent);

            }
            else
            {


                var max = new T_GlobalInvitationConfig
                {
                    MaxGlobalDiscountPercent = maxPercent
                };
                _context.Add(max);

            }
            await _context.SaveChangesAsync();
            var success = new ResponseVM
            {
                Message = "سقف تخفیف با موفقیت ثبت یا به‌روزرسانی شد",
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = null
            };
            return success;

        }

        public async Task<ResponseVM> GetAllFilterAsync(string? search)
        {
            var users = await _context.Users
            .Where(c => string.IsNullOrWhiteSpace(search) ||
                        c.FirstName.Contains(search) ||
                        c.LastName.Contains(search) ||
                        c.NationalId.Contains(search) ||
                        c.PhoneNumber.Contains(search))
            .ToListAsync();
            var usersMapped = _mapper.Map<IEnumerable<StudentInfoFiltered>>(users);
            if (users.Any())
            {
                var success = new ResponseVM
                {
                    Message = "لیست کاربران با موفقیت لود شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = usersMapped
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "کاربرانی یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> ShowDiscount()
        {
            var discountLevels = await _context.discount.ToListAsync();
            var discountMapped = _mapper.Map<IEnumerable<ShowDiscountLevelVM>>(discountLevels);
            if (discountLevels.Any())
            {
                var success = new ResponseVM
                {
                    Message = "تخفیف ها با موفقیت لود شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = discountMapped
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "تخفیف هایی یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> ShowMaxDiscount()
        {
            var maxDiscount = await _context.GlobalConfig.FirstOrDefaultAsync();
            var maxDiscountMapped = _mapper.Map<ShowGlobalConfigVM>(maxDiscount);
            if (maxDiscount != null)
            {
                var success = new ResponseVM
                {
                    Message = "تخفیف ها با موفقیت لود شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = maxDiscountMapped
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "تخفیف هایی یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
        }

        private async Task<ResponseVM> AddPayment(string studentId, string paymentType,long price,Guid termId)
        {
            if (studentId != null && paymentType != null)
            {
               
                var discount = await DiscountPayment(studentId, price);
                var StudentPayment = new T_Payment
                {
                    Amount = price,
                    AmountWithDiscount = (long)discount.Data,
                    IsPaid = true,
                    UserPaidId = studentId,
                    TermId = termId,
                    PaidAt = DateTime.Now,
                    PaymentType = paymentType
                };
                _context.Add(StudentPayment);
                await _context.SaveChangesAsync();
                var success = new ResponseVM
                {
                    Message = "پرداخت با موفقیت ثبت شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "مشکل در ارسال داده",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> GetStudentPayments(int page,int size,string studentId)
        {
            var payments = await _context.payments.Where(c => c.UserPaidId == studentId).Skip((page - 1) * size).Take(size).ToListAsync();
            var mappedPayment = _mapper.Map<IEnumerable<ShowStudentPaymentsVM>>(payments);
            if (payments.Any())
            {
                var success = new ResponseVM
                {
                    Message = "پرداختی ها با موفقیت لود شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = mappedPayment
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "پرداختی یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
                return error;
            }


        }


        private async Task<ResponseVM> DiscountPayment(string studentId, long termPrice)
        {
            var user = await _context.Users.Include(c => c.InvitedUsers.Where(c=> c.Status == StudentStatus.VERIFIED)) // level 1
                .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED))// level 2
                .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)) // level 3
                .ThenInclude(c => c.InvitedUsers.Where(c => c.Status == StudentStatus.VERIFIED)) // level 4
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

