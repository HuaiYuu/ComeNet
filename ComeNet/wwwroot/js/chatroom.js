"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationUserHub?chatroom=" + roomId).build();



connection.on("chatToUser", (name, message) => {
    message = `${name}: ${message}`;
    $("#chat").append(`<p>${message}</p>`);
    $("#message").val("");
});

connection.start().catch(function (err)
{
    return console.error(err.toString());
}).then(function () {
    document.getElementById('user').innerHTML = 'UserId:' + userId;
    connection.invoke('GetConnectionId').then(function (connectionId)
    {
        console.log("連線");
        console.log(connectionId);
    })
});
