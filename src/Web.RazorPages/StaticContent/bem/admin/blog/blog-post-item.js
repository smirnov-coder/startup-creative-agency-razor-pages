import { showBlogPostModal } from "../../common/blog-post-preview";
import showErrors from "../common/errors";

export default function() {
    $(".blog-post-item__view").on("click", viewBlogPost)
}

function viewBlogPost(event) {
    if (event) {
        let $button = $(event.target);
        $.ajax({
            method: $button.attr("formmethod"),
            url: $button.attr("formaction"),
            dataType: "json",
            success: (response) => {
                showBlogPostModal(response);
            },
            error: (response) => {
                showErrors(response);
                //console.log(response, "get blog post error");
            }
        });
    } else {
        throw new Error(`Argument name 'event' was not provided. Current value: ${event}.`);
    }
}