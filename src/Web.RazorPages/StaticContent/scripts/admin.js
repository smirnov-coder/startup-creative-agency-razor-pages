//
// Подключение css-библиотек.
//
import "../lib/bootstrap-customized/css/bootstrap.css"; // customized Twitter Bootstrap 3.x.x
import "font-awesome/css/font-awesome.css"; // FontAwesome 4.x.x

//
// Подключение пользовательских стилей.
//
import "../styles/admin/admin.scss";

//
// Подключение js-библиотек.
//
// Подключать библиотеку jQuery явно не требуется, так как jQuery объявляется 
// глобальной функцией (того требует Bootstrap и различные плагины jQuery) 
// в файле конфигурации webpack.
import "../lib/bootstrap-customized/js/bootstrap"
import "jquery-validation/dist/jquery.validate";
import "jquery-validation/dist/additional-methods";
import "jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive";

//
// Подключение пользовательских js-файлов.
//
import workExampleItem from "../blocks/admin/works/work-example-item";
import blogPostItem from "../blocks/admin/blog/blog-post-item";
import contactItem from "../blocks/admin/contacts/contact-item";
import messageRow from "../blocks/admin/messages/message-row";
import customFileInput from "../blocks/admin/common/custom-file-input";
import addCustomValidation from "../blocks/common/validation";

$(document).ready(function () {
    addCustomValidation();
    workExampleItem();
    blogPostItem();
    contactItem();
    messageRow();
    customFileInput();
});

//
// Webpack Hot Module Replacement - обновление ресурсов бандла без перезагрузки 
// страницы (там, где это возможно, конечно).
//
if (module.hot) {
    module.hot.accept();
}