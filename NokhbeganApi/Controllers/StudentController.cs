using NokhbeganApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NokhbeganApi.Repository;
using System.Security.Claims;

namespace NokhbeganApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        #region DI
        private readonly IStudent _student;
        private readonly ILogger<AuthController> _logger;
        public StudentController(IStudent student, ILogger<AuthController> logger)
        {
            _student = student;
            _logger = logger;
        }
        #endregion

        #region Tree
        [HttpGet("StudentTree")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var student = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _student.GetAllAsync(student);
                if (result.isSuccess)
                {

                    return Ok(result);
                }
                else
                {

                    return BadRequest(result);
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred.");

                var error = new ResponseVM
                {
                    Message = "خطای سرور رخ داده است. اگر مشکل ادامه داشت، لطفاً با پشتیبانی تماس بگیرید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                };

                return BadRequest(error);
            }
        }
        #endregion

        #region UserInfo
        [HttpGet("UserInfo")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> StudentInfo()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                    var result = await _student.UserInfo(studentId);
                    if (result.isSuccess)
                    {

                        return Ok(result);
                    }
                    else
                    {

                        return BadRequest(result);
                    };
                }
                else
                {

                    var error = new ResponseVM
                    {
                        Message = "کاربر لاگین نکرده است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                    return Unauthorized(error);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred.");

                var error = new ResponseVM
                {
                    Message = "خطای سرور رخ داده است. اگر مشکل ادامه داشت، لطفاً با پشتیبانی تماس بگیرید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                };

                return BadRequest(error);
            }

        }
        #endregion

        #region GetTerm

        [HttpGet("TermInfo")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> GetTermInfo()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var result = await _student.GetTermTime(studentId);
                    if (result.isSuccess)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                else
                {

                    var error = new ResponseVM
                    {
                        Message = "کاربر لاگین نکرده است",
                        isSuccess = false,
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                    return Unauthorized(error);

                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "an error occurred.");

                var error = new ResponseVM
                {
                    Message = "خطای سرور رخ داده است. اگر مشکل ادامه داشت، لطفاً با پشتیبانی تماس بگیرید",
                    isSuccess = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                };

                return BadRequest(error);
            }
        }
        #endregion

    }
}
