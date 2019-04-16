using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional
{
    public static class TestHelper
    {
        public static MultipartFormDataContent CreateTestMultipartFormDataContent(
            IEnumerable<KeyValuePair<string, string>> formInputValues, 
            string fileInputName, 
            string fileName)
        {
            var result = new MultipartFormDataContent();
            foreach (var formInput in formInputValues)
            {
                result.Add(new StringContent(formInput.Value ?? string.Empty), formInput.Key);
            }
            result.Add(new ByteArrayContent(new byte[1024]), fileInputName, fileName);
            return result;
        }
    }
}
