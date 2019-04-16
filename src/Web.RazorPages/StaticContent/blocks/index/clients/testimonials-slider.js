export default function() {
    let $testimonialsSlider = $(".testimonials-slider");
    let dotsContainerClass = "testimonials-slider__dots";
    let dotItemClass = "testimonials-slider__dot";
    
    $testimonialsSlider.owlCarousel({
        items: 1,
        autoplay: true,
        autoplayTimeout: 5000,
        autoplayHoverPause: true,
        loop: true,
        nav: false,
        dots: true,
        dotsClass: dotsContainerClass,
        dotClass: dotItemClass
    });
}