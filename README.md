# Веб-сайт креативного агентства "Startup"

## Описание

Сайт состоит из главной страницы, выполненной в виде Landing page, и многостраничной админки, позволяющей изменять содержимое отдельных секций главной страницы.

Дизайн сайта создан на основе бесплатного [PSD-шаблона](https://www.behance.net/gallery/31927243/Startup-Free-Creative-Agency-PSD-Template), найденного на просторах интернета. Вёрстка по методологии [БЭМ](https://ru.bem.info/) (CSS) адаптивная для главной страницы и без адаптивности для админки.

Пользователи сайта делятся на 3 категории:

1. Обычные (анонимные) пользователи. Могут просматривать содержимое главной страницы и отправлять контактное сообщение (секция `Contact`).

2. Зарегистрированные пользователи - рядовые сотрудники агентства (дизайнеры, менеджеры, программисты и т.д.). Могут добавлять, изменять и удалять описание услуг компании (секция `Services`), галерею примеров выполненных компанией проектов (секция `Works`), информацию о корпоративных клиентах и отзывы индивидуальных пользователей услуг компании (секция `Clients`), вести корпоративный блог (секция `Blog`).

3. Администраторы. Имеют полный доступ к управлению сайтом. В дополнение к возможностям рядовых сотрудников, могут изменять контактные данные компании (секция `Contact` и футер), просматривать контактные сообщения от пользователей, отправленные с главной страницы, регистрировать новых и удалять существующих сотрудников, управлять отображением команды сотрудников компании (секция `About`).

Используемые при разработке технологии и библиотеки:

- Backend
  - [ASP.NET Core 2.1 Razor Pages](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/)
  - [ASP.NET Core 2.1 Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity/)
  - [O/RM Entity Framework Core](https://docs.microsoft.com/en-us/ef/)
  - [SQLite](https://www.sqlite.org/) (для упрощения развёртывания)
  - [xUnit](https://xunit.net/) + [Microsoft TestServer](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests/) (модульное, интеграционное, функциональное тестирование)
  - [Webpack](https://webpack.js.org/)
  - [SASS](https://sass-lang.com/) (+ [PostCSS](https://postcss.org/))

- Frontend
  - [Bootstrap 3](https://getbootstrap.com/docs/3.4/)
  - [jQuery](https://jquery.com/)
  - [jQuery Validation](https://jqueryvalidation.org/)
  - [jQuery Validation Unobtrusive](https://github.com/aspnet/jquery-validation-unobtrusive/)
  - [OwlCarousel 2](https://owlcarousel2.github.io/OwlCarousel2/)
  - [FontAwesome 4](https://fontawesome.com/v4.7.0/)

## Скриншоты

### Главная страница

![Home page](/../screenshots/home-page.png)

### Админка

![Admin panel](/../screenshots/admin-panel.png)

Проект предназначен исключительно для демонстрации моих умений и навыков потенциальным работодателям и вряд ли представляет какую-либо практическую ценность из-за значительного упрощения.

Благодарю за внимание. :relaxed:
