//這邊放的是HTML製作的按鈕的功能
const commonbackBTN = document.getElementById('commonbackBTN');//轉蛋返回按鈕
const CatchaGatCha = document.getElementById('CatchaGatCha');//開轉蛋畫面
const CanvasRank = document.getElementById("CanvasRank");//排行榜頁面
const tutorial = document.getElementById("tutorial");//遊戲說明頁面
const Mission = document.getElementById("Mission");//任務頁面
const startRunGameBTN = document.getElementById('startRunGameBTN');//遊戲說明最後一頁的開始遊戲按鈕
const closeinstruction = document.getElementById('closeinstruction');//遊戲說明右上角叉叉
const popup = document.getElementById('popup');//開啟跑步遊戲說明視窗
const allPages = [Canvaslobby, Canvasrungame, CatchaGatCha, CanvasRank, Mission, tutorial]//主要遊戲畫面存放區
const testlogin = document.getElementById('testlogin');
const btnProBability = document.getElementById('btnProBability');//轉蛋返回按鈕
const ProbabilityContainer = document.getElementById("Probability");// 獲取第一個頁碼按鈕元素
var closeButton = document.getElementById("closetutorial");// 獲取 "closetutorial" 按鈕元素
// 獲取所有頁碼按鈕元素
var pageA = document.getElementById("pageA");
var pageB = document.getElementById("pageB");
var pageC = document.getElementById("pageC");
var closeBtn = document.getElementById("closeinstruction");
// 獲取所有點和頁面元素
var dots = document.querySelectorAll(".dot");
var pages = document.querySelectorAll(".page");
var tutorialContainer = document.getElementById("tutorial");

closeButton.addEventListener("click", function () {
    // 隱藏整個容器
    tutorialContainer.style.display = "none";
    handleDotClick(1);
    pagesControl(Canvaslobby); //畫面返回大廳
});
// 默認選中第一頁
handleDotClick(1);

// 處理點擊點的函數
function handleDotClick(selectedDot) {
    // 隱藏所有頁面
    pages.forEach(function (page) {
        page.classList.remove("active");
    });

    // 顯示所選頁面
    var selectedPage = document.getElementById("page" + selectedDot);
    if (selectedPage) {
        selectedPage.classList.add("active");
    }

    // 更新點的樣式
    dots.forEach(function (dot, index) {
        dot.classList.remove("active");
        if (index === selectedDot - 1) {
            dot.classList.add("active");
        }
    });
}

// 添加點擊事件監聽器到每個圓點頁碼按鈕
dots.forEach(function (dot, index) {
    dot.addEventListener("click", function () {
        handleDotClick(index + 1);
    });
});

// 點擊頁碼按鈕時的事件處理函數
function handlePageClick(selectedPage) {
    // 還原所有頁碼按鈕的背景顏色
    pageA.style.backgroundColor = "";
    pageB.style.backgroundColor = "";
    pageC.style.backgroundColor = "";

    // 設置選中的頁碼按鈕的背景顏色為 #17a2b8
    selectedPage.style.backgroundColor = "#17a2b8";
}
// 頁面載入完成後默認選中第一個頁碼按鈕並設定背景顏色為 #17a2b8
window.addEventListener("load", function () {
    pageA.style.backgroundColor = "#17a2b8";
});
// 添加點擊事件監聽器到每個頁碼按鈕
pageA.addEventListener("click", function () {
    handlePageClick(pageA);
});
pageB.addEventListener("click", function () {
    handlePageClick(pageB);
});
pageC.addEventListener("click", function () {
    handlePageClick(pageC);
});
// 點擊 "x" 按鈕時，重置選中的頁碼按鈕狀態為 A
closeBtn.addEventListener("click", function () {
    handlePageClick(pageA);
    localStorage.removeItem("selectedPage");
});

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
function showRank() {
    CanvasRank.style.display = "block";

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

