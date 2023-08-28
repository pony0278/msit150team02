
$(document).ready(async function () {

    var itemPerPageSelect = $('#itemPerPageSelect');
    var productList = $('#productList');

    var showMoreButton = $('#showMore');
    var categoryTitle = $('#categoryTitle')
    var catName = null;
    $('.for-ajax').click(function (e) {
        e.preventDefault();
        catName = $(this).data("category-name");
        categoryTitle.text(catName)
        fetchMoreProducts();
    })
    itemPerPageSelect.change(async function () {
        displayedItemCount = 0; // 重置已顯示的資料筆數
        productList.empty();
        initialItemsToShow = parseInt(itemPerPageSelect.val());
        await fetchMoreProducts();
    });
    //var addto = $('#addto')
    var displayedItemCount = 0; // 已顯示的資料筆數
    var initialItemsToShow = parseInt(itemPerPageSelect.val());
    function fetchMoreProducts() {
        var additionalItemsToShow = displayedItemCount + initialItemsToShow;
        $.ajax({
            url: '/ProductApi/ShopItemPerPage',
            type: 'GET',
            data: { catName: catName, itemPerPage: additionalItemsToShow }, // 傳遞顯示的總筆數
            dataType: 'json',
            success: function (data) {
                productList.empty();
                var products = catName !== null ? data[0].pItem : data;
                console.log(data)
                products.forEach(function (item) {
                    var productItemDiv = $('<div class="col-sm-6 col-md-6 col-lg-4 col-xl-4"></div>');
                    var productDiv = $('<div class="products-single fix"></div>');
                    var boxImgHover = $('<div class="box-img-hover shop-image"></div>');
                    /* var typeLb = $('<div class="type-lb"><p class="sale">Sale</p></div>');*/

                    var productLink = $(`<a href="/Index/ShopDetail?pId=${item.pId}" data-product-id="${item.pId}" class="shop-prod-click"></a>`);
                    var productImg = $(`<img src=${item.p圖片路徑} data-product-id=${item.pId} class="img-fluid" alt="Image" />`);

                    productLink.append(productImg);
                    //boxImgHover.append(typeLb);
                    boxImgHover.append(productLink);

                    var whyText = $('<div class="why-text"></div>');
                    var productName = $(`<h4>${item.pName}</h4>`);
                    var productPrice = $('<h5></h5>');

                    var addToCartLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-cart-coustom"><i class="fa-solid fa-cart-plus"></i></a>`);
                    var addToWishlistLink = $('<a href="/Membership/Membership" class="add-to-wishlist-coustom"><i class="far fa-heart"></i></a>');

                    whyText.append(productName);

                    whyText.append(productPrice);
                    if (item.pDiscount != null) {
                        productPrice.text('$' + item.p優惠價格);
                        var originalPrice = $(`<del>$ ${item.pPrice}</del>`);
                        whyText.append(originalPrice);
                    }
                    else {
                        productPrice.text('$' + item.pPrice);
                    }
                    whyText.append(addToCartLink);
                    whyText.append(addToWishlistLink);

                    productDiv.append(boxImgHover);
                    productDiv.append(whyText);

                    productItemDiv.append(productDiv);
                    productList.append(productItemDiv);
                });

                displayedItemCount = additionalItemsToShow; // 更新已顯示的資料筆數
                initialItemsToShow = displayedItemCount; // 更新下一次要顯示的筆數

            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });
    }

    // 初始載入商品
    await fetchMoreProducts();

    //載入更多
    showMoreButton.click(async function () {
        await fetchMoreProducts();
    });


    // 點擊商品時獲取識別ID
    productList.on('click', '.shop-prod-click', function () {
        
        console.log(productList);
        var productId = $(this).data('product-id');
        console.log('Clicked product ID:', productId);
        $.ajax({
            url: '/ProductApi/GetDetail?pId=' + productId,
            type: 'Get',
            data: { pId: productId },
            dataType: 'json',
            success: function (response) {
                console.log('Goto:', response.message);
                window.location.href = '/Index/ShopDetail?pId=' + productId;
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });
    });
    
    
    // 點擊加入購物車按鈕(用類別去找addtocartaustom抓不到)
    productList.on('click', '.add-to-cart-coustom', async function (e) {
        e.preventDefault(); // 阻止<a>標籤的點擊預設行為
        console.log(productList);
        var productId = $(this).data('product-id');
        console.log('Clicked Add to Cart, Product ID:', productId);
        // 透過 Ajax 將商品 ID 傳送到後端，加入購物車
        $.ajax({
            url: '/ProductApi/AddToCart',
            type: 'POST',
            data: { pId: productId },
            dataType: 'json',
            success: function (response) {
                
                toastr.options = {
                    // 參數設定[註1]
                    "closeButton": false, // 顯示關閉按鈕
                    "debug": false, // 除錯
                    "newestOnTop": false,  // 最新一筆顯示在最上面
                    "progressBar": true, // 顯示隱藏時間進度條
                    "positionClass": "toast-bottom-left", // 位置的類別
                    "preventDuplicates": false, // 隱藏重覆訊息
                    "onclick": null, // 當點選提示訊息時，則執行此函式
                    "showDuration": "300", // 顯示時間(單位: 毫秒)
                    "hideDuration": "1000", // 隱藏時間(單位: 毫秒)
                    "timeOut": "5000", // 當超過此設定時間時，則隱藏提示訊息(單位: 毫秒)
                    "extendedTimeOut": "1000", // 當使用者觸碰到提示訊息時，離開後超過此設定時間則隱藏提示訊息(單位: 毫秒)
                    "showEasing": "swing", // 顯示動畫時間曲線
                    "hideEasing": "linear", // 隱藏動畫時間曲線
                    "showMethod": "fadeIn", // 顯示動畫效果
                    "hideMethod": "fadeOut" // 隱藏動畫效果
                }
                // 顯示加入購物車成功的提示訊息
                toastr.success("已加入購物車!");
                console.log('Added to Cart:', response.message);
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });
    });


});
