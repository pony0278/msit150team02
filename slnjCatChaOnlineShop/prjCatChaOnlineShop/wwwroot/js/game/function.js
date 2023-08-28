let userBagData = {
    catDefault: true,
    catBB: false,//ProductID 3
    catBK: false,//ProductID 14
    catGY: false,//ProductID 1
    catOG: false,//ProductID 2
    milk: 1,
    can: 1
}
const gachaTextCCoin = document.getElementById('gachaTextCCoin')
const gachaTextRuby = document.getElementById('gachaTextRuby')

//載入(更新)資料庫資訊方法
function initialize() {
    $.ajax({
        url: '/Api/Api/TestDBLogin/玩家資訊數據',
        contentType: 'application/json',
        type: 'GET',
        success: function (data) {
            if (data.length > 0) {
                UserName = data[0].characterName; //登入時載入使用者名稱
                Ccoin = gachaTextCCoin.innerHTML = data[0].catCoinQuantity; //貓幣數量
                Ruby = gachaTextRuby.innerHTML = data[0].loyaltyPoints; //紅利數量
                hightestScore = data[0].runGameHighestScore//小遊戲最高分數

                if (data[0].gameItemInfo.find(item => item.productId === 7))
                    milkCount = data[0].gameItemInfo.find(item => item.productId === 7).quantityOfInGameItems; //牛奶數量id7
                if (data[0].gameItemInfo.find(item => item.productId === 8))
                    canCount = data[0].gameItemInfo.find(item => item.productId === 8).quantityOfInGameItems;//罐罐數量id8

               
                //載入使用者貓貓資訊
                //id 1=>gy，id 2=>OG，id3=>BK，id14=>BB
                //function catExist(id,color) 
                //{
                //    if (data[0].gameItemInfo.find(item => item.productId === id))
                //        return userBagData.catGY = true;
                //}
                if (data[0].gameItemInfo.find(item => item.productId === 1) &&
                    data[0]["gameItemInfo"][0]["quantityOfInGameItems"] > 0)
                    userBagData.catGY = true;

                if (data[0].gameItemInfo.find(item => item.productId === 2) &&
                    data[0]["gameItemInfo"][1]["quantityOfInGameItems"] > 0)
                    userBagData.catOG = true;

                if (data[0].gameItemInfo.find(item => item.productId === 3) &&
                    data[0]["gameItemInfo"][2]["quantityOfInGameItems"] > 0)
                    userBagData.catBB = true;

                if (data[0].gameItemInfo.find(item => item.productId === 14) &&
                    data[0]["gameItemInfo"][13]["quantityOfInGameItems"] > 0)
                    userBagData.catBK = true;
                loadUserBagCatInfo()
            }
        },
        error: function () {
            console.error('載入資料失敗');
        }
    });
}

