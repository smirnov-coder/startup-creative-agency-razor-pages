
let $deleteButton = $(`<div class="contact-item__line-delete col-sm-1">
    <button type="button" class="contact-item__delete custom-link">Delete</button>
</div>`);

export default function() {
    $(".contact-item__add").on("click", addContactLine);
    $(".contact-item__delete").on("click", deleteContactLine);
}

function addContactLine(event) {
    let selectors = {
        item: ".contact-item",
        line: ".contact-item__line",
        lineDelete: ".contact-item__line-delete",
        deleteButton: ".contact-item__delete",
        hiddenInput: "input[type=hidden]",
        label: ".contact-item__label",
        textInput: ".contact-item__text-input",
        validationSpan: 'span[class^="field-validation"]'
    };

    let $contactItem = $(event.target).parents(selectors.item);
    let $lastLine = $contactItem.find(selectors.line).last();
    let lineCount = $contactItem.find(selectors.line).length;

    let $newLine = $lastLine.clone();
    if (lineCount === 2) {
        $newLine.append($deleteButton.clone());
    }
    $lastLine.find(selectors.lineDelete).remove();

    let valueIndex = lineCount - 1;
    let contactName = $contactItem.find(selectors.hiddenInput).val();

    let $textInput = $newLine.find(selectors.textInput);
    let id = $textInput.attr("id").replace(/(.+Values)_(\d)__Value$/, `$1_${valueIndex}__Value`);
    let name = $textInput.attr("name").replace(/(.+Values)\[(\d)\]\.Value$/, `$1[${valueIndex}].Value`);
    $textInput.attr({
        "id": id,
        "name": name,
        "value": null
    });
    $newLine.find(selectors.label).text(`${contactName} #${lineCount}`).attr("for", $textInput.attr("id"));
    $newLine.find(selectors.validationSpan).attr("data-valmsg-for", $textInput.attr("name"))
    $newLine.appendTo($contactItem.find(".panel-body")).find(selectors.deleteButton).on("click", deleteContactLine);

    refreshFormValidation($contactItem.parents("form"));
}

function deleteContactLine(event) {
    let selectors = {
        item: ".contact-item",
        line: ".contact-item__line"
    };
    let $contactItem = $(event.target).parents(selectors.item);
    $(event.target).parents(selectors.line).remove();
    let $contactItemLines = $contactItem.find(selectors.line);
    if ($contactItemLines.length > 2) {
        $deleteButton.clone().appendTo($contactItemLines.last()).on("click", deleteContactLine);
    }

    refreshFormValidation($contactItem.parents("form"));
}

function refreshFormValidation($form) {
    $form.removeData("validator").removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse($form);
}