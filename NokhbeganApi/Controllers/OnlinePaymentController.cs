using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NokhbeganApi.Model;
using NokhbeganApi.Repository;
using System.Security.Claims;
using System.Net;

namespace NokhbeganApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnlinePaymentController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPay _pay;
        private readonly ISession _session;

        public OnlinePaymentController(IHttpClientFactory httpClientFactory, IPay pay)
        {
            _httpClientFactory = httpClientFactory;
            _pay = pay;
        }

        #region OnlinePay
        [Authorize]
        [HttpPost("onlinePay")]
        public async Task<IActionResult> Payment()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _pay.OnlinePay(id);
                var callBackServer = "https://nokhbegan.nahadak.ir/VerifyPayment/Verify/";
                // callbackserver dar server set shode.
                if (result.isSuccess)
                {
                    var payment = result.Data as T_Payment;
                    var data = new
                    {
                        merchant = "zibal",
                        amount = payment.PayWithDiscount,
                        callbackUrl = "https://localhost:7074/VerifyPayment/Verify/",
                        orderId = Guid.NewGuid().ToString().Substring(0, 12),
                        description = "Payment factor",

                    };
                    var jasonString = JsonConvert.SerializeObject(data);
                    var client = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.zibal.ir/v1/request/");
                    var content = new StringContent(jasonString, null, "application/json");
                    request.Content = content;
                    try
                    {
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var payload = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(payload);
                        jsonObject.TryGetValue("trackId", out var trackIdValue);
                        var trackId = trackIdValue;
                        long.TryParse(trackId.ToString(), out long parsedTrackId);
                        jsonObject.TryGetValue("result", out var resultIdValue);
                        var resultId = resultIdValue;
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Payment request failed with status code: {response.StatusCode}");
                            return BadRequest("Payment request failed.");
                        }
                        if (resultId.ToString() == "100" && trackId != null)
                        {
                            var paymentPage = $"https://gateway.zibal.ir/start/{trackId}";
                            HttpContext.Session.SetString("id", id);
                            await _pay.UpdateTrackIdAndOrderId(payment, parsedTrackId, data.orderId);
                            return Ok($"url is :{paymentPage}");
                        }
                        else
                        {
                            Console.WriteLine($"Payment request failed with status code: {response.StatusCode}");
                            return BadRequest("Payment request failed.");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        // هندل قطع بودن اینترنت یا DNS failure
                        Console.WriteLine("Connection to payment gateway error : " + ex.Message);
                        return BadRequest("در اتصال به درگاه پرداخت مشکلی پیش آمد. لطفاً اتصال اینترنت خود را بررسی کرده و دوباره تلاش کنید.");
                    }
                    catch (Exception ex)
                    {
                        // سایر خطاها
                        Console.WriteLine("General error: " + ex.Message);
                        return StatusCode(StatusCodes.Status500InternalServerError, payment.Message);
                    }
                }
                else
                {
                    Console.WriteLine("There is problem with term.");
                    return BadRequest("ترم فعالی ثبت نشده است");
                }
            }
            else
            {
                Console.WriteLine("User not authenticated.");
                return BadRequest("User not authenticated.");
            }
        }
        #endregion

        [HttpPost("inquirypayment")]
        [Authorize]
        public async Task<IActionResult> Inquiry(long trackId)
        {
            var data = new
            {
                merchant = "zibal",
                trackId
            };
            var jasonString = JsonConvert.SerializeObject(data);
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://gateway.zibal.ir/v1/inquiry");
            var content = new StringContent(jasonString, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(payload);
            return Ok(jsonObject);
        }
    }
}

