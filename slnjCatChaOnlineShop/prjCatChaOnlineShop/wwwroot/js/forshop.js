
$(document).ready(async function () {
    var itemPerPageSelect = $('#itemPerPageSelect');
    var productList = $('#productList');

    var showMoreButton = $('#showMore');
    var categoryTitle = $('#categoryTitle');
    var catName =null;

    var selOrder = $('#selOrder');
    var selBrand = $('#selBrand');
    var optionOrder = 0;
    var optionBrand = '';

    //切換類別
    $('.for-ajax').click(async function (e) {
        e.preventDefault();

        catName = $(this).data("category-name");//丟進選到的類別名稱
        console.log(catName);
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
                    var productImg = $(`<img src=${item.p圖片路徑[0]} data-product-id=${item.pId} class="img-fluid" alt="Image" />`);

                    productLink.append(productImg);
                    //boxImgHover.append(typeLb);
                    boxImgHover.append(productLink);

                    var whyText = $('<div class="why-text"></div>');
                    var productName = $(`<h4>${item.pName}</h4>`);
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
                if (response.success) {
                    $('.toast .toast-body').text(response.message);
                    $('.toast').toast('show');
                }
                else {
                    $('.toast .toast-body').text(response.message);
                    $('.toast').toast('show');
                }
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });
    });
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
                    $('#shop-toast .toast-body').text(response.message);
                    $('#shop-toast').toast('show');
                }
            },
            error: function (error) {
            }
        });
    });
    ////複合篩選
    //var btnFilter = $('#filter-submit');  
    
    //btnFilter.on('click', function () {
    //    var selOrder = $('#selOrder');
    //    var selBrand = $('#selBrand');
    //    optionOrder = parseInt(selOrder.val());
    //    optionBrand = selBrand.val();
    //    $.ajax({
    //        url: '/ProductApi/MultipleFilter',
    //        type: 'GET',
    //        data: { optionOrder: optionOrder, optionBrand: optionBrand,catName: catName, itemPerPage: displayedItemCount },
    //        dataType: 'json',
    //        success: function (data) {
    //            //console.log(data)
    //            productList.empty();
    //            var products =  data;
    //            products.forEach(function (item) {
    //                var productItemDiv = $('<div class="col-sm-6 col-md-6 col-lg-4 col-xl-4"></div>');
    //                var productDiv = $('<div class="products-single fix"></div>');
    //                var boxImgHover = $('<div class="box-img-hover shop-image"></div>');
    //                /* var typeLb = $('<div class="type-lb"><p class="sale">Sale</p></div>');*/

    //                var productLink = $(`<a href="/Index/ShopDetail?pId=${item.pId}" data-product-id="${item.pId}" class="shop-prod-click"></a>`);
    //                var productImg = $(`<img src=${item.p圖片路徑[0]} data-product-id=${item.pId} class="img-fluid" alt="Image" />`);

    //                productLink.append(productImg);
    //                //boxImgHover.append(typeLb);
    //                boxImgHover.append(productLink);

    //                var whyText = $('<div class="why-text"></div>');
    //                var productName = $(`<h4>${item.pName}</h4>`);
    //                var productPrice = $('<h5></h5>');

    //                var addToCartLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-cart-coustom"><i class="fa-solid fa-cart-plus"></i></a>`);
    //                var addToWishlistLink = $(`<a href="#" data-product-id=${item.pId} class="add-to-wishlist-coustom"><i class="far fa-heart"></i></>`);

    //                whyText.append(productName);

    //                whyText.append(productPrice);
    //                if (item.pDiscount != null) {
    //                    productPrice.text('$' + item.p優惠價格);
    //                    var originalPrice = $(`<del>$ ${item.pPrice}</del>`);
    //                    whyText.append(originalPrice);
    //                }
    //                else {
    //                    productPrice.text('$' + item.pPrice);
    //                }
    //                whyText.append(addToCartLink);
    //                whyText.append(addToWishlistLink);

    //                productDiv.append(boxImgHover);
    //                productDiv.append(whyText);

    //                productItemDiv.append(productDiv);
    //                productList.append(productItemDiv);

    //            });
    //        },
    //        error: function (error) {
    //            console.error('Ajax Error:', error);
    //        }

    //    });
    //})
    

});
