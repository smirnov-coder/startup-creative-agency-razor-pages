import smoothScroll from "../../common/smooth-scroll";

export default function() {
    // Добавить плавную прокрутку для каждой ссылки навигационного меню.
    $(".custom-nav__link").on("click", function (event) {
        event.data = {
            hash: $(this).attr("href"),
            selector: "html, body"
        };
        smoothScroll(event);
    });
}