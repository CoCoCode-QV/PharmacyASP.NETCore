const //forms = document.querySelector(".forms"),
    pwShowHide = document.querySelectorAll(".eye-icon"),
    links = document.querySelectorAll(".link");

pwShowHide.forEach(eyeIcon => {
    eyeIcon.addEventListener("click", () => {
        let pwFields = eyeIcon.parentElement.parentElement.querySelectorAll(".password");

        pwFields.forEach(password => {
            if (password.type === "password") {
                password.type = "text";
                eyeIcon.classList.replace("bx-hide", "bx-show");
                return;
            }
            password.type = "password";
            eyeIcon.classList.replace("bx-show", "bx-hide");
        })

    })
})

links.forEach(link => {
    link.addEventListener("click", e => {
        e.preventDefault(); // Ngăn chặn hành động mặc định của liên kết

        // Ẩn tất cả các biểu mẫu
        document.querySelectorAll(".form").forEach(form => {
            form.classList.remove("active");
            form.re
        });

        // Lấy lớp mục tiêu từ liên kết
        const targetClass = link.getAttribute("data-target");

        // Hiển thị biểu mẫu mục tiêu
        //document.querySelector(`.${targetClass}`).classList.add("active");
        const targetForm = document.querySelector(`.${targetClass}`);
        targetForm.classList.add("active");

        // Đặt lại dữ liệu của biểu mẫu 
        targetForm.querySelectorAll("input").forEach(input => {
            input.value = "";
        });
    });
});



