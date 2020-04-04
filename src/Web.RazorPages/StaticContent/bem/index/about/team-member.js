export default function() {
    let $teamMember = $(".team-member");
    let overlaySelector = ".team-member__img-overlay";
    
    $teamMember.hover(function() {
        $(this).find(overlaySelector).fadeToggle({ duration: 100 });
    });
}