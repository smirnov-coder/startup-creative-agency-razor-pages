export default function () {
    const METHOD_NAME = "atleastoneoftwo";

    // Добавить пользовательский метод валидации jquery-validation.
    // Метод проверяет наличие значения хотя бы у одного элемента из двух.
    if (!$.validator.methods[METHOD_NAME]) {
        $.validator.addMethod(METHOD_NAME, function (value, element, params) {
            let result = this.optional(element) == true || !!value || !!$(`#${params}`).val();
            return result;
        }, "No value was provided.");
    }

    // Добавить unobtrusive-адаптер для пользовательского метода валидации.
    if ($.validator.unobtrusive.adapters.findIndex(x => x.name === METHOD_NAME) === -1) {
        $.validator.unobtrusive.adapters.addSingleVal(METHOD_NAME, "other", METHOD_NAME);

        // Обновить все валидаторы на странице.
        $("form").each(function () {
            $(this).removeData("validator").removeData("unobtrusiveValidation");
        });
        $.validator.unobtrusive.parse(document);
    }
}