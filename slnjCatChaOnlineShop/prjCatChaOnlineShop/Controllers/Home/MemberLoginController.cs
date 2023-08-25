using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using System.Text.Json;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static prjCatChaOnlineShop.Models.ViewModels.CForgetPwdModel;
using Google.Apis.Auth;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class MemberLoginController : Controller
    {
        //將 _context 注入控制器，可以在控制器的操作方法中使用 _context 來執行資料庫查詢和操作
        private readonly cachaContext _context;
        private readonly IConfiguration _configuration;
        public MemberLoginController(cachaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region 登入
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(CLoginModel vm)
        {
            
            ShopMemberInfo user = (new cachaContext()).ShopMemberInfo.FirstOrDefault(
                t => t.Email.Equals(vm.txtEmail) && t.Password.Equals(vm.txtPassword));
            if (user != null && user.Password.Equals(vm.txtPassword))
            {
                string Json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Json);
                return RedirectToAction("Index", "Index");
            }
            return View();
        }
        #endregion

        #region 註冊會員
        public IActionResult RegisterMember()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterMember(ShopMemberInfo registerModel)
        {

            _context.ShopMemberInfo.Add(registerModel);
            _context.SaveChanges();

            return RedirectToAction("Login");

        }
        //驗證信箱是否存在
        [HttpPost]
        public JsonResult CheckEmailExist(string email)
        {
            bool emailExist = _context.ShopMemberInfo.Any(x => x.Email == email);
            return Json(emailExist);
        }
        #endregion

        #region 忘記密碼
        public IActionResult ForgetPassword()
        {
            return View();
        }
        //重設密碼
        public IActionResult ResetPassword(string verify)
        {
            verify = verify.Replace(" ", "+");
            if (string.IsNullOrEmpty(verify))
            {
                ViewData["ErrorMsg"] = "缺少驗證碼";
                return View();
            }

            // 取得系統自定密鑰，這裡使用 IConfiguration 讀取 
            string secretKey = _configuration["ForgetPassword:SecretKey"];

            try
            {
                // 使用 AES 解密驗證碼
                string decryptedVerify = DecryptString(verify, secretKey);

                if (string.IsNullOrEmpty(decryptedVerify))
                {
                    ViewData["ErrorMsg"] = "驗證碼錯誤";
                    return View();
                }

                // 解析分隔的資料
                string[] verifyParts = decryptedVerify.Split('|');

                if (verifyParts.Length != 2)
                {
                    ViewData["ErrorMsg"] = "驗證碼格式不正確";
                    return View();
                }

                string userID = verifyParts[0];
                DateTime resetTime;

                if (!DateTime.TryParse(verifyParts[1], out resetTime))
                {
                    ViewData["ErrorMsg"] = "無效的重設時間";
                    return View();
                }

                // 檢查時間是否超過 30 分鐘
                TimeSpan timeElapsed = DateTime.Now - resetTime;

                if (timeElapsed.TotalMinutes > 30)
                {
                    ViewData["ErrorMsg"] = "超過驗證碼有效時間，請重寄驗證碼";
                    return View();
                }

                // 驗證碼檢查成功，加入 Session
                HttpContext.Session.SetString("ResetPwdUserId", userID);

                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMsg"] = "處理驗證碼時出錯";
                return View();
            }
        }

        //輸入電子信箱後按下寄送認證碼執行的方法
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult SendMailToken([FromBody] SendMailTokenIn inModel)
        {
            SendMailTokenOut outModel = new SendMailTokenOut();

            // 檢查輸入來源
            if (string.IsNullOrEmpty(inModel.MemberID))
            {
                outModel.ErrMsg = "請輸入帳號";
                return Json(outModel);
            }

            // 使用 LINQ 查詢資料庫以獲取會員資料
            var member = _context.ShopMemberInfo
                .Where(m => m.Email == inModel.MemberID)
                .FirstOrDefault();

            if (member != null)
            {
                // 生成一個新的隨機金鑰
                string randomKey = CKeyGenerator.GenerateRandomKey();
                // 將隨機金鑰設置到 IConfiguration 裡
                _configuration["ForgetPassword:SecretKey"] = randomKey;

                //把使用者帳號寫進session
                HttpContext.Session.SetString("ResetPwdUserEmail", inModel.MemberID);
                string storedValue = HttpContext.Session.GetString("ResetPwdUserEmail");

                string UserEmail = member.Email;

                // 產生帳號+時間驗證碼
                string sVerify = inModel.MemberID + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                // 使用AES加密
                string encryptedVerify = EncryptString(sVerify, _configuration["ForgetPassword:SecretKey"]);

                // 網站網址
                string webPath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

                // 從信件連結回到重設密碼頁面
                string receivePage = "MemberLogin/ResetPassword";

                // 信件內容範本
                string mailContent = "請點擊以下連結，返回網站重新設定密碼，逾期 30 分鐘後，此連結將會失效。<br><br>";
                mailContent = mailContent + $"<a href='{webPath}/{receivePage}?verify={encryptedVerify}' target='_blank'>點此連結</a>";

                // 信件主題
                string mailSubject = "[測試] 重設密碼申請信";

                // Google 發信帳號密碼
                string GoogleMailUserID = _configuration["ForgetPassword:GoogleMailUserID"];
                string GoogleMailUserPwd = _configuration["ForgetPassword:GoogleMailUserPwd"];

                // 使用 Google Mail Server 發信
                string SmtpServer = "smtp.gmail.com";
                int SmtpPort = 587;
                MailMessage mms = new MailMessage();
                mms.From = new MailAddress(GoogleMailUserID);
                mms.Subject = mailSubject;
                mms.Body = mailContent;
                mms.IsBodyHtml = true;
                mms.SubjectEncoding = Encoding.UTF8;
                mms.To.Add(new MailAddress(UserEmail));

                using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(GoogleMailUserID, GoogleMailUserPwd);
                    client.Send(mms); //寄出信件
                }

                outModel.ResultMsg = "請於 30 分鐘內至你的信箱點擊連結重新設定密碼，逾期將無效";
            }
            else
            {
                outModel.ErrMsg = "查無此帳號";
            }

            return Json(outModel);
        }

        // AES加密方法:產生隨機金鑰
        private string EncryptString(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // 使用零IV，因為不需要解密

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        //AES解密方法：解密Key+帳號+送出驗證碼時間所得到的加密字串
        private string DecryptString(string cipherText, string key)
        {
            //cipherText.Replace(" ", "+");
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // 使用零IV，因為不需要解密

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            //using (Aes aesAlg = Aes.Create())
            //{
            //    aesAlg.Key = Encoding.UTF8.GetBytes(key);
            //    aesAlg.Mode = CipherMode.ECB;
            //    aesAlg.Padding = PaddingMode.PKCS7;

            //    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            //    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            //    {
            //        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            //        {
            //            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            //            {
            //                return srDecrypt.ReadToEnd();
            //            }
            //        }
            //    }
            //}

            //// 使用 Google Mail Server 發信
            //string GoogleID = "smilehsiang@gmail.com"; //Google 發信帳號
            //string TempPwd = "wafdnrkmgrjagvvf"; //應用程式密碼
            //string ReceiveMail = "smilehsiang@yahoo.com.tw"; //接收信箱

            //string SmtpServer = "smtp.gmail.com";
            //int SmtpPort = 587;
            //MailMessage mms = new MailMessage();
            //mms.From = new MailAddress(GoogleID);
            //mms.Subject = "信件主題";
            //mms.Body = "信件內容";
            //mms.IsBodyHtml = true;
            //mms.SubjectEncoding = Encoding.UTF8;
            //mms.To.Add(new MailAddress(ReceiveMail));
            //using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            //{
            //    client.EnableSsl = true;
            //    client.Credentials = new NetworkCredential(GoogleID, TempPwd);//寄信帳密 
            //    client.Send(mms); //寄出信件
            //}
        }

        //修改密碼至資料庫
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DoResetPwd([FromBody] DoResetPwdIn inModel)
        {
            DoResetPwdOut outModel = new DoResetPwdOut();

            // 檢查是否有輸入密碼
            if (string.IsNullOrEmpty(inModel.NewUserPwd))
            {
                outModel.ErrMsg = "請輸入新密碼";
                return Json(outModel);
            }
            if (string.IsNullOrEmpty(inModel.CheckUserPwd))
            {
                outModel.ErrMsg = "請輸入確認新密碼";
                return Json(outModel);
            }
            if (inModel.NewUserPwd != inModel.CheckUserPwd)
            {
                outModel.ErrMsg = "新密碼與確認新密碼不相同";
                return Json(outModel);
            }

            // 檢查帳號 Session 是否存在
            var resetPwdUserId = HttpContext.Session.GetString("ResetPwdUserId");
            if (string.IsNullOrEmpty(resetPwdUserId))
            {
                outModel.ErrMsg = "無修改帳號";
                return Json(outModel);
            }

            //取得目前使用者輸入的新密碼
            string NewPwd = inModel.NewUserPwd;

            //使用cachaContext更新到資料庫
            var resetMember = _context.ShopMemberInfo.FirstOrDefault(m => m.Email == resetPwdUserId);

            if (resetMember != null)
            {
                resetMember.Password = inModel.NewUserPwd;
                _context.Update(resetMember);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ViewData["ErrorMsg"] = "找不到該會員";
                return View();
            }
        }
        #endregion

        #region 第三方登入Google
        //Google登入傳回的頁面
        public IActionResult ValidGoogleLogin()
        {
            string? formCredential = Request.Form["credential"]; //回傳憑證
            string? formToken = Request.Form["g_csrf_token"]; //回傳令牌
            string? cookiesToken = Request.Cookies["g_csrf_token"]; //Cookie 令牌

            // 驗證 Google Token
            GoogleJsonWebSignature.Payload? payload = VerifyGoogleToken(formCredential, formToken, cookiesToken).Result;
            if (payload == null)
            {
                // 驗證失敗
                ViewData["Msg"] = "驗證 Google 授權失敗";
            }
            else
            {
                //驗證成功，取使用者資訊內容
                ViewData["Msg"] = "驗證 Google 授權成功" + "<br>";
                ViewData["Msg"] += "Email:" + payload.Email + "<br>";
                ViewData["Msg"] += "Name:" + payload.Name + "<br>";
                ViewData["Msg"] += "Picture:" + payload.Picture;
            }

            return View();
        }

        //串接Google登入的api
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string? formCredential, string? formToken, string? cookiesToken)
        {
            // 檢查空值
            if (formCredential == null || formToken == null && cookiesToken == null)
            {
                return null;
            }

            GoogleJsonWebSignature.Payload? payload;
            try
            {
                // 驗證 token
                if (formToken != cookiesToken)
                {
                    return null;
                }

                // 驗證憑證
                IConfiguration Config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
                string GoogleApiClientId = Config.GetSection("GoogleApiClientId").Value;
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoogleApiClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(formCredential, settings);
                if (!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
                {
                    return null;
                }
                if (payload.ExpirationTimeSeconds == null)
                {
                    return null;
                }
                else
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    DateTime expiration = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
                    if (now > expiration)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return payload;
        }
        #endregion
    }
}