//從資料庫消耗牛奶方法，num可以帶正數為增加，負數為消耗
function updateMilkAmount(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateLobbyBackpack", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fMilkCount: num }),
        success: function (data) {
            initialize();
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}
//從資料庫消耗罐罐方法，num可以帶正數為增加，負數為消耗
function updateCanAmount(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateLobbyBackpack", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fCanCount: num }),
        success: function (data) {
            initialize();
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}

//從資料庫增減貓幣方法，num可以帶正數為增加，負數為消耗
function updateCCoint(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateGameData", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fCCoin: num }),
        success: function (data) {
            initialize();
            console.log("updateCCoint:資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}

//從資料庫消耗紅利方法，num可以帶正數為增加，負數為消耗
function updateRuby(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateGameData", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fRuby: num }),
        success: function (data) {
            initialize();
            console.log("updateRuby:資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}


//餵食貓貓得到折價券的方法 //50元折價券 GameProductID = 19，couponID = 7
function feedCatGetCoupon(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/FeedCatGetCoupon", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fProductId: num }),
        success: function (data) {
            initialize();
            console.log("feedCatGetCoupon:資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}


//從資料庫更新小遊戲最高分數方法，，num為最高分數觸發
function updateHighestScore(num) {
    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateGameData", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: UserID, fScore: num }),
        success: function (data) {
            initialize();
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}

//使用者背包貓咪資訊
function loadUserBagCatInfo() {
    if (userBagData.catBB == true) {
        bagItem2.src = '../../images/game/staticCats/kittenBB_stopR.png'
    }
    if (userBagData.catBK == true) {
        bagItem3.src = '../../images/game/staticCats/kittenBK_stopR.png'
    }
    if (userBagData.catGY == true) {
        bagItem4.src = '../../images/game/staticCats/kittenGY_stopR.png'
    }
    if (userBagData.catOG == true) {
        bagItem5.src = '../../images/game/staticCats/kittenOG_stopR.png'
    }
}


//把確認視窗按鈕功能跟文字換掉的方法
function alterConfirmWinBTN(text, func) {
    confirmWinBTN.innerHTML = text;
    confirmWinBTN.onclick = func;
}

//確認視窗關閉方法
function closeConfirmWin() {
    confirmWin.style.display = 'none';//關閉確認視窗
    initialize();//初始化使用者資料
}
function closeConfirmWinForGacha() {
    confirmWin.style.display = 'none';//關閉確認視窗
}


// 小遊戲: 初始化目前被選擇的貓咪
let selectedCatName = null;
//小遊戲開始的方法
function startGame() {
    if (selectedCatName === null) { //如果使用者沒有選擇貓貓
        confirmWin_title.innerHTML = '尚未選擇貓貓'
        return;
    }
    else
        confirmWin.style.display = 'none';//關閉確認視窗
    selectedCatName = null //開始遊戲之後把選擇的貓貓清空
    resetRunGame(); //重設小遊戲設定
    pagesControl(Canvasrungame);//開啟小遊戲畫面
    showPage('a'); //說明視窗下次開啟時從第一頁開始
    alterConfirmWinBTN('確認', closeConfirmWin)//變更確認視窗按鈕內容
}

//小遊戲選擇貓貓的方法
function chooseCatBeforeGame() {
    alterConfirmWinBTN('開始', startGame);//變更確認視窗按鈕內容
    //先根據使用者擁有的貓貓載入圖片
    let userCats = `<div id="catSelectWin"  style="display:flex;  justify-content: center;">`;
    let allCat = ['catDefault', 'catBB', 'catBK', 'catGY', 'catOG']; //先列出全部的貓貓種類
    let catNames = [];//目前使用者擁有的貓貓
    //判斷要顯示哪些貓貓
    allCat.forEach(catName => {
        if (userBagData[catName] === true) {
            catNames.push(catName);
        }
    });
    catNames.forEach(catName => {
        if (userBagData[catName] === true) {
            userCats +=
                `<div id='${catName}_select' style="position: relative; flex-direction: column; width:50px;">
          <img src="../../images/game/staticCats/kitten${catName.substr(3)}_stopR.png" style="width: 50px; cursor: pointer;"/> 
          <img id='${catName}_arrow' src="../../images/game/Icon_Up.png" style="width: 30px; position:relative; display:none; "/>
          </div>`;
        }
    });
    userCats += `</div>`;//貓貓圖片的HTML尾巴
    confirmWin_title.innerHTML = '選擇進行遊戲的貓貓'
    confirmWin_text.innerHTML = userCats;
    confirmWin.style.display = 'block';


    // 選擇貓貓的點擊事件
    catNames.forEach(catName => {
        const cat = document.getElementById(`${catName}_select`);
        const catArrow = document.getElementById(`${catName}_arrow`);
        cat.addEventListener('click', () => {
            //三種狀況:
            //一、沒有選過貓貓
            //二、選了一隻貓而且下一次還選同一隻
            //三、選了一隻貓下一次選另外一隻
            if (selectedCatName === catName) {//狀況二
                catArrow.style.display = 'none';
                selectedCatName = null;
            }
            else {
                if (selectedCatName !== null) {//已經有選擇貓貓的狀況下
                    //狀況三
                    const selectedCatArrow = document.getElementById(`${selectedCatName}_arrow`);
                    selectedCatArrow.style.display = 'none';
                }
                catArrow.style.display = 'inline';//狀況一
                selectedCatName = catName;
            }
            rcat.catcolor = selectedCatName
            console.log(selectedCatName);
        });
    });

}



//===============轉蛋方法

async function doCCoinTenDraw() {
    if (貓幣數量 >= 9000) {
        try {
            const gachaData = await fetchData(); // 取得轉蛋資料
            const numDraws = 10;
            const drawResults = [];
            const allImages = [];
            const allItemName = [];
            貓幣數量 -= 9000;
            /*            console.log(貓幣數量);*/


            for (let i = 0; i < numDraws; i++) {
                const randomValue = Math.floor(Math.random() * 100) + 1; // 生成1到100之間的隨機數

                // 初始化索引和臨時總數
                let randomIndex = -1;
                let tempSum = 0;

                // 遍歷資料以找到對應的索引
                for (let j = 0; j < gachaData.length; j++) {
                    tempSum += gachaData[j].scaledProbability; // 累積縮放後的機率

                    if (randomValue <= tempSum) {
                        randomIndex = j;
                        break;
                    }
                }
                const drawnItem = gachaData[randomIndex];
                if (drawnItem && drawnItem.productName) {
                    // 將抽獎結果儲存在陣列中
                    drawResults.push(drawnItem);
                    allImages.push(drawnItem.productImage);
                    allItemName.push(drawnItem.productName);
                    //console.log(`第 ${i + 1} 次轉蛋：你獲得了 ${drawnItem.productName},${drawnItem.scaledProbability},${drawnItem.productImage}`);
                } else {
                    i--; // 減少i以重新執行本次抽獎
                }
            }

            // 計算最高等級的抽獎結果
            let maxScaledProbability = 100; // 初始設為100，確保每個機率都比它大
            let maxResult = null;
            for (const result of drawResults) {
                if (result.scaledProbability < maxScaledProbability) {
                    maxScaledProbability = result.scaledProbability;
                    maxResult = result;
                }
            }
            //將資料傳到Data傳進伺服器
            SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults)
            if (maxResult) {
                closeConfirmWinForGacha();
                // 顯示最高等級的動畫和結果，並傳遞所有物品的圖片到畫面上
                showGachaResult(maxResult.scaledProbability, allImages, allItemName);
                console.log('本輪獲得物品:', allItemName, '\r\nUID:', 使用者ID, '\r\n角色名稱:', 角色名稱, '\r\n持有貓幣數量:', 貓幣數量, '\r\n持有紅利數量:', 紅利數量, '\r\n本輪大獎:', maxResult.productName);
            }
        } catch (error) {
            console.error('轉蛋時發生錯誤:', error);
        }
    }
    else {
        console.log("貓幣不足");
        alterConfirmWinBTN('確認', closeConfirmWin)
        confirmWin_text.innerHTML = '貓幣不足'
    }

}
async function doRubyTenDraw() {
    if (紅利數量 >= 1800) {
        try {
            const gachaData = await fetchData(); // 取得轉蛋資料
            const numDraws = 10;
            const drawResults = [];
            const allImages = [];
            const allItemName = [];
            紅利數量 -= 1800;
            console.log(紅利數量);

            for (let i = 0; i < numDraws; i++) {
                const randomValue = Math.floor(Math.random() * 100) + 1; // 生成1到100之間的隨機數

                // 初始化索引和臨時總數
                let randomIndex = -1;
                let tempSum = 0;

                // 遍歷資料以找到對應的索引
                for (let j = 0; j < gachaData.length; j++) {
                    tempSum += gachaData[j].scaledProbability; // 累積縮放後的機率

                    if (randomValue <= tempSum) {
                        randomIndex = j;
                        break;
                    }
                }
                const drawnItem = gachaData[randomIndex];
                if (drawnItem && drawnItem.productName) {
                    // 將抽獎結果儲存在陣列中
                    drawResults.push(drawnItem);
                    allImages.push(drawnItem.productImage);
                    allItemName.push(drawnItem.productName);
                    //console.log(`第 ${i + 1} 次轉蛋：你獲得了 ${drawnItem.productName},${drawnItem.scaledProbability},${drawnItem.productImage}`);
                } else {
                    i--; // 減少i以重新執行本次抽獎
                }
            }

            // 計算最高等級的抽獎結果
            let maxScaledProbability = 100; // 初始設為100，確保每個機率都比它大
            let maxResult = null;
            for (const result of drawResults) {
                if (result.scaledProbability < maxScaledProbability) {
                    maxScaledProbability = result.scaledProbability;
                    maxResult = result;
                }
            }
            //將資料傳到Data傳進伺服器
            SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults)
            if (maxResult) {
                closeConfirmWinForGacha();
                // 顯示最高等級的動畫和結果，並傳遞所有物品的圖片到畫面上
                showGachaResult(maxResult.scaledProbability, allImages, allItemName);
                console.log('本輪獲得物品:', allItemName, '\r\nUID:', 使用者ID, '\r\n角色名稱:', 角色名稱, '\r\n持有貓幣數量:', 貓幣數量, '\r\n持有紅利數量:', 紅利數量, '\r\n本輪大獎:', maxResult.productName);
            }
        } catch (error) {
            console.error('轉蛋時發生錯誤:', error);
        }
    }
    else {
        console.log("紅利不足");
        alterConfirmWinBTN('確認', closeConfirmWin)
        confirmWin_text.innerHTML = '紅利不足'
    }
}
async function doCcoinSingleDraw() {
    if (貓幣數量 >= 1000) {
        try {
            const gachaData = await fetchData(); // 取得轉蛋資料
            const numDraws = 1;
            const drawResults = [];
            const allImages = [];
            const allItemName = [];
            貓幣數量 -= 1000;
            console.log(貓幣數量);

            for (let i = 0; i < numDraws; i++) {
                const randomValue = Math.floor(Math.random() * 100) + 1; // 生成1到100之間的隨機數

                // 初始化索引和臨時總數
                let randomIndex = -1;
                let tempSum = 0;

                // 遍歷資料以找到對應的索引
                for (let j = 0; j < gachaData.length; j++) {
                    tempSum += gachaData[j].scaledProbability; // 累積縮放後的機率

                    if (randomValue <= tempSum) {
                        randomIndex = j;
                        break;
                    }
                }
                const drawnItem = gachaData[randomIndex];
                if (drawnItem && drawnItem.productName) {
                    // 將抽獎結果儲存在陣列中
                    drawResults.push(drawnItem);
                    allImages.push(drawnItem.productImage);
                    allItemName.push(drawnItem.productName);
                    //console.log(`第 ${i + 1} 次轉蛋：你獲得了 ${drawnItem.productName},${drawnItem.scaledProbability},${drawnItem.productImage}`);
                } else {
                    i--; // 減少i以重新執行本次抽獎
                }
            }

            // 計算最高等級的抽獎結果
            let maxScaledProbability = 100; // 初始設為100，確保每個機率都比它大
            let maxResult = null;
            for (const result of drawResults) {
                if (result.scaledProbability < maxScaledProbability) {
                    maxScaledProbability = result.scaledProbability;
                    maxResult = result;
                }
            }
            //將資料傳到Data傳進伺服器
            SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults)
            if (maxResult) {
                closeConfirmWinForGacha();
                // 顯示最高等級的動畫和結果，並傳遞所有物品的圖片到畫面上
                showGachaResult(maxResult.scaledProbability, allImages, allItemName);
                console.log('本輪獲得物品:', allItemName, '\r\nUID:', 使用者ID, '\r\n角色名稱:', 角色名稱, '\r\n持有貓幣數量:', 貓幣數量, '\r\n持有紅利數量:', 紅利數量, '\r\n本輪大獎:', maxResult.productName);
            }
        } catch (error) {
            console.error('轉蛋時發生錯誤:', error);
        }
    }
    else {
        console.log("貓幣不足");
        alterConfirmWinBTN('確認', closeConfirmWin)
        confirmWin_text.innerHTML = '貓幣不足'
    }
}
async function doRubySingleDraw() {
    if (紅利數量 >= 200) {
        try {
            const gachaData = await fetchData(); // 取得轉蛋資料
            const numDraws = 1;
            const drawResults = [];
            const allImages = [];
            const allItemName = [];
            紅利數量 -= 200;
            console.log(紅利數量);

            for (let i = 0; i < numDraws; i++) {
                const randomValue = Math.floor(Math.random() * 100) + 1; // 生成1到100之間的隨機數

                // 初始化索引和臨時總數
                let randomIndex = -1;
                let tempSum = 0;

                // 遍歷資料以找到對應的索引
                for (let j = 0; j < gachaData.length; j++) {
                    tempSum += gachaData[j].scaledProbability; // 累積縮放後的機率

                    if (randomValue <= tempSum) {
                        randomIndex = j;
                        break;
                    }
                }
                const drawnItem = gachaData[randomIndex];
                if (drawnItem && drawnItem.productName) {
                    // 將抽獎結果儲存在陣列中
                    drawResults.push(drawnItem);
                    allImages.push(drawnItem.productImage);
                    allItemName.push(drawnItem.productName);
                    //console.log(`第 ${i + 1} 次轉蛋：你獲得了 ${drawnItem.productName},${drawnItem.scaledProbability},${drawnItem.productImage}`);
                } else {
                    i--; // 減少i以重新執行本次抽獎
                }
            }

            // 計算最高等級的抽獎結果
            let maxScaledProbability = 100; // 初始設為100，確保每個機率都比它大
            let maxResult = null;
            for (const result of drawResults) {
                if (result.scaledProbability < maxScaledProbability) {
                    maxScaledProbability = result.scaledProbability;
                    maxResult = result;
                }
            }
            //將資料傳到Data傳進伺服器
            SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults)
            if (maxResult) {
                closeConfirmWinForGacha();
                // 顯示最高等級的動畫和結果，並傳遞所有物品的圖片到畫面上
                showGachaResult(maxResult.scaledProbability, allImages, allItemName);
                console.log('本輪獲得物品:', allItemName, '\r\nUID:', 使用者ID, '\r\n角色名稱:', 角色名稱, '\r\n持有貓幣數量:', 貓幣數量, '\r\n持有紅利數量:', 紅利數量, '\r\n本輪大獎:', maxResult.productName);
            }
        } catch (error) {
            console.error('轉蛋時發生錯誤:', error);
        }
    }
    else {
        console.log("紅利不足");
        alterConfirmWinBTN('確認', closeConfirmWin)
        confirmWin_text.innerHTML = '紅利不足'
    }
}




//遊戲本體RWD方法
function resizeCanvas() {
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;

    canvas.width =
        Canvasrungame.width =
        CanvasGatcha.width =
        CanvasCheckIn.width =
        CanvasRank.width =
        CanvasDoubleCheck.width =
        CanvasSummonResult.width = 406;

    canvas.height =
        Canvasrungame.height =
        CanvasGatcha.height =
        CanvasCheckIn.height =
        CanvasRank.height =
        CanvasDoubleCheck.height =
        CanvasSummonResult.height = 600;

}

// 初始化時設置 canvas 寬高和縮放比例
resizeCanvas();

// 監聽視窗大小改變事件，重新調整 canvas 寬高和縮放比例
window.addEventListener('resize', resizeCanvas);