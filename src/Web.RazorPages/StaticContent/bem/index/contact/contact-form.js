export default function() {
    let $form = $("#contactForm");
    $form.submit(function (event) {
        // Отменить поведение по умолчанию (отправку данных на сервер).
        event.preventDefault();
        // Показать индикатор загрузки.
        toggleOverlay("show");
        // Если данные формы валидны, то:
        if ($form.valid()) {
            // Метод serialize() соберёт все данные формы, включая AntiforgeryToken.
            $.ajax({
                method: $form.attr("method"),
                url: $form.attr("action"),
                data: $form.serialize(),
                success: (data, status, response)  => {
                    $form[0].reset(); // Очистить форму.
                    showMessage(status, data); // Вывести ответ сервера.
                },
                error: (response, status, exception) => {
                    showMessage(status, response.responseText);
                }
            });
        } else {
            // Иначе скрыть лоадер, чтобы пользователь смог увидеть и исправить ошибки формы.
            toggleOverlay("hide");
        }
    });
}

function toggleOverlay(action) {
    let $overlay = $(".contact-form__overlay");
    let overlayVisibleClass = "contact-form__overlay--visible";
    action === "show"
        ? $overlay.addClass(overlayVisibleClass)
        : $overlay.removeClass(overlayVisibleClass);
}

function showMessage(status, text) {
    let $modal = $("#contact-form-modal");
    let selectors = {
        statusText: ".contact-form-modal__status-text",
        statusIcon: ".contact-form-modal__status-icon",
        message: ".contact-form-modal__message"
    };
    let iconClasses = {
        success: "contact-form-modal__status-icon contact-form-modal__status-icon--color-green fa fa-smile-o",
        error: "contact-form-modal__status-icon contact-form-modal__status-icon--color-red fa fa-frown-o"
    };
    const SUCCESS = "success";
    var $statusText = $modal.find(selectors.statusText);
    var $statusIcon = $modal.find(selectors.statusIcon);
    var $message = $modal.find(selectors.message);
    // В зависимости от ответа сервера сменить иконку модального окна.
    status === SUCCESS
        ? $statusIcon.removeClass().addClass(iconClasses.success)
        : $statusIcon.removeClass().addClass(iconClasses.error);
    $statusText.text(status);
    $message.text(text);
    // Скрыть лоадер и показать ответ сервера об операции.
    toggleOverlay("hide");
    $modal.modal("show");
}
