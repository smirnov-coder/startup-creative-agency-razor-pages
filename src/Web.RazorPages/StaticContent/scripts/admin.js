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
import workExampleItem from "../bem/admin/works/work-example-item";
import blogPostItem from "../bem/admin/blog/blog-post-item";
import contactItem from "../bem/admin/contacts/contact-item";
import messageRow from "../bem/admin/messages/message-row";
import customFileInput from "../bem/admin/common/custom-file-input";
import addCustomValidation from "../bem/common/validation";

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