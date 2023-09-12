"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationUserHub?userId=" + userId).build();  


console.log("gogogo");


connection.on('user-connected', (id, roomid) =>
{
    /*if (userId === id) return;*/
    console.log(`User connected: ${id}`);
    console.log(`User connected: ${roomid}`);
   
    
})
connection.on("sendToUser", (articleHeading, articleContent) =>
{

    
    var li = document.createElement("li");
    li.classList.add("dropdown-item");

    var heading = document.createElement("h3");
    heading.textContent = articleHeading;
    li.appendChild(heading);

   

    var p = document.createElement("h2");
    var a = document.createElement("a");
    a.innerText = "前往";
    a.href = articleContent

   

  

    p.appendChild(a);

    li.appendChild(p);

    var dropdownMenu = document.getElementById("dropdownMenu");
    dropdownMenu.appendChild(li);


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
connection.on("chatToUser", (name, message, useraim,usermain) =>
{
    console.log("go");
    console.log(name);
    console.log(message);
    console.log(useraim);
    console.log(usermain);


    
    if (name === usermain)
    {
        message = `${name}: ${message}`;
        console.log(message);
        $("#chatmsg").append(`<p class="sender-demo">${message}</p>`);
        $("#message").val("");
    }
    else if (name === useraim)
    {
        message = `${name}: ${message}`;
        console.log(message);
        $("#chatmsg").append(`<p class="sender-demo1">${message}</p>`);
        $("#message").val("");
    }

});
connection.on("chatnotification", (name, message) =>
{

    //console.log(notifyid);
    //// 更新通知數字
    //var notificationBadge = document.querySelector(`#chatnotification${notifyid}`);
    //notificationBadge.textContent = "new";
    //notificationBadge.classList.add('bg-danger');

    

    toastr["success"](`${name}: ${message}`)
    

});

connection.start().catch(function (err)
{
    return console.error(err.toString());
})
.then(function ()
{
    document.getElementById('user').innerHTML = 'UserId:' + userId;
    var roomid = document.getElementById('roomid');
    
    console.log(userId);
    connection.invoke('GetConnectionId').then(function (connectionId)
    {
        console.log(connectionId);        
    }).catch(function (err)
    {
        console.error('Error invoking GetConnectionId:', err.toString());
    });

    if (roomid != null)
    {
        var roomId = roomid.textContent;
         
        connection.invoke('JoinRoom', roomId)
            .then(function ()
            {
                
            })
            .catch(function (error) {
                console.error('Error joining room:', error);
            });
    }

    
  

    

   

});
