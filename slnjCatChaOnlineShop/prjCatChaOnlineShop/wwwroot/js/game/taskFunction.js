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
                        "任務名稱": item.taskName,
                        "已進行次數": item.taskProgress,
                        "需求次數": item.taskRequireTime,
                        "獎勵": item.taskReward,
                    }))
                };

                const taskData = allTask.任務清單.map(r => `
                    <tr>
                        <td>${r.任務名稱}</td>
                        <td>${r.已進行次數}/${r.需求次數}</td>
                        <td>領取獎勵</td>
                    </tr>
                `);

                const completeDailyTask = ` <tr>
                                                 <td class="_completeAllTask">完成所有每日任務</td>
                                                 <td class="_completeAllTask">${0}/${countOfTask}</td>
                                                 <td class="_completeAllTask">領取獎勵</td>
                                            </tr>`;


                let combinedTaskDatas = taskData.concat(completeDailyTask);
                document.querySelector('#msTable > tbody').innerHTML = combinedTaskDatas.join("")
            }
        },
        error: function () {
            console.error('抓取任務清單失敗');
        }
    });
}