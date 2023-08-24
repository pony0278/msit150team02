//這邊放的是HTML製作的按鈕的功能

const commonbackBTN = document.getElementById('commonbackBTN');//轉蛋返回按鈕
const CatchaGatCha = document.getElementById('CatchaGatCha');//開轉蛋畫面
const CanvasRank = document.getElementById("CanvasRank");//排行榜頁面
const startRunGameBTN = document.getElementById('startRunGameBTN');//遊戲說明最後一頁的開始遊戲按鈕
const closeinstruction = document.getElementById('closeinstruction');//遊戲說明右上角叉叉
const popup = document.getElementById('popup');//開啟跑步遊戲說明視窗
const allPages = [Canvaslobby, Canvasrungame, CatchaGatCha, CanvasRank]//主要遊戲畫面存放區
const testlogin = document.getElementById('testlogin');


function pagesControl(blockpage)//參數blockpage填入當前需要顯示的畫面，並隱藏其他頁面
{
  for(const p of allPages)
  {
    p.style.display = "none"
  }
  blockpage.style.display = "block"
}

//==========================


//回首頁功能
commonbackBTN.addEventListener("click", () => { 
  pagesControl(Canvaslobby);
  });

//小遊戲說明
startRunGameBTN.addEventListener("click", () => {
    chooseCatBeforeGame()
    hideInstructions()
});

//關閉小遊戲說明
closeinstruction.addEventListener("click", () => {
    showPage('a'); //下次開啟時從第一頁開始
    pagesControl(Canvaslobby); //畫面返回大廳
});
//TODO 這邊沒有作用，怪怪的
const confirmWin_close = document.getElementById('confirmWin_close');
//確認視窗關閉
confirmWin_close.addEventListener("click", () => {
    closeConfirmWin();
});

//測試連動資料庫登入
testlogin.addEventListener("click", () => {
    initialize()
});

const testGameDB = document.getElementById('testGameDB');
testGameDB.addEventListener("click", () => {
    console.log('開始測試')

    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateGameData", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: 1033, fCCoin: 10 }),
        success: function (data) {
            initialize();
            console.log("資料更新成功", data.message);
        },
        error: function ( error) {
            console.log("資料更新失敗",error);
        }
    });

    $.ajax({
        type: "POST",
        url: "/api/Api/UpdateLobbyBackpack", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ fId: 1033, fMilkCount: 10 }),
        success: function (data) {
            initialize();
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });


});