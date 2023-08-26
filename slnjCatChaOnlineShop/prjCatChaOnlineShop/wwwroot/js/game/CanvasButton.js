//這邊放的是Canvas製作的按鈕的功能

function isInBtnRange(btn, x, y)//判斷滑鼠點到哪一個按鈕，參數btn是new Button出來的按鈕，x y照填即可必填
{
    return x >= btn.x && x <= btn.x + btn.width && y >= btn.y && y <= btn.y + btn.height;
}

//載入排行榜資料
function loadRankData() {
    $.ajax({
        url: '/Api/Rank',
        type: 'GET',
        contentType: 'application/json', // 指定資料類型為 JSON
        data: { id: UserID },
       
        success: function (data) {
            const topTenData = data.slice(0, 10);//取出前十名
            const thisPlayerData = data.filter((item) => item.memberId === UserID);//取出目前玩家

            if (topTenData.length > 0) {

                const top10Rank = {
                    "排名": topTenData.map((item) => ({
                        "id":item.memberId,
                        "name": item.characterName,
                        "排名": item.rank,
                        "分數": item.runGameHighestScore
                    }))
                };

                const thisPlayerRank = {
                    "排名": thisPlayerData.map((item) => ({
                        "name": item.characterName,
                        "排名": item.rank,
                        "分數": item.runGameHighestScore
                    }))
                };
               
                const rankDatas = top10Rank.排名.map(r => `
                                            <tr>
                                                 <td class="_${r.id}">${r.排名}</td>
                                                 <td class="_${r.id}">${r.name}</td>
                                                 <td class="_${r.id}">${r.分數}</td>
                                            </tr>
                                            ` );
                const user = thisPlayerRank.排名.map(r => `
                                            <tr>
                                            <td></td><td>...</td><td></td>
                                            </tr>
                                            <tr>
                                                 <td style="color:red;">${r.排名}</td>
                                                 <td style="color:red;">${r.name}</td>
                                                 <td style="color:red;">${r.分數}</td>
                                            </tr>
                                            ` );
                let combinedRankDatas = rankDatas;
                if (!topTenData.some(item => item.memberId === UserID)) {
                   combinedRankDatas = rankDatas.concat(user);
                } 
                document.querySelector('#emTable > tbody').innerHTML = combinedRankDatas.join("")
                //設定目前腳色排行榜中的文字顏色
                const targetClass = `_${UserID}`;
                const targetElements = document.querySelectorAll(`.${targetClass}`); 
                targetElements.forEach(element => {
                    element.style.color = 'red';
                });
                
            }
        },
        error: function () {
            console.error('載入資料失敗');
        }
    });
}

CanvasDoubleCheck.addEventListener('click', (event) => { //跑步遊戲結束後詢問頁面
    const rect = CanvasDoubleCheck.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    if (isPause == true) { //玩家按下暫停時

        if (isInBtnRange(yesBTN_Pause, x, y)) { //玩家選擇是(繼續遊戲)
            CanvasDoubleCheck.style.display = "none" //隱藏詢問視窗
            continueGame();//載入暫停前的遊戲資訊
            isPause = false;
            return
        }
        if (isInBtnRange(cancelBTN_Pause, x, y)) { //玩家選擇否(結束遊戲)
            CanvasDoubleCheck.style.display = "none"//隱藏詢問視窗
            pagesControl(Canvaslobby); //畫面返回大廳
            return;
        }
    }

    if (isPause == false) { //玩家腳色死亡
        if (isInBtnRange(yesBTN, x, y)) { //玩家選擇是
            console.log("1")
            CanvasDoubleCheck.style.display = "none" //隱藏詢問視窗
            resetRunGame();//重置遊戲+重新開始遊戲
            return;
        }
        if (isInBtnRange(cancelBTN, x, y)) {//玩家選擇否
            CanvasDoubleCheck.style.display = "none"//隱藏詢問視窗
            pagesControl(Canvaslobby); //畫面返回大廳
            return;
        }
    }
}
);

