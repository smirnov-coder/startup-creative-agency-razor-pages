using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Html;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Web.RazorPages.Areas.Admin.ViewModels;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    /// <summary>
    /// Вспомогательный класс для работы с объектами пользователей доменной модели.
    /// </summary>
    public static class DomainUserHelper
    {
        /// <summary>
        /// Возвращает идентификационную информацию о пользователе доменной модели в виде строки.
        /// </summary>
        /// <param name="user">Пользователь доменной модели.</param>
        /// <returns>
        /// Строка вида '@идентификационное_имя_пользователя (полное_имя_пользователя)'. Если фамилия пользователя не задана, то в 
        /// качестве полного имени возвращается только имя пользователя без фамилии. Если имя пользователя не задано, то
        /// в вместо полного имени возвращается строка '--NotSet--'.
        /// </returns>
        public static string GetUserInfoString(DomainUser user)
        {
            string fullName = GetFullName(user);
            var result = user != null
                ? $"@{user.Identity?.UserName}{(!string.IsNullOrWhiteSpace(fullName) ? $" ({fullName})" : string.Empty)}"
                : "--NotSet--";
            return result;
        }

        /// <summary>
        /// Возвращает полное имя пользователя.
        /// </summary>
        /// <param name="user">Пользователь доменной модели.</param>
        /// <returns>
        /// Строка вида 'имя_пользователя фамилия_пользователя'. Если фамилия пользователя не задана, то 
        /// возвращается только имя пользователя без фамилии. Если имя пользователя не задано, то возвращается строка '--NotSet--'.
        /// </returns>
        public static string GetFullName(DomainUser user)
        {
            var profile = user?.Profile;
            var result = !string.IsNullOrWhiteSpace(profile?.FirstName) && !string.IsNullOrWhiteSpace(profile?.LastName)
                ? $"{profile.FirstName} {profile.LastName}"
                : !string.IsNullOrWhiteSpace(profile?.FirstName)
                    ? profile.FirstName
                    : string.Empty;
            return result;
        }
    }
}
