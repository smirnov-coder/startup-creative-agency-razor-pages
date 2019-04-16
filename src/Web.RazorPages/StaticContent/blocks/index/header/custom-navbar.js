export default function() {
    let $navbar = $(".custom-navbar");
    let $navbarCollapse = $navbar.find(".custom-navbar__collapse");
    let prevScrollpos = window.pageYOffset; // Предыдущее значение прокрутки.
    
    window.addEventListener("scroll", () => {
        let currentScrollPos = window.pageYOffset; // Текущее значение прокрутки.
        // Если скролим вверх, то:
        if (prevScrollpos > currentScrollPos) {
            // Показать навбар.
            $navbar.css("top", "0");
        } else {
            // Иначе, если скролим вниз, то скрыть навбар.
            $navbar.css("top", "-" + $navbar.css("height"));
            // Если панель меню была развёрнута, свернуть её.
            $navbarCollapse.collapse("hide");
        }
        prevScrollpos = currentScrollPos;
    });

    // Немного подредактируем поведение по умолчанию раскрываемой панели.
    // При переходе к разрешению <768px, панель автоматически сворачивается.
    window.addEventListener("resize", () => {
        if (window.matchMedia("(max-width: 767px)").matches) {
            $navbarCollapse.removeClass("in");
        };
        if (window.matchMedia("(min-width: 768px)").matches) {
            $navbarCollapse.addClass("in");
        };
    }); 
}