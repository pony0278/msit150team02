//當使用者背包有食物(水 飼料)
//選擇後，點擊貓
//貓頭上冒愛心，食物數量-1
//三秒後等使用者看完愛心，隨機獲得獎勵

canvas.addEventListener('click', (event) => {
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;
    //點擊食物之後點擊貓咪

    let closestCat = findClosestCat(x, y);

    if (closestCat) {
        feedcat(closestCat, x, y);
        
        setTimeout(() => { // 計時器，為了讓愛心播放3秒
            closestCat.selected = false;

            /* confirmWin.style.display = 'block';*/
        }, 3000);
    }


})

function findClosestCat(x, y) {//找到最接近滑鼠位置的貓
    let closestDistance = Infinity;
    let closestCat = null;

    // 遍歷所有貓，
    [catDefault, catBB, catBK, catOG, catGY].forEach(cat => {
        if (cat !== null) {
            const distance = Math.sqrt((x - cat.x) ** 2 + (y - cat.y) ** 2);
            if (distance < closestDistance) {
                closestDistance = distance;
                closestCat = cat;
            }
        }
    });

    return closestCat;
}
function feedcat(cat, x, y) { //餵貓貓的方法     
    if (cat === null)
        return;
    if (isInBtnRange(cat, x, y)) {
        if (itmMilk.isSelected || itmCan.isSelected)//點擊食物之後點擊貓咪
        {
            cat.selected = true;
            consumeFood()
            updateTaskProgress(14)//每日任務檢查點
        }
        return;
    }
}



function consumeFood() { //扣除食物數量邏輯，數量為0就不能被扣
    if (itmMilk.isSelected) {
        if (milkCount === 0) {
            return;
        }
        updateMilkAmount(-1)
    }
    if (itmCan.isSelected) {
        if (canCount === 0) {
            return;
        }
        updateCanAmount(-1)
    }
}

const confirmWin_title = document.getElementById('confirmWin-title'); //視窗標題
const confirmWin_text = document.getElementById('confirmWin-text');//視窗內文

function giveRewardAfterCatHeart() {


}




function feedAndGetReward() {//隨機獲得貓幣方法
    //只要有餵食就一定會獲得200貓幣
    //小機率會獲得紅利10
    //小機率會獲得商城折價券
    const num = Math.floor(Math.random() * 100) + 1
    console.log(num);
    switch (true) {
        case num <= 33:
            confirmWin_text.innerHTML = '貓抓抓商城 九折折價券*1<br>請至會員中心查看'
            feedCatGetCoupon(15, 4)//九折折價券 GameProductID = 15，couponID = 4
            break;
        case num <= 67 && num >34:
            confirmWin_text.innerHTML = '紅利 50'
            updateRuby(50)
            break;
        default:

            confirmWin_text.innerHTML = '貓幣 200'
            updateCCoint(200)
            break;
    }
    confirmWin_title.innerHTML = '恭喜獲得'
    setTimeout(() => { // 計時器  
        confirmWin.style.display = 'block';
        initialize();
        fetchDataAndProcess();
    }, 3000);
}





