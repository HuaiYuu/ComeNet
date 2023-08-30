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

connection.on('user-connected', id => {
    if (userId === id) return;
    console.log(`User connected: ${id}`);
    conectNewUser(id, localStream)

})

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
    a.innerText = "點選";
    a.setAttribute("href", articleContent);
    a.setAttribute("target", "_blank");
    p.appendChild(a);
    li.appendChild(p);

    var dropdownMenu = document.getElementById("dropdownMenu");
    dropdownMenu.appendChild(li);

    // 更新通知數字
    var notificationBadge = document.querySelector('#notificationBadge');
    notificationBadge.textContent = "new";
    notificationBadge.classList.add('bg-danger');

    

    console.log(notificationBadge);
});
connection.on("activityinvitation", (articleHeading, articleContent,activity) => {
    
    var li = document.createElement("li");
    li.classList.add("dropdown-item"); 

    var heading = document.createElement("h3");
    heading.textContent = articleHeading;
    li.appendChild(heading);

    var activityid = document.createElement("p");
    activityid.textContent = activity;
    activityid.setAttribute("id", "activityid");   
    activityid.style.display = "none";
    li.appendChild(activityid);
   
    var p = document.createElement("h2");
    var a = document.createElement("a");
    a.innerText = articleContent;  

    var agreeButton = document.createElement("button");
    agreeButton.innerText = "同意";
    agreeButton.addEventListener("click", function ()
    {

        var activityid = document.getElementById('activityid');       
        var username = document.getElementById('username');
        
        var myHeaders = new Headers();
        myHeaders.append("Content-Type", "application/json");

        var rawact = JSON.stringify({
            activityid: activityid.textContent,
            name: username.textContent,
            id: userId,         
        });

        var actrequestOptions = {
            method: 'POST',
            headers: myHeaders,
            body: rawact,
            redirect: 'follow'
        };

        fetch("https://localhost:7136/api/users/CreateActivityPeople", actrequestOptions)
            .then(response => response.json())
            .then(result => {
                if (result.message == "ok")
                {
                    toastr["success"]("活動新增成功");
                }
                else
                {
                    alert('登入失敗');
                    window.location.reload();
                }
              
            })
            .catch(error => console.log('error', error));







        p.removeChild(agreeButton);
        p.removeChild(rejectButton);
    });

    
    var rejectButton = document.createElement("button");
    rejectButton.innerText = "拒绝";
    rejectButton.addEventListener("click", function () {
        
        p.removeChild(agreeButton);
        p.removeChild(rejectButton);
    });

    
    p.appendChild(agreeButton);
    p.appendChild(rejectButton);

    p.appendChild(a);
    
    li.appendChild(p);

    var dropdownMenu = document.getElementById("dropdownMenu");
    dropdownMenu.appendChild(li);

    
    var notificationBadge = document.querySelector('#notificationBadge');
    notificationBadge.textContent = "new";
    notificationBadge.classList.add('bg-danger');



    console.log(notificationBadge);
});

connection.on("chatToUser", (name, message) =>
{
    message = `${name}: ${message}`;
    console.log(message);
    $("#chatmsg").append(`<p>${message}</p>`);
    $("#message").val("");
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
        console.log(connectionId);

    })

    
});
