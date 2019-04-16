import { viewBlogPost, showAuthor, showGroup } from "../../common/blog-post-preview";

let $blogForm = $("#getBlogPostsForm");
let $button = $(".blog-post-list__button");
let $loader = $(".blog-post-list__loader");
let buttonHiddenClass = "hidden";
let loaderVisibleClass = "blog-post-list__loader--visible";
const BLOG_POST_COUNT = 2; // ���������� ������������ ����-������ �� ���������.
const DELAY = 350; // �������� ����� ��������� ajax-������� �� ������.

export default function() {
    $blogForm.submit(function (event) {
        // �������� ��������� �� ��������� (�������� �� ������ �� ������).
        event.preventDefault();
        // ������ ������ � �������� ������.
        $button.addClass(buttonHiddenClass);
        $loader.addClass(loaderVisibleClass);
        // ���������� ������������ ����-������ ����� ���������� ��� �����������.
        let skip = $(".blog-post-preview").length;
        // �������� ��������� �������� ��� ������� ����������� ���� ��������.
        setTimeout(() => getBlogPosts(skip, BLOG_POST_COUNT), DELAY);
    });
}

function getBlogPosts(skip, take) {
    let params = { skip, take };
    // �������� ����� �������� �����. (GET)
    let type = $blogForm.attr("method");
    // ������������ ����� GET-������� (Url + QueryString).
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
    // ������ ������ � �������� ������.
    $loader.removeClass(loaderVisibleClass);
    $button.removeClass(buttonHiddenClass);
}

function updateBlogPostList(data) {
    let $loading = $(".blog-post-list__loading");
    // ���� ����� ������� �� ���� (������ ������ ������), ��:
    if (data.trim().length !== 0) {
        let selectors = {
            title: ".blog-post-preview-header__title",
            button: ".blog-post-preview__button",
            authorName: ".blog-post-preview-header__author-name",
            authorGroup: ".blog-post-preview-header__author-group",
            separator: ".blog-post-list__separator",
            preview: ".blog-post-preview"
        };
        // ����������� ����������� ����-������.
        let $separatorClone = $(selectors.separator).eq(0).clone();
        // �������� ����������� ����� ���������� ����-�����.
        $separatorClone.insertAfter($(selectors.preview).last());
        // �������� ���������� � ������� ����-����� � ����� ������.
        let $data = $(data).insertBefore($loading);
        // �������� ��������������� ����������� ������� �� ����������� ����-�����.
        $data.find(selectors.title).add($data.find(selectors.button)).on("click", viewBlogPost);
        $data.find(selectors.authorName).on("click", showAuthor);
        $data.find(selectors.authorGroup).on("click", showGroup);
        showButton();
    } else {
        // ����� ������ ��������� ����-������, �.�. ������ ����� ��������, ��� 
        // ������ ��� ����-������ ��� �����������.
        $loading.addClass("hidden");
    }
}
