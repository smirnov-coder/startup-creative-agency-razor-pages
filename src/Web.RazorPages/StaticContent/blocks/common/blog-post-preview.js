import smoothScroll from "../common/smooth-scroll";

function blogPostPreview() {
    let selectors = {
        title: ".blog-post-preview-header__title",
        button: ".blog-post-preview__button",
        authorName: ".blog-post-preview-header__author-name",
        authorGroup: ".blog-post-preview-header__author-group"
    };

    $(selectors.title).add(selectors.button).on("click", viewBlogPost);
    $(selectors.authorName).on("click", showAuthor);
    $(selectors.authorGroup).on("click", showGroup);
}

function viewBlogPost(event) {
    if (event) {
        event.preventDefault();
        let $preview = $(event.target).parents(".blog-post-preview");
        let $button = $preview.find(".blog-post-preview__button");
        $.ajax({
            method: "get",
            url: $button.attr("formaction"),
            dataType: "json",
            success: (response) => {
                showBlogPostModal(response);
            },
            error: (response) => {
                console.erro(`Failed to get blog post from ${$button.attr("formaction")}.`, response);
            }
        });
    } else {
        throw new Error(`Argument name 'event' was not provided. Current value: ${event}.`);
    }
}

function showBlogPostModal(data) {
    if (data) {
        let date = new Date(data.CreatedOn).toLocaleDateString(["en-US", "ru-RU"], { year: "numeric", month: "long", day: "numeric" });
        let meta = `${date} By ${data.CreatedBy.Profile.FirstName} ${data.CreatedBy.Profile.LastName} in ${data.Category}`;
        let $modal = $("#blog-post-modal");
        $modal.find(".blog-post__title").text(data.Title);
        $modal.find(".blog-post__meta").text(meta);
        $modal.find(".blog-post__img").attr("src", data.ImagePath);
        $modal.find(".blog-post__content").html(data.Content);
        $modal.modal("show");
    } else {
        throw new Error(`Argument name 'data' was not provided. Current value: ${data}.`);
    }
}

function showAuthor(event) {
    // 1. Узнать имя автора блог-поста.
    let authorName = $(event.target).text().trim();
    
    // 2. Плавно прокрутить страницу к секции 'about'.
    event.data = {
        hash: "#about",
        selector: "html, body"
    };
    smoothScroll(event);
    
    // 3. Определить индекс слайда с нужным автором.
    // Ищем в мусоре, который наплодил OwlCarousel, 'настоящие' элементы team-member.
    // Это те элементы, которые не имеют класса 'cloned'.
    let $team = $(".owl-item:not('.cloned') .team-member");
    // Среди 'настоящих' мемберов, ищем нужного человека по имени.
    let selector = `.team-member__name:contains('${authorName}')`;
    let $teamMember = $team.find(selector).parents(".team-member");
    // Определяем индекс элемента с нужным мембером.
    let index = $team.index($teamMember);
    
    // 5. Прокрутить карусель к нужной позиции.
    $(".team-carousel").trigger("to.owl.carousel", [index, 800]);
}

function showGroup(event) {
    // 1. Получить название группы элементов.
    let group = $(event.target).text().trim();
    
    // 2. Плавно прокрутить страницу к секции 'works'.
    event.data = {
        hash: "#works",
        selector: "html, body"
    };
    smoothScroll(event);
    
    // 3. Отфильтровать галерею.
    let filter = `[data-filter='${group.toLowerCase()}']`;
    // Добавим небольшую задержку для плавности показа.
    // Плавная прокрутка до нужной секции длится 800мс.
    setTimeout(function () {
        $(".gallery__filter-item").filter(filter).click();
    }, 900);
}

export { blogPostPreview, viewBlogPost, showBlogPostModal, showAuthor, showGroup };