export default function() {
    let selectors = {
        fileInput: ".custom-file-input",
        text: ".custom-file-input__text",
        upload: ".custom-file-input__upload"
    };
    $(selectors.fileInput).find(selectors.upload).on("change", function () {
        let fileName = $(this)[0].files[0].name;
        $(this).parents(selectors.fileInput).find(selectors.text).val(fileName);
    });
}