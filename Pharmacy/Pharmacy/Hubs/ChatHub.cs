using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.SignalR;
using Pharmacy.ViewsModels;


namespace Pharmacy.Hubs
{
    public class ChatHub : Hub
    {
        //await Clients.All.SendAsync("ReceiveMessageFromUser", userId, username, message);

        //Gửi tin nhắn từ admin đến người dùng cụ thể
        //await Clients.User(userId).SendAsync("ReceiveMessageFromAdmin", message);

        // Console.WriteLine($"Message sent to user {userId}: {message}");
        public async Task SendMessageToAdmin(string userId, string username, string message)
        {
            try
            {
                // Lưu thông tin người dùng vào Firebase (nếu cần)
                await SaveUserToFirebase(userId, username);

                // Lưu tin nhắn vào Firebase
                await SaveMessageToFirebase(userId, "admin", message);

            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error sending message to admin: {ex.Message}");
            }
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            try
            {
                // Lưu thông tin người dùng vào Firebase (nếu cần)
                await SaveUserToFirebase("admin", "admin");

                // Lưu tin nhắn vào Firebase
                await SaveMessageToFirebase("admin", userId, message);

            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error sending message to user {userId}: {ex.Message}");
            }
        }

        public async Task SaveUserToFirebase(string userId, string username)
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://chat-pharmacy-17cee-default-rtdb.firebaseio.com/");

                var user = new UserViewModels
                {
                    UserId = userId,
                    Username = username
                };

                // Kiểm tra xem người dùng đã tồn tại trong Firebase chưa
                var userExists = await firebaseClient.Child("users").Child(userId).OnceSingleAsync<UserViewModels>();

                // Nếu người dùng chưa tồn tại, lưu thông tin người dùng vào Firebase
                if (userExists == null)
                {
                    await firebaseClient.Child("users").Child(userId).PutAsync(user);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error saving user to Firebase: {ex.Message}");
            }
        }

        public async Task SaveMessageToFirebase(string senderId, string receiverId, string message)
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://chat-pharmacy-17cee-default-rtdb.firebaseio.com/");

                var messageObject = new MessageViewModels
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message,
                    Timestamp = DateTime.UtcNow.Ticks // Sử dụng ticks để lưu trữ thời gian
                };

                // Lưu tin nhắn vào Firebase
                await firebaseClient.Child("messages").PostAsync(messageObject);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error saving message to Firebase: {ex.Message}");
            }
        }

        public async Task DeleteUserAndMessages(string userId)
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://chat-pharmacy-17cee-default-rtdb.firebaseio.com/");

                // Xóa người dùng từ cơ sở dữ liệu Firebase
                await firebaseClient.Child("users").Child(userId).DeleteAsync();

                // Quét cơ sở dữ liệu để tìm và xóa các tin nhắn mà người dùng đã gửi hoặc nhận
                var messages = await firebaseClient.Child("messages").OnceAsync<MessageViewModels>();
                foreach (var message in messages)
                {
                    if (message.Object.SenderId == userId || message.Object.ReceiverId == userId)
                    {
                        await firebaseClient.Child("messages").Child(message.Key).DeleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Error deleting user and messages: {ex.Message}");
            }
        }


    }

}
