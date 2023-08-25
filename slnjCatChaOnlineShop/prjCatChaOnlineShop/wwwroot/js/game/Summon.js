const RubySingleDrow = document.getElementById("RubySingleDrow");
const RubyTenDrows = document.getElementById('RubyTenDrows');
const CatPointSingleDrow = document.getElementById("CatPointSingleDrow");
const CatPointTenDrows = document.getElementById("CatPointTenDrows");
const result = document.getElementById('result');
const gachaContainer = document.querySelector('.gacha-container');
const animationContainer = document.querySelector('.animation-container');
const animationImages = animationContainer.querySelectorAll('.catcha');
const summonbuttons = document.getElementById('summon-buttons');

let processedData = []; //建立一個空陣列接Data資料
//連接使用者的資料庫數據
let 使用者ID;
let 角色名稱;
let 貓幣數量;
let 紅利數量;
let 道具ID = [];
// 初始化 playerDataArray 為一個空陣列
const playerDataArray = [];

// 當使用者進行抽獎時，將抽獎數據添加到 playerDataArray
function SAVEDATA(使用者ID, 貓幣數量, 紅利數量, drawResults) {
    const apiUrl = '/api/Api/TestDBLogin';
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
                initialize();
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

function showGachaResult(scaledProbability, allImages, allItemName,) {
    result.innerHTML = '';
    // 創建 ItemNameContainer 變數並初始化
    const ItemNameContainer = document.createElement('div');
    ItemNameContainer.classList.add('item-container');

    // 將四個button關掉顯示
    summonbuttons.style.display = 'none';
    // 用於儲存要顯示的動畫等級
    let animationLevel = '';

    // 檢查是否有SS等級的獎項，如果有則設置動畫等級為'SS'
    if (scaledProbability <= 5) {
        animationLevel = 'SS';
    } else if (scaledProbability <= 10) {
        animationLevel = 'S';
    } else {
        animationLevel = 'A';
    }

    // 根據動畫等級顯示相應的動畫
    if (animationLevel === 'SS') {
        animationContainer.querySelector('.SS').style.display = 'inline';
    } else if (animationLevel === 'S') {
        animationContainer.querySelector('.S').style.display = 'inline';
    } else {
        animationContainer.querySelector('.A').style.display = 'inline';
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
        // 判斷是進行十抽還是單抽，並調整版型

    });


    setTimeout(() => {
        animationImages.forEach(image => {
            image.style.display = 'none';
        });

        // 插入抽獎物品的圖片到抽獎格子中
        const resultContainer = document.querySelector('.result-container');
        resultContainer.style.display = 'display';

        const confirmButton = document.createElement('button');
        confirmButton.id = 'Btn_itemOk';
        confirmButton.textContent = '確認';
        //confirmButton.style.fontSize = '20px';
        //confirmButton.style.width = '70px';
        //confirmButton.style.height = '50px';


        confirmButton.addEventListener('click', () => {
            // 點擊確認按鈕後，關閉獎項顯示，並顯示四個button
            result.innerHTML = '';
            result.style.display = 'none';
            summonbuttons.style.display = 'block';
        });

        // 將確認按鈕添加到獎項後面
        result.appendChild(confirmButton);
        result.style.display = 'grid';
    }, 5000); // 5000毫秒等於5秒
}
