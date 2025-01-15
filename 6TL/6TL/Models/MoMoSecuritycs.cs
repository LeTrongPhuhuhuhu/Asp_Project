using System.Security.Cryptography;
using System.Text;

public class MoMoSecurity
{
	public string signSHA256(string rawData, string secretKey)
	{
		byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
		using (var hmac = new HMACSHA256(secretKeyBytes))
		{
			byte[] dataBytes = Encoding.UTF8.GetBytes(rawData);
			byte[] hashBytes = hmac.ComputeHash(dataBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}
}
