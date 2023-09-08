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
using prjCatChaOnlineShop.Models.CModels;
using System.Reflection;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class MemberLoginController : Controller
    {
        //將 _context 注入控制器，可以在控制器的操作方法中使用 _context 來執行資料庫查詢和操作
        private readonly cachaContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductService _productService;


        public MemberLoginController(cachaContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
            _productService = productService;
        }



        #region 登入
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(CLoginModel vm)
        {
            ShopMemberInfo existUser = (new cachaContext()).ShopMemberInfo.OrderBy(x => x.MemberId).LastOrDefault(
               t => t.Email.Equals(vm.txtEmail));

            if (existUser == null)  //帳號不存在 => 此帳號未註冊
            {
                return Json(new { Message = "帳號不存在，請進行註冊", Success = false, acountExist = false });
            }
            //帳號存在
            if (existUser.EmailVerified == false)// 是否驗證過email? 
            {
                return Json(new { Message = "帳號未完成信箱驗證，請重新註冊", Success = false, accountExist = true }); //否 => 帳號未經email驗證，請重新註冊
            }

            if (existUser.EmailVerified == true)//是=>繼續驗證密碼
            {
                if (existUser.Password == vm.txtPassword)
                {
                    string json = JsonSerializer.Serialize(existUser);
                    HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);//Session-登入狀態紀錄
                    HttpContext.Session.SetString("UserName", existUser.Name);//Session-當前當入者名稱紀錄
                    ViewBag.Categories = _productService.getAllCategories(); //把類別傳給_Layout

                    //記錄完Session之後，馬上紀錄登入時間
                    var db = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == thisUserId());
                    if (db != null)
                    {

                        if (db.LastLoginTime == null)
                        {
                            dealWithTaskForNewUser();
                        }

                        dealWithTaskForOldUser();
                        db.LastLoginTime = DateTime.Now;
                        _context.SaveChanges();
                    }
                    return Json(new { UserName = existUser.Name, Success = true, accountExist = true });//密碼正確=>登入 => 歡迎...

                }
                else
                    return Json(new { Message = "密碼錯誤，請重新嘗試", Success = true, accountExist = true });//密碼錯誤=>密碼錯誤，請重新嘗試
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
            registerModel.EmailVerified = false;
            registerModel.CheckinDayCount = false;
            registerModel.RegistrationTime = DateTime.Now;
            registerModel.CatCoinQuantity = 0;
            registerModel.LoyaltyPoints = 0;
            _context.ShopMemberInfo.Add(registerModel);
            _context.SaveChanges();


            return RedirectToAction("Login");
        }
        //驗證信箱是否存在
        [HttpPost]
        public JsonResult CheckEmailExist(string email)
        {
            bool emailExist = _context.ShopMemberInfo.Any(x => x.Email == email && x.EmailVerified == true); //帳號存在而且被驗證過
            return Json(emailExist);
        }


        public IActionResult SendMailTokenForRegister([FromBody] SendMailTokenIn inModel)
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

            if (member != null && member.EmailVerified == false)//如果會員存在但未完成驗證
            {
                // 生成一個新的隨機金鑰
                string randomKey = CKeyGenerator.GenerateRandomKey();
                // 將隨機金鑰設置到 IConfiguration 裡
                _configuration["VerifyEmail:SecretKey"] = randomKey;

                //把使用者帳號寫進session
                HttpContext.Session.SetString("ResetPwdUserEmail", inModel.MemberID);
                string storedValue = HttpContext.Session.GetString("ResetPwdUserEmail");

                string UserEmail = member.Email;

                // 產生帳號+時間驗證碼
                string sVerify = inModel.MemberID + "|" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                // 使用AES加密
                string encryptedVerify = EncryptString(sVerify, _configuration["VerifyEmail:SecretKey"]);

                // 網站網址
                string webPath = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

                // 從信件連結回到重設密碼頁面
                string receivePage = "MemberLogin/VerifyEmail";

                // 信件內容範本
                string mailContent = "請點擊以下連結，以完成身分驗證。 30 分鐘後，此連結將會失效。<br><br>";
                mailContent = mailContent + $"<a href='{webPath}/{receivePage}?verify={encryptedVerify}' target='_blank'>點此連結</a>";

                // 信件主題
                string mailSubject = "[測試] 貓抓抓Catcha 會員註冊驗證";

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

                outModel.ResultMsg = "請於 30 分鐘內至你的信箱點擊連結完成驗證，逾期將無效";
            }
            else
            {
                outModel.ErrMsg = "查無此帳號";
            }

            return Json(outModel);
        }


        public IActionResult VerifyEmail(string verify)
        {
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout
            // 取得系統自定密鑰，這裡使用 IConfiguration 讀取 
            string secretKey = _configuration["VerifyEmail:SecretKey"];
            verify = verify.Replace(" ", "+");
            if (string.IsNullOrEmpty(verify))
            {
                ViewData["ErrorMsg"] = "缺少驗證碼";
                return View();
            }
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
                HttpContext.Session.SetString("VerifyUserId", userID);

                return View();
            }
            catch (Exception ex)
            {
                ViewData["ErrorMsg"] = "處理驗證碼時出錯";
                return View();
            }

            return RedirectToAction("Login", "MemberLogin");
        }



        [HttpPost]
        public IActionResult VerifyEmail()
        {
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout
            var User = HttpContext.Session.GetString("VerifyUserId");
            var newMember = _context.ShopMemberInfo.OrderBy(x => x.MemberId).LastOrDefault(m => m.Email == User);
            if (newMember != null)
            {
                newMember.EmailVerified = true;
                _context.Update(newMember);
                _context.SaveChanges();
                HttpContext.Session.Remove("VerifyUserId");
                ViewBag.Categories = _productService.getAllCategories();

            }
            return View();
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
            var resetMember = _context.ShopMemberInfo.OrderBy(m=>m.MemberId).LastOrDefault(m => m.Email == resetPwdUserId);

            if (resetMember != null)
            {
                resetMember.Password = inModel.NewUserPwd;
                _context.Update(resetMember);
                _context.SaveChanges();
                outModel.ResultMsg = "更新成功";
                return Json(outModel);
            }
            else
            {
                ViewData["ErrorMsg"] = "找不到該會員";
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

                //如果Email不在資料庫裡就建一個
                using (var dbcontext = new cachaContext())
                {

                    var existingMember = dbcontext.ShopMemberInfo
                                .Where(mem => mem.Email == payload.Email && mem.EmailVerified == true)
                                .OrderByDescending(mem => mem.MemberId)
                                .LastOrDefault();
                    if (existingMember == null) //從來沒有註冊過的會員，直接用google登入會走這邊
                    {
                        var newMember = new ShopMemberInfo
                        {
                            Email = payload.Email,
                            Name = payload.Name,
                            MemberImage=payload.Picture,
                            RegistrationTime = DateTime.Now,
                            CatCoinQuantity = 0,
                            LoyaltyPoints = 0,
                            EmailVerified = true,

                        };
                        dbcontext.ShopMemberInfo.Add(newMember);
                        dbcontext.SaveChanges();
                        string json = JsonSerializer.Serialize(newMember);
                        HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);//Session-登入狀態紀錄
                        HttpContext.Session.SetString("UserName", newMember.Name);//Session-當前當入者名稱紀錄
                        string userName = HttpContext.Session.GetString("UserName");
                        ViewBag.UserName = userName;//把使用者名字傳給_Layout
                        ViewBag.Categories = _productService.getAllCategories(); //把類別傳給_Layout
                        dealWithTaskForNewUser();//處理每日任務

                        
                    }
                    else//有註冊過會員，但又按google登入(第一次)走這邊
                    {
                        string json = JsonSerializer.Serialize(existingMember);
                        HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, json);//Session-登入狀態紀錄
                        HttpContext.Session.SetString("UserName", existingMember.Name);//Session-當前當入者名稱紀錄
                        string userName = HttpContext.Session.GetString("UserName");
                        ViewBag.UserName = userName;//把使用者名字傳給_Layout
                        ViewBag.Categories = _productService.getAllCategories(); //把類別傳給_Layout
                        dealWithTaskForNewUser();
                    }
                }
            }

            return RedirectToAction("Index", "Index");
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

        private int thisUserId() 
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
            int _memberId = memberInfo.MemberId;
            return _memberId;
        }

        private bool? isCrossMidnight() 
        {
            var nowTime = DateTime.Now;//登入當下的時間
            var todayMidnight = DateTime.Today;//當天的午夜12點
            var lastLoginTime = _context.ShopMemberInfo.FirstOrDefault(c => c.MemberId == thisUserId()).LastLoginTime;
            if (lastLoginTime != null)
            {
                if (lastLoginTime > todayMidnight) 
                    return false;
                if(todayMidnight >= lastLoginTime)
                    return true;
            }
            return null;
        }



        //不論新舊使用者，都要先把已經啟用的任務都加進去
        //如果是新使用者(從來沒有登過)，設定progress = 0，completeTime自動為null
        //如果不是第一次登入，則先跟前一次登入時間比較(MemberInfo的LastLoginTime)，
        //如果中間有橫跨午夜12點，設定progress=0，completeTime = null
        //如果沒有則不動作
        //====================================
        private void dealWithTaskForNewUser()//發生在使用者的首次登入or曾經註冊過但又使用google登入
        {
            var availibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 1).ToList();//選出目前啟用的任務
            var taskIdList = availibaleTask.Select(task => task.TaskId).ToList();

            if (taskIdList != null)//如果有啟用的任務
            {
                foreach (var taskId in taskIdList)
                {
                    var existingRecord = _context.GameMemberTask.FirstOrDefault(x => x.MemberId == thisUserId() && x.TaskId == taskId);
                    if (existingRecord == null)//如果該使用者沒有某一個任務
                    {
                        var newTask = new GameMemberTask//就把任務加到資料表裡面
                        {
                            TaskProgress = 0,
                            MemberId = thisUserId(),
                            TaskId = taskId
                        };
                        _context.GameMemberTask.Add(newTask);
                    }
                }
                _context.SaveChanges();
            }

            else
                _context.SaveChanges();
        }

        private void dealWithTaskForOldUser() 
        {
            if (isCrossMidnight().HasValue && isCrossMidnight() == true)
            {
                var availibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 1).ToList();//選出目前啟用的任務
                var taskIdList = availibaleTask.Select(task => task.TaskId).ToList();

                if (taskIdList != null)//如果有啟用的任務
                {
                    foreach (var taskId in taskIdList)
                    {
                        var existingRecord = _context.GameMemberTask.FirstOrDefault(x => x.MemberId == thisUserId() && x.TaskId == taskId);
                        if (existingRecord == null)
                        {
                            var newTask = new GameMemberTask
                            {
                                TaskProgress = 0,
                                MemberId = thisUserId(),
                                TaskId = taskId,
                                CompleteDate = null
                            };
                            _context.GameMemberTask.Add(newTask);
                        }
                        else//如果已經存在任務，就重置
                            existingRecord.TaskProgress = 0;
                            existingRecord.CompleteDate = null;
                    }
                    _context.SaveChanges();
                }

                else
                    _context.SaveChanges();
            }
        }
    }
}
