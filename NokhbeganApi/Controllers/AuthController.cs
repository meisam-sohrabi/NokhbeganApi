using NokhbeganApi.Model;
using NokhbeganApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace NokhbeganApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region DI
        private readonly IUser _user;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUser user, ILogger<AuthController> logger)
        {
            _user = user;
            _logger = logger;
        }
        #endregion

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterVM register)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _user.RegisterAsync(register);
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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }

            }
            else
            {
                return BadRequest("برخی از ورودی ها نامعتبر هستند");
            }

        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var result = await _user.LoginAsync(login);

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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }


            }
            else
            {
                return BadRequest("some properties are not valid ");
            }

        }

        #endregion

        #region NationalIdCheck
        [HttpPost("NationalIdCheck")]
        public async Task<IActionResult> NationalIdCheck(CheckNationalIdVM checkNationalId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _user.CheckNationalIdAsync(checkNationalId);
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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }
            }
            else
            {
                return BadRequest("some properties are not valid ");
            }

        }
        #endregion

        //#region UpdatePartial

        //[HttpPatch("Update")]
        //[Authorize(Roles = "student")]
        //public async Task<IActionResult> UpdatePartial(JsonPatchDocument<UserInfoUpdateVM> update)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //            if (studentId == null)
        //            {
        //                return Unauthorized();
        //            }

        //            var result = await _user.UpdatePartialAsync(studentId, update);

        //            if (result.isSuccess)
        //            {
        //                return Ok(result);
        //            }
        //            else
        //            {
        //                return BadRequest(result);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "an error occurred.");

        //            var error = new ResponseVM
        //            {
        //                Message = "خطای سرور رخ داده است. اگر مشکل ادامه داشت، لطفاً با پشتیبانی تماس بگیرید",
        //                isSuccess = false,
        //                StatusCode = StatusCodes.Status500InternalServerError
        //            };

        //            return BadRequest(error);
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("some properties are not valid ");
        //    }


        //}
        //#endregion

        #region Update

        [HttpPut("Update")]
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> Update(UserInfoUpdateVM update)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (studentId == null)
                    {
                        return Unauthorized();
                    }

                    var result = await _user.Update(studentId, update);

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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }
            }
            else
            {
                return BadRequest("some properties are not valid ");
            }
        }

        #endregion

        #region UpdateV2
        [HttpPut("v2/Update")]
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> UpdateV2(UserInfoUpdateV2Class update)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (studentId == null)
                    {
                        return Unauthorized();
                    }

                    var result = await _user.UpdateV2(studentId, update);

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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }
            }
            else
            {
                return BadRequest("some properties are not valid ");
            }
        }

        #endregion

        #region Uploading Image
        [HttpPut("UploadImage")]
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var result = await _user.UploadImage(file);

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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }
            }
            else
            {
                return BadRequest("some properties are not valid ");
            }
        }
        #endregion


        #region PasswordChanging

        [HttpPost("PasswordChanging")]
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> PassChange(ChangePasswordVM passChange)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (studentId == null)
                    {
                        return Unauthorized();
                    }

                    var result = await _user.ChangePass(studentId, passChange);

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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }
            }
            else
            {
                return BadRequest("some properties are not valid ");
            }
        }

        #endregion

        #region Approve
        [HttpPost("Approval")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StudentApproval(string userId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _user.ApproveUser(userId);
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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }

            }
            else
            {
                return BadRequest("some properties are not valid ");
            }

        }
        #endregion

        #region NotificationIcon
        [HttpGet("NotificationIcon")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> NotificationIcon()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {


                    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    var result = await _user.NotificationIcon(adminId);
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
        #region Notification
        [HttpGet("Notification")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> Notification(string userId, string status)
        {
            try
            {

                var result = await _user.Notification(userId, status);
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

        #region NotificationDetail
        [HttpGet("NotificationDetail")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> NotificationDetail(Guid id)
        {
            try
            {

                var result = await _user.NotificationDetail(id);
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

        #region Reject
        [HttpPost("Rejection")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StudentRejection(string userId)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _user.RejectUser(userId);
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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }

            }
            else
            {
                return BadRequest("some properties are not valid ");
            }

        }
        #endregion
        #region logindata
        [HttpGet("logindataresult")]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> LoginDataResult()
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (User.Identity.IsAuthenticated)
                    {


                        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var userRole = User.FindFirstValue(ClaimTypes.Role);

                        var result = await _user.LoginDataResult(studentId, userRole);
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
                        StatusCode = StatusCodes.Status500InternalServerError
                    };

                    return BadRequest(error);
                }

            }
            else
            {
                return BadRequest("some properties are not valid ");
            }


            #endregion

        }
    }
}