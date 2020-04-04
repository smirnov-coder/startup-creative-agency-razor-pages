import { showWorkExampleModal } from "../../common/work-example-preview";
import showErrors from "../common/errors";

export default function() {
    $(".work-example-item__view").on("click", viewWorkExample)
}

function viewWorkExample(event) {
    if (event) {
        let $button = $(event.target);
        let url = $button.attr("formaction");
        $.ajax({
            method: $button.attr("formmethod"),
            url: url,
            dataType: "json",
            success: (response) => {
                showWorkExampleModal(response);
            },
            error: (response) => {
                showErrors(response);
                //console.error(`Failed to get work example from ${url}`, response);
            }
        });
    } else {
        throw new Error(`Argument name 'event' was not provided. Current value: ${event}.`);
    }
}