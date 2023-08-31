$(document).ready(function () {
    //優惠券選擇，當使用者選擇該優惠券時將優惠券名稱填至input標籤裡
    $('.useCouponBtn').on('click', function () {
        // 獲取按鈕上的優惠券名稱
        var couponName = $(this).data('coupon-code');
        console.log(couponName);
        // 將優惠券名稱填入<input>標籤
        $("#couponCodeInput").val(couponName);
        // 模擬使用者操作關閉模態框
        $('.btn-close').click();
    });


    // 監聽確定按鈕的點擊事件
    //選擇宅配或郵寄地址將使用者選擇的該地址填至表格裡
    //這個代碼假設選擇的 < input > 元素是位於表格行(<tr>)內部的第一個列(<td>)中。可以根據HTML結構調整find函數中的nth-child選擇器以匹配你的實際結構。
    //這樣就不需要設置每個<input>元素的ID，並且你可以根據所選的行來查找相關的資料，然後放到指定的<span>元素中。
    $('#selectedAddress').click(function () {
        // 獲取所選的地址的相關數據
        var selectedAddress = $('input.address-radio:checked');

        // 獲取所選地址所在的<tr>
        var selectedRow = selectedAddress.closest('tr');

        // 在所選地址所在的<tr>中查找相關的資料
        var recipientName = selectedRow.find('td:nth-child(2)').text();
        var recipientAddress = selectedRow.find('td:nth-child(3)').text();
        var recipientPhone = selectedRow.find('td:nth-child(4)').text();

        console.log(recipientName);
        console.log(recipientAddress);
        console.log(recipientPhone);

        // 填入<ul>中的<span>元素
        $('#recipientNameDisplay').text(recipientName);
        $('#recipientAddressDisplay').text(recipientAddress);
        $('#recipientPhoneDisplay').text(recipientPhone);

        // 模擬使用者操作關閉模態框
        $('.btn-close').click();
    });


    // 獲取送貨方式和付款方式的元素
    // 選取DeliveryMethod_nav-tab選項容器中的所有 <button> 元素。
    const deliveryMethodButtons = document.querySelectorAll('#DeliveryMethod_nav-tab button');
    //選擇具有相同name屬性的所有元素：信用卡付款、銀行轉帳和貨到付款。
    const paymentMethodRadios = document.querySelectorAll('input[name="paymentMethod"]');

    // 監聽送貨方式按鈕的點擊事件
    deliveryMethodButtons.forEach(button => {
        button.addEventListener('click', () => {
            // 獲取當前選擇的送貨方式
            const selectedDeliveryMethod = button.getAttribute('data-bs-target');
            // 根據選擇的送貨方式啟用或禁用付款方式選項
            if (selectedDeliveryMethod === '#nav-homeService' || selectedDeliveryMethod === '#nav-post') {
                // 如果選擇了宅配到府或郵局配送，則禁用貨到付款選項
                paymentMethodRadios.forEach(radio => {
                    if (radio.id === 'nav-cashOnDelivery-tab') {
                        radio.disabled = true;
                    }
                });
            } else {
                // 否則啟用所有付款方式選項
                paymentMethodRadios.forEach(radio => {
                    radio.disabled = false;
                });
            }
        });
    });


    //監聽checkbox變更事件
    //同意退換貨條款的選項為必勾選的項目，若沒有勾選擇無法送出訂單
    $('#returnsInvoice').change(function () {
        // 檢查 checkbox 是否被勾選
        if ($(this).is(':checked')) {
            // 隱藏錯誤訊息
            $('#error-message').text('');
        } else {
            // 顯示錯誤訊息
            $('#error-message').text('此為必勾選的項目');
        }
    });
    // 監聽送出訂單按鈕的點擊事件
    $('#submit-order-btn').click(function (e) {
        // 檢查 checkbox 是否被勾選
        if (!($('#returnsInvoice').is(':checked'))) {
            // 如果未勾選，取消點擊事件，防止送出訂單
            e.preventDefault();
            // 顯示錯誤訊息
            $('#error-message').text('此為必勾選的項目');
        }
        else {
            /* e.preventDefault(); *//*因為送出就跳轉到綠界，這個可以停住確認自己的console.log的內容*/
            console.log("阿囉哈你好嗎可以讓我過關嗎^_^??")
            let formData = $("#ecpayform").serializeArray();
            var json = {};
            $.each(formData, function () {
                json[this.name] = this.value || "";
            });
            console.log(json); /*F12 -> console*/
            //step3 : 新增訂單到資料庫
            $.ajax({
                type: 'POST',
                url: 'https://localhost:7218/Ecpay/AddOrders',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(json),
                success: function (res) {
                    console.log(res);
                    ///*成功後執行跳轉*/
                    /* window.location.href = 'https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5';*/
                },
                error: function (err) { console.log(err); },
            });
        }
        //else {
        //    let formData = $("#checkForm").serializeArray();
        //    var json = {};
        //    $.each(formData, function () {
        //        json[this.name] = this.value || "";
        //    });
        //    // 發送 AJAX 請求到 AddOrder 方法
        //    $.ajax({
        //        type: 'POST',
        //        url: '/Cart/AddOrder', // 使用適當的路由 URL
        //        contentType: 'application/json; charset=utf-8',
        //        data: JSON.stringify(json),
        //        success: function (res) {
        //            console.log(res);
        //            // 根據需要處理成功回應，例如轉向到訂單確認頁面
        //        },
        //        error: function (err) {
        //            console.log(err);
        //            // 處理錯誤回應，例如顯示錯誤消息給用戶
        //        },
        //    });
        //}
    });

});

