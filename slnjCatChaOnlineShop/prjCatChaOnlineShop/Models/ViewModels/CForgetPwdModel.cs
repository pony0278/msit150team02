namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CForgetPwdModel
    {
        /// <summary>
        /// [寄送驗證碼]參數
        /// </summary>
        public class SendMailTokenIn
        {
            public string MemberID { get; set; }
        }

        /// <summary>
        /// [寄送驗證碼]回傳
        /// </summary>
        public class SendMailTokenOut
        {
            public string ErrMsg { get; set; }
            public string ResultMsg { get; set; }
        }

        /// <summary>
        /// [重設密碼]參數
        /// </summary>
        public class DoResetPwdIn
        {
            public string NewUserPwd { get; set; }
            public string CheckUserPwd { get; set; }
        }

        /// <summary>
        /// [重設密碼]回傳
        /// </summary>
        public class DoResetPwdOut
        {
            public string ErrMsg { get; set; }
            public string ResultMsg { get; set; }
        }
    }
}
