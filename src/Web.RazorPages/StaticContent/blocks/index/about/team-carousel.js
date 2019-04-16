export default function() {
    let carouselNavClass = "team-carousel__nav";
    let prevButtonClass = "carousel-nav-button team-carousel__nav-button-prev";
    let nextButtonClass = "carousel-nav-button team-carousel__nav-button-next";
    let prevButtonIcon = "<i class=\"fa fa-angle-left\"></i>";
    let nextButtonIcon = "<i class=\"fa fa-angle-right\"></i>";

    $(".team-carousel").owlCarousel({
        items: 0,
        margin: 30,
        loop: true,
        dots: false,
        nav: true,
        navContainerClass: carouselNavClass,
        navClass: [prevButtonClass, nextButtonClass],
        navText: [prevButtonIcon, nextButtonIcon],
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            480: {
                items: 2
            },
            768: {
                items: 3
            },
            992: {
                items: 4
            }
        }
    });
};