import { viewBlogPost, showAuthor, showGroup } from "../../common/blog-post-preview";

let $blogForm = $("#getBlogPostsForm");
let $button = $(".blog-post-list__button");
let $loader = $(".blog-post-list__loader");
let buttonHiddenClass = "hidden";
let loaderVisibleClass = "blog-post-list__loader--visible";
const BLOG_POST_COUNT = 2; // Количество отображаемых блог-постов по умолчанию.
const DELAY = 350; // Задержка перед отправкой ajax-запроса на сервер.

export default function() {
    $blogForm.submit(function (event) {
        // Отменить поведение по умолчанию (отправку на данных на сервер).
        event.preventDefault();
        // Скрыть кнопку и показать лоадер.
        $button.addClass(buttonHiddenClass);
        $loader.addClass(loaderVisibleClass);
        // Количество пропускаемых блог-постов равно количеству уже отображённых.
        let skip = $(".blog-post-preview").length;
        // Добавить небольшую задержку для лучшего отображения хода загрузки.
        setTimeout(() => getBlogPosts(skip, BLOG_POST_COUNT), DELAY);
    });
}

function getBlogPosts(skip, take) {
    let params = { skip, take };
    // Получить метод отправки формы. (GET)
    let type = $blogForm.attr("method");
    // Сформировать адрес GET-запроса (Url + QueryString).
    let url = `${$blogForm.attr("action")}?${$.param(params)}`;
    $.ajax({
        type: type,
        url: url,
        dataType: "text",
        success: updateBlogPostList,
        error: showButton
    });
}

function showButton() {
    // Скрыть лоадер и показать кнопку.
    $loader.removeClass(loaderVisibleClass);
    $button.removeClass(buttonHiddenClass);
}

function updateBlogPostList(data) {
    let $loading = $(".blog-post-list__loading");
    // Если ответ сервера не пуст (пустая строка текста), то:
    if (data.trim().length !== 0) {
        let selectors = {
            title: ".blog-post-preview-header__title",
            button: ".blog-post-preview__button",
            authorName: ".blog-post-preview-header__author-name",
            authorGroup: ".blog-post-preview-header__author-group",
            separator: ".blog-post-list__separator",
            preview: ".blog-post-preview"
        };
        // Скопировать разделитель блог-постов.
        let $separatorClone = $(selectors.separator).eq(0).clone();
        // Вставить разделитель после последнего блог-поста.
        $separatorClone.insertAfter($(selectors.preview).last());
        // Вставить полученные с сервера блог-посты в конец списка.
        let $data = $(data).insertBefore($loading);
        // Навесить соответствующие обработчики событий на вставленные блог-посты.
        $data.find(selectors.title).add($data.find(selectors.button)).on("click", viewBlogPost);
        $data.find(selectors.authorName).on("click", showAuthor);
        $data.find(selectors.authorGroup).on("click", showGroup);
        showButton();
    } else {
        // Иначе скрыть загрузчик блог-постов, т.к. пустой ответ означает, что 
        // больше нет блог-постов для отображения.
        $loading.addClass("hidden");
    }
}
