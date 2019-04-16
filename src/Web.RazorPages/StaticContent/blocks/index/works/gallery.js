export default function() {
    let $filterItem = $(".gallery__filter-item");
    let gallerySelector = ".gallery";
    let galleryItemSelector = ".gallery__item";
    let activeFilterClass = "gallery__filter-item--active";

    $filterItem.click(function () {
        let $activeFilterItem = $(this);
        let itemFilter = $activeFilterItem.data("filter");

        // Вешаем класс active на соответствующий фильтр.
        $activeFilterItem.removeClass(activeFilterClass).addClass(activeFilterClass);
        // Все остальные фильтры делаем неактивными.
        $activeFilterItem.siblings().each(function () {
            let $inactiveFilterItem = $(this);
            $inactiveFilterItem.removeClass(activeFilterClass);
            if ($inactiveFilterItem.data("filter") === itemFilter) {
                $inactiveFilterItem.addClass(activeFilterClass);
            }
        });

        // Фильтруем галерею.
        $filterItem.parents(gallerySelector).find(galleryItemSelector).each(function () {
            let $galleryItem = $(this);
            let itemGroup = $galleryItem.data("group");
            if (itemFilter === "*" || itemFilter.toUpperCase() === "ALL" || itemFilter.toUpperCase() === itemGroup.toUpperCase()) {
                $galleryItem.show("slow");
            } else {
                $galleryItem.hide("slow");
            }
        });
    });
}