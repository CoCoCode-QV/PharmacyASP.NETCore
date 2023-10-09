(function ($) {
    "use strict";
    
    // Dropdown on mouse hover
    //$(document).ready(function () {
    //    function toggleNavbarMethod() {
    //        if ($(window).width() > 992) {
    //            $('.navbar .dropdown').on('mouseover', function () {
    //                $('.dropdown-toggle', this).trigger('click');
    //            }).on('mouseout', function () {
    //                $('.dropdown-toggle', this).trigger('click').blur();
    //            });
    //        } else {
    //            $('.navbar .dropdown').off('mouseover').off('mouseout');
    //        }
    //    }
    //    toggleNavbarMethod();
    //    $(window).resize(toggleNavbarMethod);
    //});
    
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });


    // Vendor carousel
    $('.vendor-carousel').owlCarousel({
        loop: true,
        margin: 29,
        nav: false,
        autoplay: true,
        smartSpeed: 1000,
        responsive: {
            0:{
                items:2
            },
            576:{
                items:3
            },
            768:{
                items:4
            },
            992:{
                items:5
            },
            1200:{
                items:6
            }
        }
    });


    // Related carousel
    $('.related-carousel').owlCarousel({
        loop: true,
        margin: 29,
        nav: false,
        autoplay: true,
        smartSpeed: 1000,
        responsive: {
            0:{
                items:1
            },
            576:{
                items:2
            },
            768:{
                items:3
            },
            992:{
                items:4
            }
        }
    });


    // Product Quantity
    $('.quantity button').on('click', function () {
        var button = $(this);
        var oldValue = button.parent().parent().find('input').val();
        if (button.hasClass('btn-plus')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        button.parent().parent().find('input').val(newVal);
    });

    //active page
    const links = document.querySelectorAll('.nav-item.nav-link');
    links.forEach(link => {
        link.addEventListener('click', (e) => {
            console.log("Link clicked"); // Thêm dòng này để kiểm tra
            // Rest of your code
        });
    });

    links.forEach(link => {
        link.addEventListener('click', (e) => {
            // Loại bỏ trạng thái active từ tất cả các liên kết
            links.forEach(l => {
                l.classList.remove('active');
            });

            // Thêm trạng thái active cho liên kết được click
            link.classList.add('active');
        });
    });
    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        margin: 24,
        dots: false,
        loop: true,
        nav: true,
        navText: [
            '<i class="bi bi-arrow-left"></i>',
            '<i class="bi bi-arrow-right"></i>'
        ],
        responsive: {
            0: {
                items: 1
            },
            992: {
                items: 2
            }
        }
    });
})(jQuery);

