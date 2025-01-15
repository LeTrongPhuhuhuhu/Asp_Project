using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static _6TL.Controllers.HomeController;
using System.Text;
using Newtonsoft.Json;

public class MoMoController : Controller
{
	private readonly IConfiguration _configuration;

	public MoMoController(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[HttpPost]
	[Route("api/checkout/momo")]
	public async Task<IActionResult> CheckoutMoMo([FromBody] CartData cartData)
	{
		try
		{
			string endpoint = _configuration["MoMo:Endpoint"];
			string partnerCode = _configuration["MoMo:PartnerCode"];
			string accessKey = _configuration["MoMo:AccessKey"];
			string secretKey = _configuration["MoMo:SecretKey"];
			string orderInfo = "Thanh toan don hang"; // Không nên có dấu trong orderInfo
			string redirectUrl = _configuration["MoMo:ReturnUrl"];
			string ipnUrl = _configuration["MoMo:NotifyUrl"];
			string amount = Math.Round(cartData.totalAmount).ToString(); // MoMo không chấp nhận số thập phân
			string orderId = DateTime.Now.Ticks.ToString(); // Tạo orderId đơn giản hơn
			string requestId = DateTime.Now.Ticks.ToString(); // Tạo requestId đơn giản hơn
			string extraData = "";

			var rawHash = "accessKey=" + accessKey +
				"&amount=" + amount +
				"&extraData=" + extraData +
				"&ipnUrl=" + ipnUrl +
				"&orderId=" + orderId +
				"&orderInfo=" + orderInfo +
				"&partnerCode=" + partnerCode +
				"&redirectUrl=" + redirectUrl +
				"&requestId=" + requestId +
				"&requestType=captureWallet";

			MoMoSecurity crypto = new MoMoSecurity();
			string signature = crypto.signSHA256(rawHash, secretKey);

			var message = new
			{
				partnerCode = partnerCode,
				partnerName = "Test",
				storeId = "MomoTestStore",
				requestId = requestId,
				amount = amount,
				orderId = orderId,
				orderInfo = orderInfo,
				redirectUrl = redirectUrl,
				ipnUrl = ipnUrl,
				lang = "vi",
				extraData = extraData,
				requestType = "captureWallet",
				signature = signature
			};

			using (var client = new HttpClient())
			{
				var jsonRequest = JsonConvert.SerializeObject(message);
				var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Add("Accept", "application/json");

				var response = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", content);
				var responseContent = await response.Content.ReadAsStringAsync();

				Console.WriteLine($"Request to MoMo: {jsonRequest}");
				Console.WriteLine($"Response from MoMo: {responseContent}");

				JObject jmessage = JObject.Parse(responseContent);
				return Json(new { success = true, payUrl = jmessage.GetValue("payUrl").ToString() });
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error in MoMo payment: {ex.Message}");
			return Json(new { success = false, message = "Không thể khởi tạo thanh toán MoMo: " + ex.Message });
		}
	}

	[HttpPost]
	[Route("api/checkout/momoipn")]
	public IActionResult MoMoIPN([FromBody] JObject ipnData)
	{
		try
		{
			// Đọc và kiểm tra các thông tin cần thiết từ ipnData
			string partnerCode = ipnData.GetValue("partnerCode")?.ToString();
			string orderId = ipnData.GetValue("orderId")?.ToString();
			string requestId = ipnData.GetValue("requestId")?.ToString();
			string amount = ipnData.GetValue("amount")?.ToString();
			string orderInfo = ipnData.GetValue("orderInfo")?.ToString();
			string orderType = ipnData.GetValue("orderType")?.ToString();
			string transId = ipnData.GetValue("transId")?.ToString();
			string resultCode = ipnData.GetValue("resultCode")?.ToString();
			string message = ipnData.GetValue("message")?.ToString();
			string payType = ipnData.GetValue("payType")?.ToString();
			string responseTime = ipnData.GetValue("responseTime")?.ToString();
			string extraData = ipnData.GetValue("extraData")?.ToString();
			string signature = ipnData.GetValue("signature")?.ToString();

			// Kiểm tra tính hợp lệ của chữ ký
			string rawHash = "partnerCode=" + partnerCode + "&orderId=" + orderId + "&requestId=" + requestId + "&amount=" + amount +
							 "&orderInfo=" + orderInfo + "&orderType=" + orderType + "&transId=" + transId + "&resultCode=" + resultCode +
							 "&message=" + message + "&payType=" + payType + "&responseTime=" + responseTime + "&extraData=" + extraData;

			MoMoSecurity crypto = new MoMoSecurity();
			string secretKey = _configuration["MoMo:SecretKey"];
			string expectedSignature = crypto.signSHA256(rawHash, secretKey);

			if (signature != expectedSignature)
			{
				return BadRequest(new { success = false, message = "Chữ ký không hợp lệ" });
			}

			// Xử lý kết quả thanh toán
			if (resultCode == "0") // Kiểm tra resultCode để xác định thanh toán thành công
			{
				// Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu của bạn
				// Ví dụ:
				// var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
				// if (order != null)
				// {
				//     order.Status = "Paid";
				//     order.TransactionId = transId;
				//     _context.SaveChanges();
				// }

				return Ok(new { success = true, message = "Thanh toán thành công" });
			}
			else
			{
				return BadRequest(new { success = false, message = "Thanh toán thất bại: " + message });
			}
		}
		catch (Exception ex)
		{
			return BadRequest(new { success = false, message = "Lỗi khi xử lý thông báo từ MoMo: " + ex.Message });
		}
	}

	[HttpGet]
	[Route("Checkout/PaymentCallBack")]
	public IActionResult PaymentCallBack([FromQuery] string partnerCode, [FromQuery] string orderId, [FromQuery] string requestId, [FromQuery] string amount, [FromQuery] string orderInfo, [FromQuery] string orderType, [FromQuery] string transId, [FromQuery] string resultCode, [FromQuery] string message, [FromQuery] string payType, [FromQuery] string responseTime, [FromQuery] string extraData, [FromQuery] string signature)
	{
		try
		{
			// Kiểm tra tính hợp lệ của chữ ký
			string rawHash = "partnerCode=" + partnerCode + "&orderId=" + orderId + "&requestId=" + requestId + "&amount=" + amount +
							 "&orderInfo=" + orderInfo + "&orderType=" + orderType + "&transId=" + transId + "&resultCode=" + resultCode +
							 "&message=" + message + "&payType=" + payType + "&responseTime=" + responseTime + "&extraData=" + extraData;

			MoMoSecurity crypto = new MoMoSecurity();
			string secretKey = _configuration["MoMo:SecretKey"];
			string expectedSignature = crypto.signSHA256(rawHash, secretKey);

			if (signature != expectedSignature)
			{
				return BadRequest("Chữ ký không hợp lệ");
			}

			// Xử lý kết quả thanh toán
			if (resultCode == "0") // Thanh toán thành công
			{
				// Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu của bạn
				// Ví dụ:
				// var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
				// if (order != null)
				// {
				//     order.Status = "Paid";
				//     order.TransactionId = transId;
				//     _context.SaveChanges();
				// }

				return RedirectToAction("SuccessPage"); // Trang hiển thị thông báo thành công
			}
			else
			{
				// Xử lý thanh toán thất bại
				return RedirectToAction("FailurePage"); // Trang hiển thị thông báo thất bại
			}
		}
		catch (Exception ex)
		{
			return BadRequest("Lỗi khi xử lý phản hồi từ MoMo: " + ex.Message);
		}
	}
}
