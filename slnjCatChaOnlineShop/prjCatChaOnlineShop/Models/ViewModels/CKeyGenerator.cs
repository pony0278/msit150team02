using System.Security.Cryptography;

namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CKeyGenerator
    {
        //隨機生成金鑰
        public static string GenerateRandomKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[16];
                rng.GetBytes(bytes);    
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
