using AutoMapper;
using NokhbeganApi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Context;
using NokhbeganApi.Repository;

namespace NokhbeganApi.Service
{
    public class StudentService : IStudent
    {
        private readonly NokhbeganDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<T_CustomUser> _userManager;

        public StudentService(NokhbeganDbContext context, IMapper mapper, UserManager<T_CustomUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ResponseVM> GetAllAsync(string? studentId = null)
        {
            var users = await _context.Users.Where(s => s.Id == studentId).OrderBy(c => c.InvitedDate).Include(s => s.InvitedUsers)
                .ThenInclude(c => c.InvitedUsers)
                .ThenInclude(c => c.InvitedUsers)
                .ThenInclude(c => c.InvitedUsers)
                .ToListAsync();
            if (users.Count != 0)
            {
                var final = _mapper.Map<IEnumerable<GetAllStudentsVM>>(users);
                var success = new ResponseVM
                {
                    Message = "زبان آموز و زیرمجموعه ها با موفقیت لود شد",
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
                    Message = "زبان آموزی یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }

        }
        public async Task<ResponseVM> GetTermTime(string studentId)
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
            if (student.Status == StudentStatus.PENDING)
            {
                return new ResponseVM
                {
                    Message = "عدم دسترسی ، لطفا منتظر تایید نهایی از مدیر آموزشگاه باشید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            var userTerm = await _context.studentTerms.Where(c => c.UserId == studentId && c.StartedAt <= DateTime.Now && c.EndedAt >= DateTime.UtcNow).FirstOrDefaultAsync();
            if (userTerm == null)
            {
                var error = new ResponseVM
                {
                    Message = "هیج گونه ترم فعالی وجود نداره",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }
            var result = _mapper.Map<TermInfo>(userTerm);

            var success = new ResponseVM
            {
                Message = "ترم آموزشی با موفقیت یافت شد",
                isSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                Data = new { response = result }
            };
            return success;

        }
        public async Task<ResponseVM> UserInfo(string studentId)
        {
            var student = await _userManager.FindByIdAsync(studentId);
            if (student != null)
            {
                var final = _mapper.Map<StudentInfoVM>(student);

                if (student.Status == StudentStatus.PENDING)
                {
                    return new ResponseVM()
                    {
                        Message = "عدم دسترسی ، لطفا منتظر تایید نهایی از مدیر آموزشگاه باشید",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var success = new ResponseVM
                {
                    Message = "اطلاعات کاربر با موفقیت یافت شد",
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
                    Message = "کاربر یافت نشد",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status404NotFound
                };
                return error;
            }
        }
    }
}
