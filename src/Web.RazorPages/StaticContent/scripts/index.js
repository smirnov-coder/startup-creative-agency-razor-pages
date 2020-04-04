//
// Подключение css-библиотек.
//
import "../lib/bootstrap-customized/css/bootstrap.css"; // customized Twitter Bootstrap 3.x.x
import "font-awesome/css/font-awesome.css"; // FontAwesome 4.x.x
import "owl.carousel/dist/assets/owl.carousel.css"; // OwlCarousel 2.x.x

//
// Подключение пользовательских стилей.
//
import "../styles/index/index.scss";

//
// Подключение js-библиотек.
//
// Подключать библиотеку jQuery явно не требуется, так как jQuery объявляется 
// глобальной функцией (того требует Bootstrap и различные плагины jQuery) 
// в файле конфигурации webpack.
import "../lib/bootstrap-customized/js/bootstrap"
import "owl.carousel/dist/owl.carousel";
import "jquery-validation/dist/jquery.validate";
import "jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive";

//
// Подключение пользовательских js-файлов.
//
import customNavbar from "../bem/index/header/custom-navbar";
import customNav from "../bem/index/header/custom-nav";
import header from "../bem/index/header/header";
import teamCarousel from "../bem/index/about/team-carousel";
import teamMember from "../bem/index/about/team-member";
import gallery from "../bem/index/works/gallery";
import { workExamplePreview } from "../bem/common/work-example-preview";
import parallaxSection from "../bem/index/parallax/parallax-section";
import { blogPostPreview } from "../bem/common/blog-post-preview";
import blogPostList from "../bem/index/blog/blog-post-list";
import brandsCarousel from "../bem/index/clients/brands-carousel";
import testimonialsSlider from "../bem/index/clients/testimonials-slider";
import contactForm from "../bem/index/contact/contact-form";

$(document).ready(() => {
    customNavbar();
    customNav();
    header();
    teamCarousel();
    teamMember();
    gallery();
    workExamplePreview();
    parallaxSection();
    blogPostPreview();
    blogPostList();
    brandsCarousel();
    testimonialsSlider();
    contactForm();
});

//
// Webpack Hot Module Replacement - обновление ресурсов бандла без перезагрузки 
// страницы (там, где это возможно, конечно).
//
if (module.hot) {
    module.hot.accept();
}
