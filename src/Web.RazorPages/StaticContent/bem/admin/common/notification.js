export default function(type, text) {
    let notificationClass = type === "success" ? "alert-success" : "alert-danger";
    let $notification = $(document.createElement("div"));
    $notification.addClass(`notification alert ${notificationClass} alert-dismissible fade in`);
    let $button = $(document.createElement("button"));
    $button.attr({
        "class": "close",
        "type": "button",
        "data-dismiss": "alert"
    });
    let $span = $(document.createElement("span")).html("&times;");
    $(".notifications").prepend($notification.append($button.append($span)).append(text));
}