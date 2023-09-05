"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationUserHub?chatroom=" + roomId).build();



connection.on("chatToUser", (name, message) => {
    message = `${name}: ${message}`;
    $("#chat").append(`<p>${message}</p>`);
    $("#message").val("");
});


