import smoothScroll from "../../common/smooth-scroll";

export default function() {
    $(".parallax-section__button").on("click", function(event) {
        event.data = {
            hash: $(this).attr("href"),
            selector: "html, body"
        };
        smoothScroll(event);
    });
}