CanvasSummonResult.addEventListener('click', (event) => { //轉蛋結果展示頁面
    const rect = CanvasSummonResult.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;
    if (isInBtnRange(btn, x, y)) {
        // 功能

        CanvasSummonResult.style.display = 'none';//隱藏轉蛋商品展示頁面
        arr.length = 0; //清空存放隨機數字的陣列
    }
});


Canvasrungame.addEventListener("click", (event) => {//按下暫停
    const rect = Canvasrungame.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;
    if (isInBtnRange(pauseBTN, x, y)) {
        isPause = true;
        recordCurrentPara()//紀錄目前遊戲參數
        CanvasDoubleCheck.style.display = "block"//顯示確認視窗面板
        gamePause()//彈出確認視窗
        isAnimationRunning = false; //停止遊戲所有畫面
        return;
    }
    rcat.jump();

});
function clickBagAndShowCat(cat, color) { //從背包控制收放貓貓的方法
    if (cat == null) {
        cat = new Cat(`${color}`)
        return cat
    }
    else {
        cat = null
        return cat
    }
}

canvas.addEventListener('click', (event) => {
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    //主功能按鈕

    if (isInBtnRange(helpBTN, x, y)) { //遊戲說明
        alterConfirmWinBTN('轉蛋', func)
        confirmWin.style.display = 'block';
        confirmWin_title.innerHTML = '進行轉蛋'
        confirmWin_text.innerHTML = '即將消耗 9000 貓幣進行十連抽'
        
        //// 發起 GET 請求並處理 JSON 數據
        //fetch('Api/gameapi')  // 請確保這個路徑是正確的
        //    .then(response => {
        //        if (!response.ok) {
        //            throw new Error('網絡錯誤');
        //        }
        //        return response.json();
        //    })
        //    .then(data => {
        //        console.log(data); // 在控制台輸出 JSON 數據
        //        // 在這裡進行數據處理
        //    })
        //    .catch(error => {
        //        console.error('無法獲取 JSON 數據', error);
        //    });
        //return;
    }


    if (isInBtnRange(gotoRunGame, x, y)) { //小遊戲
        popup.style.display = "block"
        showInstructions();
        return;
    }

    if (isInBtnRange(gotoGacha, x, y)) { //轉蛋
        pagesControl(CatchaGatCha);
        return;
    }

    if (isInBtnRange(rankBTN, x, y)) { //Rank
        pagesControl(CanvasRank);
        Canvaslobby.style.display = "block"
        //載入資料庫排行榜資料
        loadRankData();
        return;
    }




    // 功能
    if (isInBtnRange(itm1, x, y)) {//在背包點選Default貓貓
        if (userBagData.catDefault === true)
            catDefault = clickBagAndShowCat(catDefault, 'Default')
    }
    if (isInBtnRange(itm2, x, y)) {//在背包點選BB貓貓
        if (userBagData.catBB === true)
            catBB = clickBagAndShowCat(catBB, 'BB')
    }
    if (isInBtnRange(itm3, x, y)) {//在背包點選BK貓貓
        if (userBagData.catBK === true)
            catBK = clickBagAndShowCat(catBK, 'BK')
    }
    if (isInBtnRange(itm4, x, y)) {//在背包點選GY貓貓
        if (userBagData.catGY === true)
            catGY = clickBagAndShowCat(catGY, 'GY')
    }
    if (isInBtnRange(itm5, x, y)) {//在背包點選OG貓貓
        if (userBagData.catOG === true)
            catOG = clickBagAndShowCat(catOG, 'OG')
    }
    if (isInBtnRange(itmMilk, x, y)) {//在背包選取牛奶，一次只能選一個食物
        if (milkCount == 0)
            return;
        itmMilk.setSelected(!itmMilk.isSelected);
        if (itmCan.isSelected) {
            itmCan.setSelected(!itmCan.isSelected);
            return;
        }
    }
    if (isInBtnRange(itmCan, x, y)) {//在背包選取罐罐，一次只能選一個食物
        if (canCount == 0)
            return;
        itmCan.setSelected(!itmCan.isSelected);
        if (itmMilk.isSelected) {
            itmMilk.setSelected(!itmMilk.isSelected);
            return;
        }
    }
    if (isInBtnRange(itm8, x, y)) {
        return;
    }

});

