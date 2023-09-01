//這邊放的是HTML製作的按鈕的功能

const commonbackBTN = document.getElementById('commonbackBTN');//轉蛋返回按鈕
const CatchaGatCha = document.getElementById('CatchaGatCha');//開轉蛋畫面
const CanvasRank = document.getElementById("CanvasRank");//排行榜頁面
const startRunGameBTN = document.getElementById('startRunGameBTN');//遊戲說明最後一頁的開始遊戲按鈕
const closeinstruction = document.getElementById('closeinstruction');//遊戲說明右上角叉叉
const popup = document.getElementById('popup');//開啟跑步遊戲說明視窗
const allPages = [Canvaslobby, Canvasrungame, CatchaGatCha, CanvasRank]//主要遊戲畫面存放區
const testlogin = document.getElementById('testlogin');
const btnProBability = document.getElementById('btnProBability');//轉蛋返回按鈕
const ProbabilityContainer = document.getElementById("Probability");


//開關機率表
let probabilityTableVisible = false;
btnProBability.addEventListener("click", () => {

    if (probabilityTableVisible) {
        ProbabilityContainer.style.display = "none";
        probabilityTableVisible = false;
    } else {
        ProbabilityContainer.style.display = "block";
        probabilityTableVisible = true;
    }
});
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
    alterConfirmWinBTN('確認', closeConfirmWin)
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

