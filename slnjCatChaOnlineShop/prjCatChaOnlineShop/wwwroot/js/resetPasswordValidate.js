jQuery.extend(jQuery.validator.messages, {
    required: "此欄位必填",
    email: "請輸入正確的電子信箱",
    equalTo: "請再次輸入相同的值.",
    maxlength: $.validator.format("最多輸入 {0} 個字"),
    minlength: $.validator.format("至少輸入 {0} 個字"),
    rangelength: $.validator.format("請輸入 {0} 到 {1} 個字"),
    range: $.validator.format("請輸入 {0} 到 {1} 的數字"),
    max: $.validator.format("請輸入小於等於 {0} 的值"),
    min: $.validator.format("請輸入大於等於 {0} 的值")
});
//驗證密碼
jQuery.validator.addMethod("NewUserPwds", function (value, element) {
    var str = value;
    var result = false;

    if (str.length > 0) {
        //測試有沒有在6~15個字
        var patt = /^[a-zA-z0-9]{6,15}$/;
        var result = patt.test(str);
        //測試有沒有英文字母
        var pattEn = /[a-zA-Z]{1,}/;
        resultEn = pattEn.test(str);
        //測試有沒有數字
        var pattDigit = /[0-9]{1,}/;
        resultNum = pattDigit.test(str);

        //三個條件都符合才對
        if (result == true && resultEn == true && resultNum == true) {
            result = true;
        }
        else {
            result = false;
        }
    } else {
        result = true;
    }
    return result;
}, "密碼為 6～15個字元的英文字母、數字混合，但不含空白鍵及標點符號。")