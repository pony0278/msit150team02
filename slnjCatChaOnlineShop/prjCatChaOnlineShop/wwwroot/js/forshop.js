
$(document).ready(async function () {
    var categoryTitle = $('#categoryTitle');

    categoryTitle.text("全部商品")//沒有選類別時先顯示全部商品
    //從layout點進類別
    var catName = categoryTitle.data('category-name');
    if (catName!="" ) {
        categoryTitle.text(catName)//顯示選到的類別
    }
    console.log(catName);
    var itemPerPageSelect = $('#itemPerPageSelect');
    var productList = $('#productList');
    var showMoreButton = $('#showMore');
   
    /*var catName =null;*/
    var selOrder = $('#selOrder');
    var selBrand = $('#selBrand');

    //從index進入
    var optionOrder = categoryTitle.data('order-by');
    if (optionOrder == 1)//全新商品
        selOrder.val(1);
    if (optionOrder == 3)//熱門商品
        selOrder.val(3);

    var optionBrand = '';

    //切換類別
    $('.for-ajax').click(async function (e) {
        //e.preventDefault();

        //catName = $(this).data("category-name");//丟進選到的類別名稱
        //console.log(catName);
        //categoryTitle.text(catName)//顯示選到的類別
        var categoryTitle = $('#categoryTitle');
        var catName = categoryTitle.data('category-name');
        categoryTitle.text(catName)//顯示選到的類別

        productList.empty();//清空
        displayedItemCount = 0; //重置目前要顯示的所有筆數
        selectedValue = parseInt(itemPerPageSelect.val());//一次顯示多少筆

        await fetchMoreProducts();
    })

    //複合篩選
    var btnFilter = $('#filter-submit');
    btnFilter.on('click', async function () {
        
        productList.empty();//清空商品區
        displayedItemCount = 0;//重置目前要顯示的所有筆數
        selectedValue = parseInt(itemPerPageSelect.val());//一次顯示多少筆

        optionOrder = parseInt(selOrder.val());//丟入選到選項
        optionBrand = selBrand.val();//丟入選到選項
       
        await fetchMoreProducts();
    });

    //選擇一次顯示筆數
    itemPerPageSelect.change(async function () {

        productList.empty();
        displayedItemCount = 0; // 重置已顯示的資料筆數
        selectedValue = parseInt(itemPerPageSelect.val());
        await fetchMoreProducts();
    });
    var displayedItemCount = 0; // 已顯示的資料筆數
    var selectedValue = parseInt(itemPerPageSelect.val());

    function fetchMoreProducts() {
        displayedItemCount += selectedValue;
        $.ajax({
            url: '/ProductApi/MultipleFilter',
            type: 'GET',
            data: { optionOrder: optionOrder, optionBrand: optionBrand, catName: catName, itemPerPage: displayedItemCount }, // 傳遞顯示的總筆數
            dataType: 'json',
            success: function (data) {
                //console.log(data)
                productList.empty();
                var products =data;
                products.forEach(function (item) {
                    var productItemDiv = $('<div class="col-sm-6 col-md-6 col-lg-4 col-xl-4"></div>');
                    var productDiv = $('<div class="products-single fix"></div>');
                    var boxImgHover = $('<div class="box-img-hover shop-image"></div>');
                    /* var typeLb = $('<div class="type-lb"><p class="sale">Sale</p></div>');*/

                    var productLink = $(`<a href="/Index/ShopDetail?pId=${item.pId}" data-product-id="${item.pId}" class="shop-prod-click"></a>`);
                    var productImg = $(`<img src=${item.p圖片路徑[0]} data-product-id=${item.pId} class="img-fluid img-box-size" alt="Image" />`);

                    productLink.append(productImg);
                    //boxImgHover.append(typeLb);
                    boxImgHover.append(productLink);

                    var whyText = $('<div class="why-text"></div>');
                    var productName = $(`<h4 class="for-quantity" data-quantity=${item.p剩餘庫存}>${item.pName}</h4>`);
                    var productPrice = $('<h5></h5>');

                    var addToCartLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-cart-coustom"><i class="fa-solid fa-cart-plus"></i></a>`);
                    if (item.p是否加入收藏 == true) {
                        var addToWishlistLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-wishlist-coustom favorited"><i class="far fa-heart"></i></>`);
                    }
                    else {
                        var addToWishlistLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-wishlist-coustom"><i class="far fa-heart"></i></>`);
                    }

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
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });

    }

    // 初始載入商品
    await fetchMoreProducts();
    //載入更多
    showMoreButton.click(async function() {
        await fetchMoreProducts();
    });

    //--------------------------功能----------------------
    // 點擊商品時獲取識別ID
    //productList.on('click', '.shop-prod-click', function (e) {
    //    e.preventDefault();
        //console.log(productList);
        //var productId = $(this).data('product-id');
        //console.log('Clicked product ID:', productId);
        //$.ajax({
        //    url: '/ProductApi/GetDetail?pId=' + productId,
        //    type: 'Get',
        //    data: { pId: productId },
        //    dataType: 'json',
        //    success: function (response) {
        //        console.log('Goto:', response.message);
        //        window.location.href = '/Index/ShopDetail?pId=' + productId;
        //    },
        //    error: function (error) {
               
        //    }
        //});
    /*});*/
    
    
    // 點擊加入購物車按鈕(用類別去找addtocartaustom抓不到)
    productList.on('click', '.add-to-cart-coustom', async function (e) {
        e.preventDefault(); // 阻止<a>標籤的點擊預設行為
        
        var productId = $(this).data('product-id');
        var count = parseInt($(".for-quantity").data('quantity'));
        if (count == 0 || isNaN(count)) {
            //缺貨
            $('.toast-body i').removeClass();
            $('.toast-body i').addClass('toast-err fa-solid fa-circle-xmark fa-fade fa-lg');
            $('.toast-body strong').text('缺貨中');
            $('#shop-toast').toast('show');
        }
        else {
        // 透過 Ajax 將商品 ID 傳送到後端，加入購物車
        $.ajax({
            url: '/ProductApi/AddToCart',
            type: 'POST',
            data: { pId: productId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    $('.toast-body i').addClass('toast-added fa-regular fa-circle-check fa-beat fa-lg');
                    $('.toast-body strong').text(response.message);
                    $('#shop-toast').toast('show');
                    showCartBouble();
                }
                else {
                    if (response.messageNoQuantity) {
                        $('.toast-body i').removeClass();
                        $('.toast-body i').addClass('toast-err fa-solid fa-circle-xmark fa-fade fa-lg');
                        $('.toast-body strong').text(response.messageNoQuantity);
                        $('#shop-toast').toast('show');
                    }
                    else {
                        $('.toast-body i').removeClass();
                        $('.toast-body i').addClass('toast-warn fa-solid fa-triangle-exclamation fa-fade fa-lg');
                        $('.toast-body strong').text(response.message);
                        $('#shop-toast').toast('show');
                    }

                }
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });


        }
    });
    //加入收藏
    productList.on('click', '.add-to-wishlist-coustom', async function (e) {
        e.preventDefault(); // 阻止<a>標籤的點擊預設行為
        
        var button = $(this);
        var productId = $(this).data('product-id');
        console.log('Clicked Add to Cart, Product ID:', productId);
        // 透過 Ajax 將商品 ID 傳送到後端，加入購物車
        $.ajax({
            url: '/ProductApi/AddToWishList',
            type: 'POST',
            data: { pId: productId },
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    if (response.message == "favorited") {
                        button.addClass('favorited');
                    }
                    else {
                        button.removeClass('favorited');
                    }
                }
                else {
                    $('.toast-body i').addClass('toast-warn fa-solid fa-triangle-exclamation fa-fade fa-lg');
                    $('.toast-body strong').text(response.message);
                    $('#shop-toast').toast('show');
                }
            },
            error: function (error) {
            }
        });
    });
});
