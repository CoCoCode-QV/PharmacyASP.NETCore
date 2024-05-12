
const firebaseConfig = {
    apiKey: "AIzaSyA7CtM6SGjQYArs2M72PbEQA6uIbj0IJAI",
    authDomain: "chat-pharmacy-17cee.firebaseapp.com",
    projectId: "chat-pharmacy-17cee",
    storageBucket: "chat-pharmacy-17cee.appspot.com",
    messagingSenderId: "51432033398",
    appId: "1:51432033398:web:2b1a312aaf326ec70d2357"
};
// Initialize Firebase
firebase.initializeApp(firebaseConfig);
const database = firebase.database();
"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
var userName;
var userId;

function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
            v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

document.getElementById("send-btn").disabled = true;

// Lắng nghe sự kiện click vào nút xác nhận tên người dùng
document.getElementById("userNameSubmitBtn").addEventListener("click", function () {
    userName = document.getElementById("userNameInput").value;
    // Ẩn ô nhập tên người dùng và hiển thị tên người dùng trong tin nhắn chào mừng
    document.querySelector(".user-name-input").style.display = "none";
    document.getElementById("userNameSpan").textContent = userName;
    // Hiển thị chatbox và chat-input
    document.querySelector(".chatbox").style.display = "block";
    document.querySelector(".chat-input").style.display = "flex";

    userId = generateGuid();
    listenForNewMessages(userId);
    connection.invoke("SaveUserToFirebase", userId, userName).catch(function (err) {
        return console.error(err.toString());
    });

});

const chatInput = document.querySelector(".chat-input textarea");
const sendChatBtn = document.querySelector(".chat-input span");
const chatbox = document.querySelector(".chatbox");
const chatbotToggler = document.querySelector(".chatbot-toggler");
const outerDiv = document.querySelector(".chatbot").parentNode;

let userMessage;

const createChatLi = (message, className) => {
    const chatLi = document.createElement("li");
    chatLi.classList.add("chat", className);
    let chatContent = className === "outgoing" ? `<p>${message}</p>` : `<span class="material-symbols-outlined">smart_toy</span><p>${message}</p>`;
    chatLi.innerHTML = chatContent;
    return chatLi;
}

connection.start().then(function () {
    document.getElementById("send-btn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
})

const handleChat = (event) => {
    userMessage = chatInput.value.trim();
    if (!userMessage) return;
    chatInput.value = '';

    var user = document.getElementById("userNameInput").value;
    //var message = document.getElementById("message").value;
    var message = userMessage;

    connection.invoke("SaveUserToFirebase", userId, user).catch(function (err) {
        return console.error(err.toString());
    });
    connection.invoke("SaveMessageToFirebase", userId, "admin", message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
}

document.getElementById("send-btn").addEventListener("click", handleChat);

chatbotToggler.addEventListener("click", function () {
    // Nếu phần tử đã có lớp 'show-chatbot', loại bỏ nó. Nếu không, thêm nó vào.
    if (outerDiv.classList.contains("show-chatbot")) {
        outerDiv.classList.remove("show-chatbot");
    } else {
        outerDiv.classList.add("show-chatbot");
    }
});

sendChatBtn.addEventListener("click", handleChat);

function listenForNewMessages(user) {
    var messagesRef = database.ref('messages');
    var latestMessageTimestamp = 0; // Biến lưu trữ thời gian của tin nhắn mới nhất

    // Lắng nghe sự kiện child_added để nhận tin nhắn mới
    messagesRef.orderByChild('Timestamp').startAt(latestMessageTimestamp + 1).on('child_added', function (childSnapshot) {
        var message = childSnapshot.val();
        var messageText = message.Content;
        var messageTimestamp = message.Timestamp;
        var senderId = message.SenderId;
        var receiverId = message.ReceiverId;

        // Kiểm tra nếu tin nhắn là mới nhất
        if (messageTimestamp > latestMessageTimestamp) {
            var messageType;
            if (senderId === user) {
                messageType = "outgoing"; // Tin nhắn gửi đi từ người dùng hiện tại
            } else if (senderId === "admin" && receiverId === user) {
                messageType = "incoming";
            } else {
                return;
            }
            // Hiển thị tin nhắn mới
            chatbox.appendChild(createChatLi(messageText, messageType));

            // Cập nhật thời gian của tin nhắn mới nhất
            latestMessageTimestamp = messageTimestamp;

            // Cuộn xuống tin nhắn mới nhất
            chatbox.scrollTo(0, chatbox.scrollHeight);
        }
    });
}

