using Microsoft.AspNetCore.Identity;
using NokhbeganApi.Model;
using NokhbeganApi.Repository;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using NokhbeganApi.Helper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Context;

namespace NokhbeganApi.Service
{
    public class UserService : IUser
    {

        private readonly UserManager<T_CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly NokhbeganDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(UserManager<T_CustomUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, NokhbeganDbContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }


        public async Task<ResponseVM> RegisterAsync(RegisterVM register)
        {
            var phoneExist = await CheckPhoneNumber(register.PhoneNumber);
            if (phoneExist == true)
            {
                var error = new ResponseVM
                {
                    Message = "شماره تماس قبلا استفاده شده",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
                return error;
            }
            var userReferralCode = ReferralCode.ReferralCodeGenerator();
            var identityUser = new T_CustomUser()
            {
                UserName = Guid.NewGuid().ToString(),
                FirstName = register.FirstName,
                LastName = register.LastName,
                NationalId = register.NationalId,
                PhoneNumber = register.PhoneNumber,
                ReferralCode = userReferralCode,
                Status = StudentStatus.PENDING
            };
            if (register.ImageUrl != null)
            {
                if (register.ImageUrl.Length == 0)
                {
                    var error = new ResponseVM
                    {
                        Message = "فایل آپلود شده خالی است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                    return error;
                }
                if (register.ImageUrl.Length > 1048576)
                {
                    var error = new ResponseVM
                    {
                        Message = "حجم فایل تصویر بیشتر از ۱ مگابایت است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                    return error;
                }
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(register.ImageUrl.FileName);
                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    return new ResponseVM
                    {
                        Message = "نوع فایل نامعتبر است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                }
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userimage");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var url = Path.Combine(uploadFolder, fileName);
                var urldb = $"https://nokhbegan.nahadak.ir/userimage/{fileName}";
                using (var fileStream = new FileStream(url, FileMode.Create))
                {
                    await register.ImageUrl.CopyToAsync(fileStream);
                }
                identityUser.ImageUrl = urldb;
            }
            if (!string.IsNullOrEmpty(register.ReferralCode))
            {
                var invitedBy = await GetStudentByCode(register.ReferralCode);
                if (invitedBy.InvitedUsers.Count >= 4)
                {
                    return new ResponseVM()
                    {
                        Message = "سقف استفاده از کد دعوت به پایان رسیده است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                }
                if (invitedBy == null)
                {
                    return new ResponseVM()
                    {
                        Message = "کد دعوت وارد شده معتبر نیست. لطفاً مجددا کد را برسی کرده و دوباره تلاش کنید",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        Data = null
                    };
                }

                identityUser.InvitedById = invitedBy.Id;
                identityUser.InvitedDate = DateTime.UtcNow;
            }
            var result = await _userManager.CreateAsync(identityUser, register.Password);
            if (!result.Succeeded)
            {
                return new ResponseVM()
                {
                    Message = "مشکلی در ایجاد کاربر رخ داده است",
                    StatusCode = StatusCodes.Status400BadRequest,
                    isSuccess = false,
                    Data = null
                };
            }
            else
            {
                string studentRole = "student";

                if (!await _roleManager.RoleExistsAsync(studentRole))
                {
                    var userRole = new IdentityRole()
                    {
                        Name = studentRole
                    };
                    await _roleManager.CreateAsync(userRole);
                }
                await _userManager.AddToRoleAsync(identityUser, studentRole);


                return new ResponseVM()
                {
                    Message = "کاربر با موفقیت ایجاد شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
            }

        }
        public async Task<ResponseVM> LoginAsync(LoginVM login)
        {
            var userExist = await _userManager.Users.FirstOrDefaultAsync(c => c.NationalId == login.NationalId);

            if (userExist == null)
            {
                return new ResponseVM()
                {
                    Message = "کد ملی یا رمز عبور اشتباه است لطفا مجددا تلاش کنید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var result = await _userManager.CheckPasswordAsync(userExist, login.Password);
            if (!result)
            {
                return new ResponseVM()
                {
                    Message = "کد ملی یا رمز عبور اشتباه است لطفا مجددا تلاش کنید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var userRole = await _userManager.GetRolesAsync(userExist);

            var SecreteKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signingKey = new SigningCredentials(SecreteKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier,userExist.Id),
                new(ClaimTypes.Name, userExist.FirstName)
            };

            if (userRole != null)
            {
                foreach (var role in userRole)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var jwtConfig = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMonths(3),
                signingCredentials: signingKey

                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtConfig);
            return new ResponseVM()
            {
                Message = token,
                isSuccess = true,
                Role = userRole.FirstOrDefault(),
                StatusCode = StatusCodes.Status200OK,
                Data = null
            };
        }
        public async Task<ResponseVM> CheckNationalIdAsync(CheckNationalIdVM checkNationalId)
        {
            var userExist = await _userManager.Users.FirstOrDefaultAsync(c => c.NationalId == checkNationalId.NationalId);

            if (userExist == null)
            {
                return new ResponseVM()
                {
                    Message = "کاربری با این کد ملی یافت نشد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
            }
            else
            {
                return new ResponseVM()
                {
                    Message = "کاربر با همین کد ملی قبلا ثبت نام کرده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }


        }

        #region PartialUpdate
        //public async Task<ResponseVM> UpdatePartialAsync(string studentId, JsonPatchDocument<UserInfoUpdateVM> update)
        //{

        //    if (update == null)
        //    {
        //        return new ResponseVM()
        //        {
        //            Message = "No updates were provided.",
        //            isSuccess = false,
        //            StatusCode = StatusCodes.Status400BadRequest
        //        };
        //    }
        //    else
        //    {
        //        var student = await _userManager.FindByIdAsync(studentId);

        //        if (student != null)
        //        {

        //            var studentMapped = _mapper.Map<UserInfoUpdateVM>(student);

        //            update.ApplyTo(studentMapped);

        //            var validationResults = new List<ValidationResult>();
        //            var validationContext = new ValidationContext(studentMapped);

        //            bool isValid = Validator.TryValidateObject(studentMapped, validationContext, validationResults, true);

        //            if (!isValid)
        //            {
        //                return new ResponseVM()
        //                {
        //                    Message = "Validation failed for the updated data.",
        //                    isSuccess = false,
        //                    StatusCode = StatusCodes.Status400BadRequest,
        //                    Data = validationResults.Select(v => v.ErrorMessage)
        //                };
        //            }

        //            var studentPatched = _mapper.Map(studentMapped, student);
        //            await _context.SaveChangesAsync();

        //            var success = new ResponseVM
        //            {
        //                Message = $"User successfully updated .",
        //                isSuccess = true,
        //                StatusCode = StatusCodes.Status200OK,
        //                Data = new { response = _mapper.Map<UserInfoUpdateVM>(studentPatched) }
        //            };
        //            return success;

        //        }
        //        else
        //        {
        //            return new ResponseVM()
        //            {
        //                Message = "Student Not Found.",
        //                isSuccess = false,
        //                StatusCode = StatusCodes.Status404NotFound
        //            };
        //        }

        //    }

        //}

        #endregion

        public async Task<ResponseVM> Update(string studentId, UserInfoUpdateVM update)
        {
            if (update == null)
            {
                return new ResponseVM()
                {
                    Message = "به‌روزرسانی‌ای ارائه نشده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var student = await _userManager.FindByIdAsync(studentId);
            if (student != null)
            {
                student.FirstName = update.FirstName;
                student.LastName = update.LastName;
                student.PhoneNumber = update.PhoneNumber;
                await _userManager.UpdateAsync(student);

                var success = new ResponseVM
                {
                    Message = "کاربر با موفقیت به‌روزرسانی شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            else
            {
                return new ResponseVM()
                {
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }
        }
        public async Task<ResponseVM> ChangePass(string userId, ChangePasswordVM changePass)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new ResponseVM()
                {
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var result = await _userManager.ChangePasswordAsync(user, changePass.OldPassword, changePass.NewPassword);
            if (result.Succeeded)
            {
                var success = new ResponseVM
                {
                    Message = "رمز عبور با موفقیت تغییر یافت",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            else
            {
                return new ResponseVM()
                {
                    Message = "تغییر رمز عبور ناموفق بود",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
        }
        public async Task<T_CustomUser> GetStudentByCode(string code)
        {
            var student = await _context.Users.Include(c => c.InvitedUsers).FirstOrDefaultAsync(u => u.ReferralCode == code);
            return student;
        }
        public async Task<ResponseVM> ApproveUser(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return new ResponseVM()
                {
                    Message = ".شناسه کاربر باید ارائه شود",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };

            }
            var student = await _userManager.FindByIdAsync(studentId);

            if (student == null)
            {
                var error = new ResponseVM
                {
                    Message = ".کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
            if (student.Status == StudentStatus.VERIFIED)
            {
                var error = new ResponseVM
                {
                    Message = ".کاربر قبلاً تأیید شده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
            else
            {
                student.Status = StudentStatus.VERIFIED;
                await _userManager.UpdateAsync(student);


                var success = new ResponseVM
                {
                    Message = ".وضعیت کاربر با موفقیت به‌روزرسانی شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
        }
        public async Task<ResponseVM> RejectUser(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return new ResponseVM()
                {
                    Message = ".شناسه کاربر باید ارائه شود",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };

            }
            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null)
            {
                var error = new ResponseVM
                {
                    Message = ".کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
            if (student.Status == StudentStatus.REJECTED)
            {
                var error = new ResponseVM
                {
                    Message = ".کاربر قبلاً رد شده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
            else
            {
                student.Status = StudentStatus.REJECTED;
                await _userManager.UpdateAsync(student);
                var success = new ResponseVM
                {
                    Message = ".وضعیت کاربر با موفقیت به‌روزرسانی شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
        }
        public async Task<ResponseVM> LoginDataResult(string studentId, string role)
        {

            if (string.IsNullOrEmpty(studentId))
            {
                return new ResponseVM()
                {
                    Message = ".بازیابی اطلاعات کاربر امکان‌پذیر نیست",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var userExist = await _userManager.FindByIdAsync(studentId);
            if (userExist == null)
            {
                return new ResponseVM()
                {
                    Message = ".کاربری با این شناسه وجود ندارد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }
            if (userExist.Status == StudentStatus.PENDING)
            {
                return new ResponseVM()
                {
                    Message = ".کاربری با این شناسه وجود دارد اما در انتظار تأیید است",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new StudentInfoVM
                    {
                        NationalId = userExist.NationalId,
                        PhoneNumber = userExist.PhoneNumber,
                        FirstName = userExist.FirstName,
                        LastName = userExist.LastName,
                        Status = userExist.Status.ToString(),
                        ImageUrl = userExist.ImageUrl,
                        Role = role
                    }
                };
            }
            if (userExist.Status == StudentStatus.VERIFIED)
            {
                return new ResponseVM()
                {
                    Message = ".کاربری با این شناسه وجود دارد و تأیید شده است",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new StudentInfoVM
                    {
                        NationalId = userExist.NationalId,
                        PhoneNumber = userExist.PhoneNumber,
                        FirstName = userExist.FirstName,
                        LastName = userExist.LastName,
                        Status = userExist.Status.ToString(),
                        ReferralCode = userExist.ReferralCode,
                        ImageUrl = userExist.ImageUrl,
                        Role = role
                    }
                };
            }
            return new ResponseVM()
            {
                Message = ".خطای غیرمنتظره یا وضعیت پشتیبانی‌نشده",
                isSuccess = false,
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = null
            };
        }
        public async Task<ResponseVM> Notification(string studentId, string status)
        {
            var notification = await _context.notifications.Where(c => c.T_User_ID == studentId).ToListAsync();
            if (status == "All")
            {
                var notificationMapped = _mapper.Map<IEnumerable<ShowNotificationVM>>(notification);
                var success = new ResponseVM
                {
                    Message = "All notifications are retrieved successfully.",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = notificationMapped
                };
                return success;
            }
            else if (status == "Seen")
            {

                var seen = notification.Where(c => c.IsRead == true).ToList();
                var notificationMapped = _mapper.Map<IEnumerable<ShowNotificationVM>>(seen);
                var success = new ResponseVM
                {
                    Message = "Seen notifications are retrieved successfully.",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = notificationMapped
                };
                return success;
            }
            else if (status == "Unseen")
            {
                var unSeen = notification.Where(c => c.IsRead == false).ToList();
                var notificationMapped = _mapper.Map<IEnumerable<ShowNotificationVM>>(unSeen);
                var success = new ResponseVM
                {
                    Message = "Unseen notifications are retrieved successfully.",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = notificationMapped
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "Parameter is not appropriate.",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
                return error;
            }

        }

        public async Task<ResponseVM> NotificationIcon(string studentId)
        {

            var student = await _context.Users.FindAsync(studentId);
            if (studentId == null)
            {
                return new ResponseVM
                {
                    Message = "User not found.",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var userNotification = await _context.notifications.Where(c => c.T_User_ID == studentId && c.IsRead == false).ToListAsync();
            var notificationCount = userNotification.Count();
            var success = new ResponseVM
            {
                Message = "Notification icon is retrieved successfully.",
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new { icon = notificationCount, user = studentId }

            };
            return success;
        }

        public async Task<ResponseVM> NotificationDetail(Guid id)
        {
            var notification = await _context.notifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
                var notificationMapped = _mapper.Map<ShowNotificationVM>(notification);
                var success = new ResponseVM
                {
                    Message = "Notification is retrieved successfully.",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = new { notification = notificationMapped }
                };
                return success;
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "Notification not found.",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> UploadImage(IFormFile file)
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {

                var id = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    if (file != null)
                    {
                        if (file.Length > 1048576)
                        {
                            var error = new ResponseVM
                            {
                                Message = "حجم فایل تصویر بیشتر از ۱ مگابایت است",
                                isSuccess = false,
                                StatusCode = StatusCodes.Status400BadRequest,
                                Data = null
                            };
                            return error;
                        }
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(file.FileName);
                        if (!allowedExtensions.Contains(fileExtension.ToLower()))
                        {
                            return new ResponseVM
                            {
                                Message = "نوع فایل نامعتبر است",
                                isSuccess = false,
                                StatusCode = StatusCodes.Status400BadRequest,
                                Data = null
                            };
                        }
                        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userimage");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var url = Path.Combine(uploadFolder, fileName);
                        var urldb = $"https://nokhbegan.nahadak.ir/userimage/{fileName}";
                        using (var fileStream = new FileStream(url, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        user.ImageUrl = urldb;
                        await _userManager.UpdateAsync(user);
                        var success = new ResponseVM
                        {

                            Message = "تصویر با موفقیت ذخیره شد",
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
                            Message = "مشکل در ارسال فایل",
                            isSuccess = false,
                            StatusCode = StatusCodes.Status404NotFound,
                            Data = null
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
                        StatusCode = StatusCodes.Status404NotFound,
                        Data = null
                    };
                    return error;
                }
            }
            else
            {
                var error = new ResponseVM
                {
                    Message = "کاربر احراز هویت نشده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status403Forbidden,
                    Data = null
                };
                return error;
            }
        }

        public async Task<ResponseVM> UpdateV2(string studentId, UserInfoUpdateV2Class update)
        {
            if (update == null)
            {
                return new ResponseVM()
                {
                    Message = "به‌روزرسانی‌ای ارائه نشده است",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = null
                };
            }
            var student = await _userManager.FindByIdAsync(studentId);
            if (student != null)
            {
                if (update.file != null)
                {
                    if (update.file.Length > 1048576)
                    {
                        var error = new ResponseVM
                        {
                            Message = "حجم فایل تصویر بیشتر از ۱ مگابایت است",
                            isSuccess = false,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Data = null
                        };
                        return error;
                    }
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(update.file.FileName);
                    if (!allowedExtensions.Contains(fileExtension.ToLower()))
                    {
                        return new ResponseVM
                        {
                            Message = "نوع فایل نامعتبر است",
                            isSuccess = false,
                            StatusCode = StatusCodes.Status400BadRequest,
                            Data = null
                        };
                    }
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userimage");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var url = Path.Combine(uploadFolder, fileName);
                    var urldb = $"https://nokhbegan.nahadak.ir/userimage/{fileName}";
                    using (var fileStream = new FileStream(url, FileMode.Create))
                    {
                        await update.file.CopyToAsync(fileStream);
                    }
                    student.ImageUrl = urldb;

                }
                student.FirstName = update.FirstName;
                student.LastName = update.LastName;
                student.PhoneNumber = update.PhoneNumber;
                await _userManager.UpdateAsync(student);

                var success = new ResponseVM
                {
                    Message = "کاربر با موفقیت به‌روزرسانی شد",
                    isSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Data = null
                };
                return success;
            }
            else
            {
                return new ResponseVM()
                {
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Data = null
                };
            }
        }

        private async Task<bool> CheckPhoneNumber(string phoneNumber)
        {
            var user = await _context.Users.Where(c => c.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            if (user != null)
                return true;
            else
                return false;
        }
    }
}

