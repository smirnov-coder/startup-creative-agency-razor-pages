using System;
using System.Collections.Generic;
using System.Text;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional
{
    public static class PagesTestData
    {
        public static IEnumerable<object[]> PublicPages =>
            new List<object[]>
            {
                new object[] { "/", "RazorPages Index" },
                new object[] { "/index", "RazorPages Index" },
                new object[] { "/account/login", "Account Login" },
            };

        public static IEnumerable<object[]> PrivatePages =>
            new List<object[]>
            {
                new object[] { "/admin/blog/add", "Admin Add Blog Post" },
                new object[] { "/admin/blog/edit/1", "Admin Edit Blog Post" },
                new object[] { "/admin/blog", "Admin Blog" },
                new object[] { "/admin/brands/add", "Admin Add Brand" },
                new object[] { "/admin/brands/edit/1", "Admin Edit Brand" },
                new object[] { "/admin/brands", "Admin Brands" },
                new object[] { "/admin/myprofile", "Admin My Profile" },
                new object[] { "/admin/services/add", "Admin Add Service" },
                new object[] { "/admin/services/edit/1", "Admin Edit Service" },
                new object[] { "/admin/services", "Admin Services" },
                new object[] { "/admin/testimonials/add", "Admin Add Testimonial" },
                new object[] { "/admin/testimonials/edit/1", "Admin Edit Testimonial" },
                new object[] { "/admin/testimonials", "Admin Testimonials" },
                new object[] { "/admin/users", "Admin Users" },
                new object[] { "/admin/works/add", "Admin Add Work Example" },
                new object[] { "/admin/works/edit/1", "Admin Edit Work Example" },
                new object[] { "/admin/works", "Admin Works" },
            };

        public static IEnumerable<object[]> AdminOnlyPages =>
            new List<object[]>
            {
                new object[] { "/admin/contacts", "Admin Contacts" },
                new object[] { "/admin/messages", "Admin Messages" },
                new object[] { "/admin/message/1", "Admin Message" },
                new object[] { "/admin/users/manage/user2", "Admin Manage User" },
                new object[] { "/admin/users/register", "Admin Register User" },
            };
    }
}
