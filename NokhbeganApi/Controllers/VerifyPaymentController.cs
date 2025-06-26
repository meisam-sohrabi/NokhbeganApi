using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NokhbeganApi.Repository;
using Microsoft.AspNetCore.Identity;
using NokhbeganApi.Model;
using NokhbeganApi.Customized;

namespace NokhbeganApi.Controllers
{
    public class VerifyPaymentController : Controller
    {
        private readonly IPay _pay;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<T_CustomUser> _userManager;
        private readonly ILogger<VerifyPaymentController> _logger;

        public VerifyPaymentController(ILogger<VerifyPaymentController> logger, IPay pay, IHttpClientFactory httpClientFactory, UserManager<T_CustomUser> userManager)
        {
            _pay = pay;
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _logger = logger;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        #region VerifyPayment
        //[HttpGet]
        //public async Task<IActionResult> Verify(string status, string trackId, string orderId)
        //{


        //    var payment = await _pay.GetPayment(orderId);

        //    if (payment == null)
        //        return BadRequest("پرداخت یافت نشد.");

        //    long.TryParse(trackId, out var longTrackid);
        //    var newTrackid = longTrackid;
        //    if (!string.IsNullOrWhiteSpace(status) && (!string.IsNullOrWhiteSpace(trackId) && (!string.IsNullOrWhiteSpace(orderId))))
        //    {
        //        var Data = new
        //        {
        //            merchant = "zibal",
        //            trackId = newTrackid,
        //        };
        //        var jsonString = JsonConvert.SerializeObject(Data);
        //        var client = _httpClientFactory.CreateClient();
        //        var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.zibal.ir/v1/verify");
        //        var content = new StringContent(jsonString, null, "application/json");
        //        request.Content = content;
        //        try
        //        {
        //            var response = await client.SendAsync(request);
        //            response.EnsureSuccessStatusCode();
        //            var responseString = await response.Content.ReadAsStringAsync();
        //            //var paymentResponses = JsonConvert.DeserializeObject<PaymentViewModel>(responseString);
        //            var jsonObject = JObject.Parse(responseString);
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                Console.WriteLine($"Verification failed with status code: {response.StatusCode}");
        //                return BadRequest("Payment verification failed.");
        //            }
        //            jsonObject.TryGetValue("result", out var resultValue);
        //            var resultResponse = resultValue.ToString();
        //            var statusMessage = ZibalPaymentStatusHelper.GetStatusMessage(status);
        //            //var verifyMessage = ZibalPaymentStatusHelper.GetVerifyResultMessage(resultResponse);
        //            if (resultResponse == "100")
        //            {
        //                var message = "پرداخت با موفقیت تأیید شد.";
        //                var isSuccess = true;
        //                await _pay.UpdatePayment(orderId, isSuccess, message, statusMessage);

        //                return View(payment.Data);

        //            }
        //            else if (resultResponse == "202")
        //            {
        //                var message = "خطا در تایید پرداخت";
        //                var isSuccess = false;
        //                await _pay.UpdatePayment(orderId, isSuccess, message, statusMessage);
        //                return View(payment.Data);
        //            }
        //            else
        //            {

        //                var message = "خطا در تایید پرداخت";
        //                var isSuccess = false;
        //                await _pay.UpdatePayment(orderId, isSuccess, message, statusMessage);
        //                return View(payment.Data);

        //            }
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            // هندل قطع بودن اینترنت یا DNS failure
        //            Console.WriteLine("Connection of payment gateway error: " + ex.Message);
        //            return BadRequest("در اتصال به درگاه پرداخت مشکلی پیش آمد. لطفاً اتصال اینترنت خود را بررسی کرده و دوباره تلاش کنید.");
        //        }
        //        catch (Exception ex)
        //        {
        //            // سایر خطاها
        //            Console.WriteLine("General error : " + ex.Message);
        //            return StatusCode(StatusCodes.Status500InternalServerError, "خطایی در سرور رخ داده است.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}
        #endregion

    }
}
