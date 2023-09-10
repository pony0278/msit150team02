//任務清單UI呈現用
function loadTask() {
    $.ajax({
        url: '/Task/LoadTask',
        type: 'GET',
        contentType: 'application/json', // 指定資料類型為 JSON
        success: function (data) {
            if (data.length > 0) {
                const countOfTask = data.length;//確認目前有幾個每日任務
                const allTask = {
                    "任務清單": data.map((item) => ({
                        "任務ID": item.taskId,
                        "任務名稱": item.taskName,
                        "已進行次數": item.taskProgress,
                        "需求次數": item.taskRequireTime,
                        "獎勵": item.taskReward,
                        "完成時間": item.completeDate
                    }))
                };

                const taskData = allTask.任務清單.map(r => {
                    if (r.完成時間 !== null) {
                        return `
            <tr>
                <td>${r.任務名稱}</td>
                <td>${r.已進行次數}/${r.需求次數}</td>
                <td onclick="complteTaskReward(${r.任務ID})" style="cursor: pointer; background: #DE9E4F;">領取獎勵</td>
            </tr>
        `;
                    } else if (r.完成時間 == null & r.已進行次數!=0) {
                        return `
            <tr>
                <td>${r.任務名稱}</td>
                <td>${r.已進行次數}/${r.需求次數}</td>
                <td style="cursor: pointer; background: gray; color: white;">已領取獎勵</td>
            </tr>
        `;
                    } else {
                        return `
            <tr>
                <td>${r.任務名稱}</td>
                <td>${r.已進行次數}/${r.需求次數}</td>
                <td style="cursor: pointer; background: gray; ">未完成</td>
            </tr>
        `;
                    }
                });



               
                //const completeDailyTask = ` <tr>
                //                                 <td class="_completeAllTask">完成所有每日任務</td>
                //                                 <td class="_completeAllTask">${0}/${countOfTask}</td>
                //                                 <td class="_completeAllTask">領取獎勵</td>
                //                            </tr>`;


                let combinedTaskDatas = taskData/*.concat(completeDailyTask);*/
                document.querySelector('#msTable > tbody').innerHTML = combinedTaskDatas.join("")
            }
        },
        error: function () {
            console.error('抓取任務清單失敗');
        }
    });
}

//確認使用者某任務的進度用
//TaskID12 小遊戲5分以上
//TaskID13 小遊戲玩3次
//TaskID14 餵貓一次

//function checkProgress(taskIdToCheck) {
//    $.ajax({
//        url: '/Task/CheckMachine',
//        type: 'GET',
//        contentType: 'application/json', // 指定資料類型為 JSON
//        data: JSON.stringify({ fTaskId: taskIdToCheck }),
//        success: function (data) {
//            if (data.taskProgress == data.taskRequireTime) {
//                //這邊要觸發另外一個ajax方法
//                //如果完成之後要給一個時間戳
//                //用時間戳去觸發把領獎按鈕開啟(時間戳會去觸發開啟獎勵)
//                //使用者領完之後再把領獎按鈕鎖回去(時間戳設為null)


//            }
        
//        },
//        error: function () {
//            console.error('查詢使用者任務失敗');
//        }
//    });



//}

//更新使用者任務進度用(要包含做到一半跟完成)
function updateTaskProgress(TaskId) {
    $.ajax({
        type: "POST",
        url: "/Task/UpdateTask", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ TaskId: TaskId }),
        success: function (data) {
            loadTask()
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
}





function complteTaskReward(taskid) {//任務完成獲得貓幣的方法
    confirmWin_text.innerHTML = '貓幣 200'
    updateCCoint(200)
    confirmWin_title.innerHTML = '恭喜獲得' 
    confirmWin.style.display = 'block';
    ResetTaskAfterReward(taskid);
    loadTask();
    initialize();
    fetchDataAndProcess();
            
}

//更新使用者任務進度用(要包含做到一半跟完成)
function ResetTaskAfterReward(TaskId) {
    $.ajax({
        type: "POST",
        url: "/Task/ResetTaskAfterReward", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        data: JSON.stringify({ TaskId: TaskId }),
        success: function (data) {
            loadTask()
            console.log("資料更新成功", data.message);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }
    });
} 

//DEMO用重置日期

function demoRestTask() {
    $.ajax({
        type: "GET",
        url: "/Task/demoRest", // API 的 URL
        contentType: 'application/json', // 指定資料類型為 JSON
        success: function (data) {
            loadTask()
            console.log(data);
        },
        error: function (error) {
            console.log("資料更新失敗", error);
        }

    });

}