using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Entities;
using Xunit;

namespace StartupCreativeAgency.Infrastructure.Tests.Integration.Repositories
{
    public class TestFiles : IDisposable
    {
        private string _dataDirectory;
        private string _imagesDirectory;

        public string ReadTextFileName => "test-read.txt";
        public string WriteTextFileName => "test-write.txt";
        public string WriteImageFileName => "test-write.jpg";
        public string ReadTextFilePath { get; private set; }
        public string WriteTextFilePath { get; private set; }
        public string WriteImageFilePath { get; private set; }

        public string ReadContactsFileName => "read-contacts.json";
        public string ReadSocialLinksFileName => "read-social-links.json";
        public string ReadContactsFilePath { get; private set; }
        public string ReadSocialLinksFilePath { get; private set; }

        public string WriteContactsFileName => "write-contacts.json";
        public string WriteSocialLinksFileName => "write-social-links.json";
        public string WriteContactsFilePath { get; private set; }
        public string WriteSocialLinksFilePath { get; private set; }

        public TestFiles()
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);
            if (!Directory.Exists(_imagesDirectory))
                Directory.CreateDirectory(_imagesDirectory);

            ReadTextFilePath = Path.Combine(_dataDirectory, ReadTextFileName);
            WriteTextFilePath = Path.Combine(_dataDirectory, WriteTextFileName);
            WriteImageFilePath = Path.Combine(_imagesDirectory, WriteImageFileName);

            ReadContactsFilePath = Path.Combine(_dataDirectory, ReadContactsFileName);
            ReadSocialLinksFilePath = Path.Combine(_dataDirectory, ReadSocialLinksFileName);

            WriteContactsFilePath = Path.Combine(_dataDirectory, WriteContactsFileName);
            WriteSocialLinksFilePath = Path.Combine(_dataDirectory, WriteSocialLinksFileName);

            File.WriteAllText(ReadTextFilePath, "dummy text");
            File.WriteAllText(ReadContactsFilePath, JsonConvert.SerializeObject(new ContactInfo[]
            {
                new ContactInfo("Name #1") { Caption = "Caption #1", Values = new string[] { "Value #1", "Value #2", "Value #3" }},
                new ContactInfo("Name #2") { Caption = "Caption #2", Values = new string[] { "Value #1", "Value #2", "Value #3" }},
                new ContactInfo("Name #3") { Caption = "Caption #3", Values = new string[] { "Value #1", "Value #2" }}
            }));
            File.WriteAllText(ReadSocialLinksFilePath, JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                ["Name #1"] = "Url #1",
                ["Name #2"] = "Url #2",
                ["Name #3"] = "Url #3"
            }));
        }

        public void Dispose()
        {
            if (Directory.Exists(_dataDirectory))
                Directory.Delete(_dataDirectory, true);
            if (Directory.Exists(_imagesDirectory))
                Directory.Delete(_imagesDirectory, true);
        }
    }

    [CollectionDefinition("TestFileCollection")]
    public class TestFileCollection : ICollectionFixture<TestFiles>
    { }
}
