"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationUserHub?userId=" + userId).build();  

//connection.on("sendToUser", (articleHeading, articleContent) => {
//    var heading = document.createElement("h3");
//    heading.textContent = articleHeading;

//    var p = document.createElement("p");
//    p.innerText = articleContent;

//    var div = document.createElement("div");
//    div.appendChild(heading);
//    div.appendChild(p);

//    document.getElementById("articleList").appendChild(div);
//});

connection.on("sendToUser", (articleHeading, articleContent) => {
    // 創建 li 元素來容納訊息
    var li = document.createElement("li");
    li.classList.add("dropdown-item"); // 添加選單項目的樣式

    // 創建標題元素
    var heading = document.createElement("h3");
    heading.textContent = articleHeading;
    li.appendChild(heading);

    // 創建內容元素
    var p = document.createElement("h2");
    var a = document.createElement("a");
    p.innerText = articleContent;
    // 設定 <a> 標籤的 href 屬性
    a.setAttribute("href", "目標連結的URL");
    // 將 <a> 標籤添加到 <h2> 標籤中
    p.appendChild(a);
    // 將 <h2> 標籤添加到你的目標元素（例如 li）
    li.appendChild(p);

    // 將 li 元素添加到選單中的 dropdownMenu 元素中
    var dropdownMenu = document.getElementById("dropdownMenu");
    dropdownMenu.appendChild(li);

    // 更新通知數字
    var notificationBadge = document.querySelector('#notificationBadge');
    notificationBadge.textContent = "new";
    notificationBadge.classList.add('bg-danger');

    

    console.log(notificationBadge);
});

connection.start().catch(function (err)
{
    return console.error(err.toString());
}).then(function ()
{
    document.getElementById('user').innerHTML = 'UserId:' + userId;
    connection.invoke('GetConnectionId').then(function (connectionId)
    {
        console.log("連線");
     /* *//*  document.getElementById('signalRConnectionId').innerHTML = connectionId;*/
        console.log(connectionId);
    })
});