////優惠券選擇，當使用者選擇該優惠券時將優惠券名稱填至input標籤裡
//$(document).ready(function () {
//    $('.useCouponBtn').on('click', function () {
//        // 獲取按鈕上的優惠券名稱
//        var couponName = $(this).data('coupon-code');
//        console.log(couponName);
//        // 將優惠券名稱填入<input>標籤
//        $("#couponCodeInput").val(couponName);
//         // 模擬使用者操作關閉模態框
//        $('.btn-close').click();
//    });
//});

////選擇宅配或郵寄地址將使用者選擇的該地址填至表格裡
////這個代碼假設選擇的 < input > 元素是位於表格行(<tr>)內部的第一個列(<td>)中。可以根據HTML結構調整find函數中的nth-child選擇器以匹配你的實際結構。
////這樣就不需要設置每個<input>元素的ID，並且你可以根據所選的行來查找相關的資料，然後放到指定的<span>元素中。
//$(document).ready(function () {
//    // 監聽確定按鈕的點擊事件
//    $('#selectedAddress').click(function () {
//        // 獲取所選的地址的相關數據
//        var selectedAddress = $('input.address-radio:checked');

//        // 獲取所選地址所在的<tr>
//        var selectedRow = selectedAddress.closest('tr');

//        // 在所選地址所在的<tr>中查找相關的資料
//        var recipientName = selectedRow.find('td:nth-child(2)').text();
//        var recipientAddress = selectedRow.find('td:nth-child(3)').text();
//        var recipientPhone = selectedRow.find('td:nth-child(4)').text();

//        console.log(recipientName);
//        console.log(recipientAddress);
//        console.log(recipientPhone);

//        // 填入<ul>中的<span>元素
//        $('#recipientNameDisplay').text(recipientName);
//        $('#recipientAddressDisplay').text(recipientAddress);
//        $('#recipientPhoneDisplay').text(recipientPhone);

//        // 模擬使用者操作關閉模態框
//        $('.btn-close').click();
//    });
//});

////同意退換貨條款的選項為必勾選的項目，若沒有勾選擇無法送出訂單
//$(document).ready(function () {
//    // 監聽 checkbox 變更事件
//    $('#returnsInvoice').change(function () {
//        // 檢查 checkbox 是否被勾選
//        if ($(this).is(':checked')) {
//            // 隱藏錯誤訊息
//            $('#error-message').text('');
//        } else {
//            // 顯示錯誤訊息
//            $('#error-message').text('此為必勾選的項目');
//        }
//    });

//    // 監聽送出訂單按鈕的點擊事件
//    $('#submit-order-btn').click(function (e) {
//        // 檢查 checkbox 是否被勾選
//        if (!($('#returnsInvoice').is(':checked'))) {
//            // 如果未勾選，取消點擊事件，防止送出訂單
//            e.preventDefault();
//            // 顯示錯誤訊息
//            $('#error-message').text('此為必勾選的項目');
//        }
//    });
//});


