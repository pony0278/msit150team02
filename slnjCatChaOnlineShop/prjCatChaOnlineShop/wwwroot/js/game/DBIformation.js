const testDBlogin = document.getElementById('testDBlogin');

async function fetchData() {
    try {
        const response = await fetch('/api/Api/Gameapi');
        if (!response.ok) {
            throw new Error('網絡錯誤');
        }
        const data = await response.json(); // 解析 JSON 格式的回應內容

        // 計算總概率
        const totalProbability = data.reduce((sum, item) => sum + item.lotteryProbability, 0);

        // 計算縮放因子
        const scalingFactor = totalProbability <= 1000 ? 1000 / totalProbability : 1;

        // 創建一個空陣列來儲存處理後的資料
        const processedData = [];

        // 對每個項目進行處理
        data.forEach(item => {
            const { productName, productId, productImage, lotteryProbability, productCategoryId, couponId} = item;

            // 將原始機率乘以縮放因子，得到縮放後的機率
            const scaledProbability = lotteryProbability * scalingFactor;

            // 將處理後的資料添加到 processedData 陣列中
            processedData.push({
                productName,
                productId,
                productImage,
                scaledProbability,
                productCategoryId,
                couponId,
                lotteryProbability
            });
        });
        return processedData; // 返回處理後的資料
    } catch (error) {
        console.error('錯誤:', error);
    }
}
async function fetchDBData() {
    try {
        const response = await fetch('/Api/Api/TestDBLogin/玩家資訊數據');
        if (!response.ok) {
            console.error('網路錯誤'); // 在拋出錯誤之前印出錯誤訊息
            throw new Error('網路錯誤');
        }
        
        const data = await response.json(); // 解析 JSON 格式的回應內容

        // 創建一個空陣列來儲存處理後的資料
        const processedData = [];

        const firstItem = data[0];
        if (!firstItem) {
            console.error('網路錯誤：沒有資料'); // 如果沒有資料，印出錯誤訊息
            throw new Error('網路錯誤：沒有資料');
        }

        firstItem.gameItemInfo.forEach(itemInfo => {
            const { productId, quantityOfInGameItems, itemName } = itemInfo;

            processedData.push({
                MemberId: firstItem.memberId,
                CharacterName: firstItem.characterName,
                CatCoinQuantity: firstItem.catCoinQuantity,
                LoyaltyPoints: firstItem.loyaltyPoints,
                RunGameHighestScore: firstItem.runGameHighestScore,
                ProductId: productId,
                QuantityOfInGameItems: quantityOfInGameItems,
                ItemName: itemName
            });
        });

        return processedData; // 返回處理後的資料
    } catch (error) {
        console.error('錯誤:', error);
    }
}

testDBlogin.addEventListener('click', async function () {
    try {


        const gachaData = await fetchData(); // 取得轉蛋資料
        const information = await fetchDBData();
        const userData = await initialize();
        //gachaData.forEach(testlProbability => {
        //    const pElement = document.createElement("p");
        //    pElement.textContent = `道具: ${testlProbability.itemName} 機率: ${testlProbability.scaledProbability}%`;
        //    itemsProbabilityContainer.appendChild(pElement);
        //    console.log(testlProbability.scaledProbability)
        //})
        information.forEach(IFM => {
            console.log(
                'ProductId:', IFM.ProductId,
                'QuantityOfInGameItems:', IFM.QuantityOfInGameItems,
                'ItemName:', IFM.ItemName
            )
        },
            使用者ID = information[0].MemberId,
            角色名稱=information[0].CharacterName,
            貓幣數量=information[0].CatCoinQuantity,
            紅利數量=information[0].LoyaltyPoints,
            console.log(
            information[0].MemberId,
            information[0].CharacterName,
            information[0].CatCoinQuantity,
            information[0].LoyaltyPoints,
            information[0].RunGameHighestScore
            )
        )
    } catch (error) {
        console.error('錯誤:', error);
    }
});

async function fetchDataAndProcess() {
    try {
        const itemsProbabilityContainer = document.getElementById("ItemsProbability");
        const gachaData = await fetchData(); // 取得轉蛋資料
        const information = await fetchDBData();
        const userData = await initialize();
        //多加入一個用來判斷要不要自動開啟遊戲說明
        
        


        gachaData.forEach(testProbability => {
            // 創建一個新的<p>元素
            const pElement = document.createElement("p");

            // 創建一個新的<img>元素
            const imgElement = document.createElement("img");
            imgElement.src = testProbability.productImage;
            imgElement.width = 48; // 設置寬度為48像素
            imgElement.height = 48; // 設置高度為48像素

            // 設置<p>元素的內容，包括圖片、道具名稱和機率
            pElement.appendChild(imgElement); // 將圖片添加到<p>元素中
            pElement.innerHTML += ` 道具名稱：${testProbability.productName} 機率：${(testProbability.scaledProbability / 10).toFixed(3) }%`;

            // 將<p>元素添加到ItemsProbability容器中
            itemsProbabilityContainer.appendChild(pElement);
        });

        information.forEach(IFM => {
            console.log(
                'ProductId:', IFM.ProductId,
                'QuantityOfInGameItems:', IFM.QuantityOfInGameItems,
                'ItemName:', IFM.ItemName
            )
        },
            使用者ID = information[0].MemberId,
            角色名稱 = information[0].CharacterName,
            貓幣數量 = information[0].CatCoinQuantity,
            紅利數量 = information[0].LoyaltyPoints,
            console.log(
                information[0].MemberId,
                information[0].CharacterName,
                information[0].CatCoinQuantity,
                information[0].LoyaltyPoints,
                information[0].RunGameHighestScore
            )
        )
    } catch (error) {
        console.error('錯誤:', error);
    }
}


