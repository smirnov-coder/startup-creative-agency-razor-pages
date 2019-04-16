export default function() {
    $(".brands-carousel").owlCarousel({
        autoplay: true,
        autoplayTimeout: 3000,
        loop: true,
        items: 5,
        nav: false,
        dots: false,
        margin: 60,
        autoHeightClass: true,
        rtl: true, // Слайды движутся слева направо.
        responsiveClass: true,
        responsive : {
            0: {
                items: 1
            },
            400: {
                items: 2
            },
            600: {
                items: 3
            },
            768: {
                items: 4
            },
            992: {
                items: 5
            }
        }
    });
}