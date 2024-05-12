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
var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
connection.start().then(function () {
    console.log("Connected to SignalR Hub");
}).catch(function (err) {
    return console.error(err.toString());
});

const chatbox = document.querySelector(".chat-body");
const chatbot = document.querySelector(".message");

const createChatLi = (message, className) => {
    const chatLi = document.createElement("li");
    chatLi.classList.add("chat", className);
    let chatContent = `<p>${message}</p>`;
    chatLi.innerHTML = chatContent;
    return chatLi;
}
database.ref('users').on('value', function (snapshot) {
    snapshot.forEach(function (childSnapshot) {
        var userId = childSnapshot.key; // Lấy id của người dùng
        var userData = childSnapshot.val(); // Lấy dữ liệu của người dùng

        // Xử lý dữ liệu của người dùng
        var userName = userData.Username;
        console.log(userName);
        displayNewUser(userId, userName);
    });
});
var createdUserIds = [];
var clickActivated = false;
var clickedUserId;
var selectedUserId;
// Hàm để kiểm tra và thêm div chứa tên người dùng mới vào sideBar
function displayNewUser(userId, userName) {
    // Kiểm tra xem userId đã được tạo div hay chưa
    if (!createdUserIds.includes(userId)) {
        // Tạo một div mới cho người dùng
        var sideBarBody = document.createElement('div');
        sideBarBody.className = 'sideBar-body';
        sideBarBody.setAttribute('data-userid', userId);


        // Tạo một div cho tên người dùng
        var userNameDiv = document.createElement('div');
        userNameDiv.className = 'col-sm-8 col-xs-8 sideBar-name';

        // Tạo một span để chứa tên người dùng
        var userNameSpan = document.createElement('span');
        userNameSpan.className = 'name-meta';
        userNameSpan.textContent = userName;

        // Thêm span vào div của người dùng
        userNameDiv.appendChild(userNameSpan);

        // Thêm div của người dùng vào .sideBar-body
        sideBarBody.appendChild(userNameDiv);

        // Thêm .sideBar-body vào .sideBar
        var sideBar = document.querySelector('.sideBar');
        sideBar.appendChild(sideBarBody);

        // Thêm userId vào mảng để đánh dấu là đã tạo div cho userId này
        createdUserIds.push(userId);
        sideBarBody.addEventListener('click', function () {
            clickedUserId = this.getAttribute('data-userid');
            console.log(clickedUserId);

            // Kiểm tra xem có phải người dùng mới được chọn không
            if (selectedUserId !== clickedUserId) {
                // Xóa tin nhắn hiện tại khỏi phần conversation
                document.querySelector('.chat-body').innerHTML = '';

                // Dừng lắng nghe tin nhắn từ người dùng trước đó (nếu có)
                if (selectedUserId) {
                    database.ref('messages').off('child_added');
                }

                // Hiển thị tin nhắn của người dùng mới
                displayMessagesForUser(clickedUserId);

                // Lưu trữ người dùng đang được chọn
                selectedUserId = clickedUserId;
            }

            document.querySelector(".reply-send").addEventListener("click", function (event) {
                // Lấy nội dung tin nhắn từ phần textarea
                var message = document.getElementById("comment").value;

                // Kiểm tra xem tin nhắn có rỗng hay không
                if (message.trim() !== "") {
                    connection.invoke("SaveMessageToFirebase", "admin", clickedUserId, message).catch(function (err) {
                        return console.error(err.toString());
                    });
                    console.log(message, clickedUserId);

                    // Xóa nội dung của phần textarea sau khi gửi tin nhắn thành công
                    document.getElementById("comment").value = "";
                }

                // Ngăn chặn hành động mặc định của form
                event.preventDefault();
            });
        });
    }
}

var userDivs = document.querySelectorAll('.sideBar-body');

function displayMessagesForUser(userId) {
    // Truy vấn tin nhắn của user từ Firebase
    database.ref('messages').orderByChild('SenderId').equalTo(userId).once('value', function (snapshot) {
        // Hiển thị phần conversation
        document.querySelector('.chat-body').style.display = 'block';

        // Xóa toàn bộ tin nhắn hiện tại trong phần conversation
        //var chatbox = document.querySelector(".chat-body");
        chatbox.innerHTML = '';

        // Lấy thông tin người dùng từ Firebase
        database.ref('users/' + userId).once('value', function (userSnapshot) {
            var userName = userSnapshot.val().Username;

            // Cập nhật tên người gửi tin nhắn
            document.getElementById("adminHeadingNameMeta").textContent = userName;
        });
        listenForNewMessages(userId);
        // Đánh dấu rằng sự kiện click đã được kích hoạt

    });
}

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
            if (senderId === "admin" && receiverId === user) {
                messageType = "sender"; // Tin nhắn gửi đi từ người dùng hiện tại
            } else if (senderId === user && receiverId === "admin") {
                messageType = "receive";
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