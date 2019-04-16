function workExamplePreview() {
    let $hoverArea = $(".work-example-preview__inner");
    let $button = $(".work-example-preview__button");
    let overlaySelector = ".work-example-preview__overlay";
    
    $hoverArea.hover(function () {
        $(this).find(overlaySelector).slideToggle();
        // $(this).find(overlaySelector).animate({ width: "toggle" }, 300);
    });

    $button.on("click", function (event) {
        if (event) {
            event.preventDefault();
            let $this = $(this);
            $.ajax({
                method: "get",
                url: $this.attr("formaction"),
                dataType: "json",
                success: (response) => {
                    showWorkExampleModal(response);
                },
                error: (response) => {
                    console.error(`Failed to get work example from ${$this.attr("formaction")}`, response);
                }
            });
        } else {
            throw new Error(`Value for parameter 'event' was not provided. Current value: ${event}.`);
        }
    });
};

function showWorkExampleModal(data) {
    if (data) {
        let $modal = $("#work-example-modal");
        $modal.find(".work-example__title").text(data.Name);
        $modal.find(".work-example__subtitle").text(data.Category);
        $modal.find(".work-example__description").text(data.Description);
        $modal.find(".work-example__img").attr({
            "src": data.ImagePath,
            "alt": data.Name
        });
        $modal.modal("show");
    } else {
        throw new Error(`Value for parameter 'data' was not provided. Current value: ${data}.`);
    }
}

export { workExamplePreview, showWorkExampleModal }