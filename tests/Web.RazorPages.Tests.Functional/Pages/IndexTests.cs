using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Infrastructure;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional.Pages
{
    [Collection("Factories")]
    public class IndexTests
    {
        private readonly CustomWebAppFactories _factoryCollection;

        public IndexTests(CustomWebAppFactories factoryCollection)
        {
            _factoryCollection = factoryCollection;
        }

        [Fact]
        public async Task CanShowServices()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Item = ".service-list__item",
                        Icon = ".service__icon > i",
                        Caption = ".service__caption",
                        Description = ".service__description"
                    };
                    int expectedCount = 3;

                    var services = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, services.Count());
                    Assert.Equal("Class #1", services.First().QuerySelector(selectors.Icon).ClassName);
                    Assert.Equal("Caption #1", services.First().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Equal("Description #1", services.First().QuerySelector(selectors.Description).TextContent.Trim());
                    Assert.Equal("Class #3", services.Last().QuerySelector(selectors.Icon).ClassName);
                    Assert.Equal("Caption #3", services.Last().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Equal("Description #3", services.Last().QuerySelector(selectors.Description).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanShowTeamMembers()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        TeamMember = ".team-member",
                        Image = ".team-member__img",
                        FullName = ".team-member__name",
                        Job = ".team-member__job",
                        SocialLink = ".team-member__socials > .menu__item > a"
                    };
                    int expectedTeamMembersCount = 4,
                        expectedSocialLinksCount = 4;

                    var members = doc.QuerySelectorAll(selectors.TeamMember);

                    Assert.Equal(expectedTeamMembersCount, members.Count());
                    Assert.Equal("Path #1", members.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("FirstName #1 LastName #1", members.First().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("FirstName #1 LastName #1", members.First().QuerySelector(selectors.FullName).TextContent.Trim());
                    Assert.Equal("Job #1", members.First().QuerySelector(selectors.Job).TextContent.Trim());
                    Assert.Equal(expectedSocialLinksCount, members.First().QuerySelectorAll(selectors.SocialLink).Count());
                    Assert.Equal("Link #1", members.First().QuerySelectorAll(selectors.SocialLink).First().GetAttribute("href"));
                    Assert.Equal("Link #4", members.First().QuerySelectorAll(selectors.SocialLink).Last().GetAttribute("href"));

                    Assert.Equal("Path #5", members.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("FirstName #5 LastName #5", members.Last().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("FirstName #5 LastName #5", members.Last().QuerySelector(selectors.FullName).TextContent.Trim());
                    Assert.Equal("Job #5", members.Last().QuerySelector(selectors.Job).TextContent.Trim());
                    Assert.Equal(expectedSocialLinksCount, members.Last().QuerySelectorAll(selectors.SocialLink).Count());
                    Assert.Equal("Link #1", members.Last().QuerySelectorAll(selectors.SocialLink).First().GetAttribute("href"));
                    Assert.Equal("Link #4", members.Last().QuerySelectorAll(selectors.SocialLink).Last().GetAttribute("href"));
                }
            }
        }

        [Fact]
        public async Task CanShowGalleryFilters()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Filter = ".gallery__filter-item",
                        Active = ".gallery__filter-item--active"
                    };
                    int expectedCount = 6;

                    var filters = doc.QuerySelectorAll(selectors.Filter);
                    var activeFilters = doc.QuerySelectorAll(selectors.Active);

                    Assert.Equal(expectedCount, filters.Count());
                    Assert.Single(activeFilters);
                    Assert.Equal("All", filters.First().TextContent.Trim());
                    Assert.Equal("*", filters.First().GetAttribute("data-filter"));
                    Assert.NotEmpty(filters[1].TextContent.Trim());
                    Assert.NotEmpty(filters[1].GetAttribute("data-filter"));
                    Assert.NotEmpty(filters.Last().TextContent.Trim());
                    Assert.NotEmpty(filters.Last().GetAttribute("data-filter"));
                }
            }
        }

        [Fact]
        public async Task CanShowGalleryItems()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Item = ".work-example-preview",
                        Image = ".work-example-preview__img",
                        Title = ".work-example-preview__title",
                        Subtitle = ".work-example-preview__subtitle"
                    };
                    int expectedCount = 9;

                    var items = doc.QuerySelectorAll(selectors.Item);

                    Assert.Equal(expectedCount, items.Count());
                    Assert.Equal("Path #1", items.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Name #1", items.First().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Name #1", items.First().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("Category #1", items.First().QuerySelector(selectors.Subtitle).TextContent.Trim());
                    Assert.Equal("Path #9", items.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Name #9", items.Last().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Name #9", items.Last().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("Category #2", items.Last().QuerySelector(selectors.Subtitle).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanShowBlogPosts()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        BlogPost = ".blog-post-preview",
                        Image = ".blog-post-preview__img",
                        Day = ".blog-post-preview-header__day",
                        Month = ".blog-post-preview-header__month",
                        Title = ".blog-post-preview-header__title",
                        Author = ".blog-post-preview-header__author-name",
                        Group = ".blog-post-preview-header__author-group",
                        Text = ".blog-post-preview__text"
                    };
                    int expectedCount = 2;

                    var blogPosts = doc.QuerySelectorAll(selectors.BlogPost);

                    Assert.Equal(expectedCount, blogPosts.Count());
                    Assert.Equal("Path #4", blogPosts.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #4", blogPosts.First().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Title #4", blogPosts.First().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal(DateTime.Now.Day.ToString(), blogPosts.First().QuerySelector(selectors.Day).TextContent.Trim());
                    Assert.Equal(CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(DateTime.Now.Month).ToUpper(), 
                        blogPosts.First().QuerySelector(selectors.Month).TextContent.Trim().ToUpper());
                    Assert.Equal("FirstName #4 LastName #4", blogPosts.First().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Category #4", blogPosts.First().QuerySelector(selectors.Group).TextContent.Trim());
                    Assert.Equal("Content #4", blogPosts.First().QuerySelector(selectors.Text).InnerHtml.Trim());
                    Assert.Equal("Path #3", blogPosts.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #3", blogPosts.Last().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Title #3", blogPosts.Last().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("Admin Admin", blogPosts.Last().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Category #3", blogPosts.Last().QuerySelector(selectors.Group).TextContent.Trim());
                    Assert.Equal("Content #3", blogPosts.Last().QuerySelector(selectors.Text).InnerHtml.Trim());
                }
            }
        }

        [Fact]
        public async Task CanGetBlogPostsByAjaxRequest()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                int take = 2, skip = 2;
                string url = $"api/BlogPosts/RenderedPreviews?skip={skip}&take={take}";

                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(url)))
                {
                    var selectors = new
                    {
                        BlogPost = ".blog-post-preview",
                        Image = ".blog-post-preview__img",
                        Day = ".blog-post-preview-header__day",
                        Month = ".blog-post-preview-header__month",
                        Title = ".blog-post-preview-header__title",
                        Author = ".blog-post-preview-header__author-name",
                        Group = ".blog-post-preview-header__author-group",
                        Text = ".blog-post-preview__text"
                    };
                    int expectedCount = 2;

                    var blogPosts = doc.QuerySelectorAll(selectors.BlogPost);

                    Assert.Equal(expectedCount, blogPosts.Count());
                    Assert.Equal("Path #2", blogPosts.First().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #2", blogPosts.First().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Title #2", blogPosts.First().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("FirstName #2 LastName #2", blogPosts.First().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Category #2", blogPosts.First().QuerySelector(selectors.Group).TextContent.Trim());
                    Assert.Equal("Content #2", blogPosts.First().QuerySelector(selectors.Text).InnerHtml.Trim());
                    Assert.Equal("Path #1", blogPosts.Last().QuerySelector(selectors.Image).GetAttribute("src"));
                    Assert.Equal("Title #1", blogPosts.Last().QuerySelector(selectors.Image).GetAttribute("alt"));
                    Assert.Equal("Title #1", blogPosts.Last().QuerySelector(selectors.Title).TextContent.Trim());
                    Assert.Equal("FirstName #1 LastName #1", blogPosts.Last().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Category #1", blogPosts.Last().QuerySelector(selectors.Group).TextContent.Trim());
                    Assert.Equal("Content #1", blogPosts.Last().QuerySelector(selectors.Text).InnerHtml.Trim());
                }
            }
        }

        [Fact]
        public async Task CanShowBrands()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Image = ".brands-carousel__img"
                    };
                    int expectedCount = 5;

                    var brandImages = doc.QuerySelectorAll(selectors.Image);

                    Assert.Equal(expectedCount, brandImages.Count());
                    Assert.Equal("Path #1", brandImages.First().GetAttribute("src"));
                    Assert.Equal("Name #1", brandImages.First().GetAttribute("alt"));
                    Assert.Equal("Path #5", brandImages.Last().GetAttribute("src"));
                    Assert.Equal("Name #5", brandImages.Last().GetAttribute("alt"));
                }
            }
        }

        [Fact]
        public async Task CanShowTestimonials()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Testimonial = ".testimonial",
                        Text = ".testimonial__text",
                        Author = ".testimonial__author"
                    };
                    int expectedCount = 3;

                    var testimonials = doc.QuerySelectorAll(selectors.Testimonial);

                    Assert.Equal(expectedCount, testimonials.Count());
                    Assert.Equal("Text #1", testimonials.First().QuerySelector(selectors.Text).TextContent.Trim());
                    Assert.Equal("Author #1, Company #1", testimonials.First().QuerySelector(selectors.Author).TextContent.Trim());
                    Assert.Equal("Text #3", testimonials.Last().QuerySelector(selectors.Text).TextContent.Trim());
                    Assert.Equal("Author #3, Company #3", testimonials.Last().QuerySelector(selectors.Author).TextContent.Trim());
                }
            }
        }

        [Fact]
        public async Task CanShowContacts()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        ContactLine = ".contacts__line",
                        Caption = ".contacts__caption",
                        Text = ".contacts__text"
                    };
                    int expectedCount = 3;

                    var contactLines = doc.QuerySelectorAll(selectors.ContactLine);

                    Assert.Equal(expectedCount, contactLines.Count());
                    Assert.Equal("Caption #1", contactLines.First().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Contains("Value #1", contactLines.First().QuerySelector(selectors.Text).InnerHtml.Trim());
                    Assert.Contains("Value #2", contactLines.First().QuerySelector(selectors.Text).InnerHtml.Trim());
                    Assert.Equal("Caption #3", contactLines.Last().QuerySelector(selectors.Caption).TextContent.Trim());
                    Assert.Contains("Value #1", contactLines.Last().QuerySelector(selectors.Text).InnerHtml.Trim());
                    Assert.Contains("Value #2", contactLines.Last().QuerySelector(selectors.Text).InnerHtml.Trim());
                }
            }
        }

        [Fact]
        public async Task CanSendContactMessage()
        {
            var factory = _factoryCollection.ForAdd;
            using (var httpClient = factory.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Form = "#contactForm",
                        Name = "#Message_Name",
                        Email = "#Message_Email",
                        Subject = "#Message_Subject",
                        Company = "#Message_Company",
                        Text = "#Message_Text"
                    };
                    var message = new Message
                    {
                        Name = "Test Name",
                        Email = "test@example.com",
                        Subject = "Test Subject",
                        Company = "Test Company",
                        Text = "Test Text"
                    };

                    var form = doc.QuerySelector(selectors.Form) as IHtmlFormElement;
                    form.QuerySelector(selectors.Name).SetAttribute("value", message.Name);
                    form.QuerySelector(selectors.Email).SetAttribute("value", message.Email);
                    form.QuerySelector(selectors.Subject).SetAttribute("value", message.Subject);
                    form.QuerySelector(selectors.Company).SetAttribute("value", message.Company);
                    form.QuerySelector(selectors.Text).SetAttribute("value", message.Text);

                    var request = new HttpRequestMessage
                    {
                        Method = new HttpMethod(form.Method),
                        RequestUri = new Uri(form.Action, UriKind.Relative),
                        Content = new FormUrlEncodedContent(form.GetInputValues())
                    };

                    using (var response = await httpClient.SendAsync(request))
                    {
                        Assert.True(response.IsSuccessStatusCode);
                        string responseMessage = HttpUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
                        Assert.Contains("Thank you for your message!", responseMessage);
                        using (var scope = factory.Server.Host.Services.CreateScope())
                        {
                            using (var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                var result = db.Messages.LastOrDefault();
                                Assert.NotNull(result);
                                Assert.Equal(message.Name, result.Name);
                                Assert.Equal(message.Email, result.Email);
                                Assert.Equal(message.Subject, result.Subject);
                                Assert.Equal(message.Company, result.Company);
                                Assert.Equal(message.Text, result.Text);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public async Task CanShowFooter()
        {
            using (var httpClient = _factoryCollection.ForRead.CreateClient())
            {
                HtmlParser parser = new HtmlParser();
                using (var doc = await parser.ParseDocumentAsync(await httpClient.GetStringAsync(httpClient.BaseAddress)))
                {
                    var selectors = new
                    {
                        Link = ".footer__socials-item > a"
                    };
                    int expectedCount = 4;

                    var links = doc.QuerySelectorAll(selectors.Link);

                    Assert.Equal(expectedCount, links.Count());
                    Assert.Equal("Link #1", links.First().GetAttribute("href"));
                    Assert.Equal("Link #4", links.Last().GetAttribute("href"));
                }
            }
        }
    }
}
