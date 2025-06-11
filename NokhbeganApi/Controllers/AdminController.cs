using NokhbeganApi.Model;
using Microsoft.AspNetCore.Mvc;
using NokhbeganApi.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace NokhbeganApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        #region DI 
        private readonly IAdmin _admin;
        private readonly ILogger<AuthController> _logger;
        public AdminController(IAdmin admin, ILogger<AuthController> logger)
        {
            _admin = admin;
            _logger = logger;
        }
        #endregion

        #region AllVerifyStudents
        [HttpGet("AllVerifyStudents")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllStudents(int page, int pageSize)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    page = 1;
                    pageSize = 1;
                }
                var result = await _admin.GetAllVerifyAsync(page, pageSize);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region AllPending
        [HttpGet("PendingStudents")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllPendingStudents()
        {
            try
            {

                var result = await _admin.GetAllPendingAsync();
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region GetAllFilter
        [HttpGet("AllFilterStudents")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllFilterStudents(string? search)
        {
            try
            {
             
                var result = await _admin.GetAllFilterAsync(search);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region AddTerm

        [HttpPost("AddStudentTerm")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StudentTerm(string userId, StudentTermVM term)
        {
            try
            {

                var result = await _admin.StudentTerms(userId, term);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        [HttpGet("GetStudentTerms")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetTermInfo(string userId)
        {
            try
            {

                var result = await _admin.GetTermTime(userId);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return Ok(result);
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

        #region UpdateTerm

        [HttpPost("UpdateTerm")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTerm(Guid termId, StudentTermUpdate term)
        {
            try
            {

                var result = await _admin.UpdateTerm(termId, term);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region DeleteTerm
        [HttpPost("DeleteTerm")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTerm(Guid TermId)
        {
            try
            {

                var result = await _admin.DeleteTerm(TermId);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region AddNotification
        [HttpPost("Notification")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StudentNotification(string userId, CreateNotificationVM notification)
        {
            try
            {

                var result = await _admin.StudentNotification(userId, notification);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region AdminInfo
        [HttpGet("AdminInfo")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminInfo()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {


                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userRole = User.FindFirstValue(ClaimTypes.Role);

                    var result = await _admin.AdminInfo(studentId, userRole);
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
        #region CreateDiscount
        [HttpPost("Discount")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateDiscount(DiscountLevelVM discount)
        {
            try
            {

                var result = await _admin.CreateDiscountValue(discount);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region ShowDisountLevel
        [HttpGet("ShowDiscountLevels")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ShowDiscountLevels()
        {
            try
            {

                var result = await _admin.ShowDiscount();
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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

        #region AddMaxDiscount

        [HttpPost("MaxDiscount")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddMaxDiscountPercent(double maxPercent)
        {
            try
            {

                var result = await _admin.AddMaxDiscount(maxPercent);
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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
        #region ShowMaxDiscount
        [HttpGet("ShowMaxDiscount")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ShowMaxDiscount()
        {
            try
            {

                var result = await _admin.ShowMaxDiscount();
                if (result.isSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
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
