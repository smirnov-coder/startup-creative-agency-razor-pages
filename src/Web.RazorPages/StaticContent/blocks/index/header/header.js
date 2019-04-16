import smoothScroll from "../../common/smooth-scroll";

export default function() {
    // Добавить отступ сверху величиной высоты навбара, чтобы
    // центрировать по вертикали содержимое шапки.
    let navbarHeight = $(".custom-navbar").css("height");
    $(".header__center-content").css("padding-top", navbarHeight);
    
    // Добавить плавную прокрутку на кнопку контента шапки.
    $(".header__button").on("click", function (event) {
        event.data = {
            hash: $(this).attr("href"),
            selector: "html, body"
        };
        smoothScroll(event);
    });
}