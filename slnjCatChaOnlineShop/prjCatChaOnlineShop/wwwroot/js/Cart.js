//刪除購物車的商品
$('.trashBtn').on('click', function () {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            popup: 'custom-size',
            confirmButton: 'btn btn-success',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    });
    var $button = $(this);

    // 觸發 SweetAlert2 對話框
    swalWithBootstrapButtons.fire({
        title: '確定要刪除嗎?',
        text: '刪除就要重新加囉!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: '刪啦',
        cancelButtonText: '算了',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
              //取得該商品id
            var pId = $button.data('cid');
            console.log(pId);
            $.ajax({
                type: "POST",
                url: '/Cart/DeleteCartItem',
                data: { id: pId },
                success: function (data) {
                    //同步更新刪除畫面上的商品
                    var $itemcontainer = $('[data-cid="' + pId + '"]')
                    $itemcontainer.remove();
                    swalWithBootstrapButtons.fire(
                        '刪掉囉',
                        'Your file has been deleted.',
                        'success'
                    );
                }
            })
        } else if (
            result.dismiss === Swal.DismissReason.cancel
        ) {
            swalWithBootstrapButtons.fire(
                '取消啦',
                'Your imaginary file is safe :)',
                'error'
            );
        }
    });
})