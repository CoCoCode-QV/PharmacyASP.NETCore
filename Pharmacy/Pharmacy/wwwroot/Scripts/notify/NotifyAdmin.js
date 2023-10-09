    
function confirmDelete(itemId, itemName, deleteAction, Message) {
    if (confirm('Bạn có chắc chắn muốn xóa ' + Message+ ' ' + itemName + ' ?')) {
        window.location.href = deleteAction + '/' + itemId;
    } else {
        return false;
    }
}
