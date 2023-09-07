//定義初始變數
let UserID = 1033; //TODO改成動態，正式版本改成用account判斷?
let UserName = "未登入"
let Ccoin = "N/A";
let Ruby = "N/A";
let milkCount = 0;
let canCount = 0;
let hightestScore = 0;


// 確保 DOM 加載完成後執行代碼
document.addEventListener("DOMContentLoaded", function () {

    
    // 獲取所有需要的元素
    const submitButton = document.getElementById("memberIdLogin");
    const GameMain = document.querySelector('.canvas-container');
    const userMemberidlogin = document.getElementById("userMemberidlogin");
    const registerForm = document.getElementById('registerForm');
    const registerButton = document.getElementById('registerButton');
    const testDBlogin = document.getElementById('testDBlogin');
    const testGameDB = document.getElementById('testGameDB');

    let _memberId
    //// memberId 輸入，只允許數字
    //document.getElementById("memberId").addEventListener("input", function (e) {
    //    const input = e.target;
    //    const value = input.value;
    //    if (/[^0-9]/.test(value)) {
    //        input.value = value.replace(/[^0-9]/g, '');
    //    }
    //});

    //meberId登入
    submitButton.addEventListener("click", function (event) {
        event.preventDefault();
        const apiUrl = '/Api/Api/TestUserLogin/玩家登入數據';

        const data = {
            MemberId: _memberId,
        };

        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('發送數據時發生錯誤GGGGGG');
                }
                return response.json();
            })
            .then(responseData => {
                handleLoginResponse(responseData);
            })
            .catch(error => {
                console.error('發送數據時發生錯誤:', error);
            });
    });

    // 註冊角色名稱
    registerButton.addEventListener('click', function () {
        const characterName = document.getElementById('characterName').value;
        const registerData = {
            CharacterName: characterName,
        };

        fetch('/Api/Api/TestUserLogin/玩家註冊數據', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(registerData),
        })
            .then(response => response.json())
            .then(registerResponse => {
                handleRegisterResponse(registerResponse);
            })
            .catch(error => {
                console.error('註冊時發生錯誤:', error);
            });
    });

    // 處理登入回應
    function handleLoginResponse(responseData) {
        if (responseData.message === '找到這個玩家') {
            alert('登入成功！');
            testGameDB.style.display = 'block';
            testDBlogin.style.display = 'block';
            GameMain.style.display = 'block';
            userMemberidlogin.style.display = 'none';
            callApiWithMemberId(_memberId);
        } else if (responseData.message === '沒有此玩家') {
            alert('該玩家不存在，請註冊！');
            userMemberidlogin.style.display = 'none';
            registerForm.style.display = 'block';
        } else {
            alert('未知錯誤發生，請稍後在試');
        }
    }

    // 處理註冊回應
    function handleRegisterResponse(registerResponse) {
        // 處理註冊回應，例如顯示註冊成功訊息等
        if (registerResponse.message === '此ID已被註冊') {
            alert('此ID已被註冊請重新輸入');
        } else if (registerResponse.message === "註冊成功") {
            alert('註冊成功!!\r\n獲得新手禮包---初始褐貓!!\r\n獲得貓幣---10000貓幣');
            GameMain.style.display = 'block';
            registerForm.style.display = 'none';
            testGameDB.style.display = 'block';
            testDBlogin.style.display = 'block';
            testlogin.style.display = 'block';
        } else {
            alert('未知錯誤發生，請稍後在試');
        }
    }
    // 呼叫 API 並傳遞 MemberId
    function callApiWithMemberId(_memberId) {
        // 在這裡執行需要使用 MemberId 的 API 呼叫
        const apiUrl = '/Api/Api/TestUserLogin/玩家登入數據';
        const data = {
            MemberId: _memberId,
        };

        fetch(apiUrl, {
            method: 'POST', 
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('發送數據時發生錯誤GGGGGG');
                }
                return response.json();
            })
            .then(apiResponseData => {
                console.log('使用 MemberId 的 API 回應:', apiResponseData);
            })
            .catch(error => {
                console.error('使用 MemberId 的 API 發送數據時發生錯誤:', error);
            });
    }
});
