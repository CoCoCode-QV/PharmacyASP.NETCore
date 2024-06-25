    
function confirmDelete(itemId, itemName, deleteAction, Message) {
    if (confirm('Bạn có chắc chắn muốn xóa ' + Message+ ' ' + itemName + ' ?')) {
        window.location.href = deleteAction + '/' + itemId;
    } else {
        return false;
    }
}

function confirmContentDelete(itemId, itemName, deleteAction, message) {
    // Hiển thị hộp thoại xác nhận
    if (confirm('Bạn có chắc chắn muốn xóa ' + message + ' ' + itemName + ' ?')) {
        // Hiển thị hộp thoại nhập lý do xóa
        var reason = prompt("Vui lòng nhập lý do xóa:");
       
        if (reason !== null && reason.trim() !== "") {
            var url = new URL(deleteAction, window.location.origin);
            url.searchParams.append('id', itemId);
            url.searchParams.append('reason', reason);
            // Chuyển hướng tới URL
            window.location.href = url.toString();
        } else {
            alert("Bạn phải nhập lý do xóa.");
        }
    } else {
        return false;
    }
}
