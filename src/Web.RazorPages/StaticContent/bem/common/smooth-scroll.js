export default function(event) {
    const hash = event.data.hash;
    const selector = event.data.selector;
    // Убедиться, что ссылка имеет хэш (значение атрибута href).
    if (hash !== "") {
        // Отменить поведение ссылки по умолчанию.
        event.preventDefault();

        // Плавно прокрутить страницу до нужного места в течение 800мс.
        $(selector).animate({
            scrollTop: $(hash).offset().top
        }, 800, () => {
            // Добавить хэш в адресную строку (поведение ссылки по умолчанию).
            window.location.hash = hash;
        });
    };
};