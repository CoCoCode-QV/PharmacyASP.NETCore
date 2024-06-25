const firebaseConfig = {
    apiKey: "AIzaSyA7CtM6SGjQYArs2M72PbEQA6uIbj0IJAI",
    authDomain: "chat-pharmacy-17cee.firebaseapp.com",
    projectId: "chat-pharmacy-17cee",
    storageBucket: "chat-pharmacy-17cee.appspot.com",
    messagingSenderId: "51432033398",
    appId: "1:51432033398:web:2b1a312aaf326ec70d2357"
};

// Function to initialize Firebase and set up listeners
function initializeChat() {
    // Initialize Firebase
    firebase.initializeApp(firebaseConfig);
    const database = firebase.database();

    // Initialize SignalR connection
    var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
    connection.start().then(function () {
        console.log("Connected to SignalR Hub");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    // Get initial message count from localStorage or set to 0
    let oldMessageCount = parseInt(localStorage.getItem('oldMessageCount')) || 0;
    let newMessageCount = 0;

    // Function to add red dot to message icon
    function addRedDotToMessageIcon() {
        var messageIcon = document.querySelector('.fa-envelope.fa-fw');
        if (messageIcon) {
            var redDot = document.querySelector('.fa-envelope.fa-fw .red-dot');
            if (!redDot) {
                redDot = document.createElement('span');
                redDot.className = 'red-dot';
                messageIcon.appendChild(redDot);
            }
        }
    }

    // Function to remove red dot from message icon
    function removeRedDotFromMessageIcon() {
        var redDot = document.querySelector('.fa-envelope.fa-fw .red-dot');
        if (redDot) {
            redDot.remove();
        }
    }

    // Function to listen for new messages
    function listenForNewMessages() {
        var messagesRef = database.ref('messages');

        // Get initial message count
        messagesRef.orderByChild('ReceiverId').equalTo('admin').once('value', function (snapshot) {
            newMessageCount = snapshot.numChildren();
            if (newMessageCount > oldMessageCount) {
                addRedDotToMessageIcon();
            }
            localStorage.setItem('oldMessageCount', newMessageCount);
        });

        messagesRef.orderByChild('ReceiverId').equalTo('admin').on('child_added', function (childSnapshot) {
            newMessageCount++;
            if (newMessageCount > oldMessageCount) {
                addRedDotToMessageIcon();
                oldMessageCount = newMessageCount; // Update old message count to new message count
                localStorage.setItem('oldMessageCount', oldMessageCount); // Save to localStorage
            }
        });
    }

    // Call function to listen for new messages
    listenForNewMessages();

    // Event listener for "Tin nhắn" link click
    var messageLink = document.querySelector('a[href="' + '@Url.Action("ChatAdmin", "HomeAdmin")' + '"]');
    if (messageLink) {
        messageLink.addEventListener('click', function () {
            removeRedDotFromMessageIcon();
        });
    }
}

// Run the initializeChat function when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeChat();
});
