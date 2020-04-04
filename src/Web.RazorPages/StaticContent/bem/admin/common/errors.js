import addNotification from "../common/notification";

export default function(response) {
    let message = "";
    if (response.responseJSON && response.responseJSON.title && response.responseJSON.errors) {
        let data = response.responseJSON;
        message = `<div>${data.title.trim()}<ul>`;
        Object.keys(data.errors).forEach(function (key) {
            message = message.concat(`<li>${key}<ul>`);
            for (let index in data.errors[key]) {
                message = message.concat(`<li>${data.errors[key][index]}</li>`);
            }
            message = message.concat("</ul></li>");
        });
        message = message.concat("</ul></div>");
    } else if (response.responseText.trim() !== "") {
        message = response.responseText;
    } else {
        message = `Status: ${response.status} ${response.statusText}. Something went wrong. Check server logs.`;
    }
    addNotification("error", message);
}