const RubySingleDrow = document.getElementById("RubySingleDrow");
const RubyTenDrows = document.getElementById('RubyTenDrows');
const CatPointSingleDrow = document.getElementById("CatPointSingleDrow");
const CatPointTenDrows = document.getElementById("CatPointTenDrows");
const result = document.getElementById('result');
const gachaContainer = document.querySelector('.gacha-container');
const animationContainer = document.querySelector('.animation-container');
const animationImages = animationContainer.querySelectorAll('.catcha');
const summonbuttons = document.getElementById('summon-buttons');
const skipButton = document.getElementById('skipButton');

let skipClicked = false;
let processedData = []; //建立一個空陣列接Data資料
//連接使用者的資料庫數據
let 使用者ID;
let 角色名稱;
let 貓幣數量;
let 紅利數量;
let 道具ID = [];
// 定義全局變數以存儲計時器的引用
let gachaTimer;


// 初始化 playerDataArray 為一個空陣列
const playerDataArray = [];

// 當使用者進行抽獎時，將抽獎數據添加到 playerDataArray
function SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults) {
    const apiUrl = '/Api/Api/TestDBLogin/傳回轉蛋數據';
    const userData = {
        MemberId: 使用者ID,
        CatCoinQuantity: 貓幣數量,
        LoyaltyPoints: 紅利數量,

        GachaResult: drawResults
    }
        // 發送 POST 請求
        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(userData),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('發送數據時發生錯誤GGGGGG');
                }
                fetchDataAndProcess();
                return response.json();
            })
            .then(responseData => {
                console.log('數據已成功發送:', responseData);
            })
            .catch(error => {
                console.error('發送數據時發生錯誤:', error);
            });
}



CatPointTenDrows.addEventListener('click', function () {
    confirmWin.style.display = 'block';
    confirmWin_title.innerHTML = '進行轉蛋'
    confirmWin_text.innerHTML = '即將消耗 9000 貓幣進行十連抽'
    confirmWinBTN.onclick = doCCoinTenDraw;
});

RubyTenDrows.addEventListener('click', function () {
    confirmWin.style.display = 'block';
    confirmWin_title.innerHTML = '進行轉蛋'
    confirmWin_text.innerHTML = '即將消耗 1800 紅利進行十連抽'
    confirmWinBTN.onclick = doRubyTenDraw;
    
});

CatPointSingleDrow.addEventListener('click', function () {
    confirmWin.style.display = 'block';
    confirmWin_title.innerHTML = '進行轉蛋'
    confirmWin_text.innerHTML = '即將消耗 1000 貓幣進行單抽'
    confirmWinBTN.onclick = doCcoinSingleDraw;
});

RubySingleDrow.addEventListener('click', function () {
    confirmWin.style.display = 'block';
    confirmWin_title.innerHTML = '進行轉蛋'
    confirmWin_text.innerHTML = '即將消耗 200 紅利進行單抽'
    confirmWinBTN.onclick = doRubySingleDraw;
});
//顯示抽獎動畫及道具
function showGachaResult(scaledProbability, allImages, allItemName) {
    skipClicked = false;
    clearTimeout(gachaTimer);
    result.innerHTML = '';

    // 創建 ItemNameContainer 變數並初始化
    const ItemNameContainer = document.createElement('div');
    ItemNameContainer.classList.add('item-container');

    // 將四個 button 關掉顯示
    summonbuttons.style.display = 'none';

    // 用於儲存要顯示的動畫等級
    let animationLevel = '';

    // 檢查是否有 SS 等級的獎項，如果有則設置動畫等級為 'SS'
    if (scaledProbability <= 10) {
        animationLevel = 'SS';
    } else if (scaledProbability <= 15) {
        animationLevel = 'S';
    } else if (scaledProbability <= 20) {
        animationLevel = 'CATS';
    } else {
        animationLevel = 'A';
    }

    // 清空動畫容器
    animationImages.forEach(image => {
        image.style.display = 'none';
        image.src = ''; // 清空 GIF 圖片的 src 屬性
    });
    skipButton.style.display = 'block';

    // 根據動畫等級顯示相應的動畫
    if (animationLevel === 'SS') {
        const ssImage = animationContainer.querySelector('.SS');
        ssImage.style.display = 'inline';
        ssImage.src = '../images/game/gacha/SS.gif'; // 設置 GIF 圖片的路徑
    } else if (animationLevel === 'S') {
        const sImage = animationContainer.querySelector('.S');
        sImage.style.display = 'inline';
        sImage.src = '../images/game/gacha/S.gif'; // 設置 GIF 圖片的路徑
    } else if (animationLevel === 'CATS') {
        const catsImage = animationContainer.querySelector('.CATS');
        catsImage.style.display = 'inline';
        catsImage.src = '../images/game/gacha/CATS.gif'; // 設置 GIF 圖片的路徑
    } else {
        const aImage = animationContainer.querySelector('.A');
        aImage.style.display = 'inline';
        aImage.src = '../images/game/gacha/A.gif'; // 設置 GIF 圖片的路徑
    }


    // 創建一個空的容器用於儲存所有的物品容器
    const allItemContainers = [];

    allImages.forEach((imageSrc, index) => {
        // 創建包含圖片和文字的容器
        const itemContainer = document.createElement('div');
        itemContainer.classList.add('item-container'); // 可以添加額外的 CSS 類名或樣式

        // 創建圖片元素
        const itemImage = document.createElement('img');
        itemImage.src = imageSrc; // 這裡需要提供正確的圖片 URL
        itemImage.style.width = '48px';
        itemImage.style.height = '48px';

        // 創建文字元素
        const itemNameElement = document.createElement('p');
        itemNameElement.textContent = allItemName[index]; // 使用相同的索引以取得相應的文字

        // 將圖片和文字添加到容器中
        itemContainer.appendChild(itemImage);
        itemContainer.appendChild(itemNameElement);

        // 儲存此容器到 allItemContainers 陣列中
        allItemContainers.push(itemContainer);

        // 將容器添加到 ItemNameContainer 中
        result.appendChild(itemContainer);
    });

    gachaTimer = setTimeout(() => {
        if (!skipClicked) {
            animationImages.forEach(image => {
                image.style.display = 'none';
            });
            skipButton.style.display = 'none';

            const resultContainer = document.querySelector('.result-container');
            resultContainer.style.display = 'block';

            const confirmButton = document.createElement('button');
            confirmButton.id = 'Btn_itemOk';
            confirmButton.textContent = '確認';

            confirmButton.addEventListener('click', () => {
                result.innerHTML = '';
                result.style.display = 'none';
                summonbuttons.style.display = 'block';
            });
            result.appendChild(confirmButton);
            result.style.display = 'grid';
        }
    }, 10000); // 10 秒
}


// 跳過動畫
skipButton.addEventListener('click', function () {
    skipClicked = true;
    clearTimeout(gachaTimer); // 清除計時器

    animationImages.forEach(image => {
        image.style.display = 'none';
    });
    skipButton.style.display = 'none';

    const resultContainer = document.querySelector('.result-container');
    resultContainer.style.display = 'block';

    const confirmButton = document.createElement('button');
    confirmButton.id = 'Btn_itemOk';
    confirmButton.textContent = '確認';

    confirmButton.addEventListener('click', () => {
        result.innerHTML = '';
        result.style.display = 'none';
        summonbuttons.style.display = 'block';
    });
    result.appendChild(confirmButton);
    result.style.display = 'grid';
});
