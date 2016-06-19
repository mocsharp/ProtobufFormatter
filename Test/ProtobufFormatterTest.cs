using Google.Protobuf;
using Mocsharp.WebApi.Formatters.Protobuf.Test.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mocsharp.WebApi.Formatters.Protobuf.Test
{
    public class ProtobufFormatterTest : IClassFixture<ServerFixture>
    {
        ServerFixture _fixture;
        HttpClient _client;

        public ProtobufFormatterTest(ServerFixture fixture)
        {
            _fixture = fixture;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DownloadTest(int id)
        {
            var req = await _client.GetAsync($"http://localhost:{_fixture.Port}/api/phone/get/{id}");
            req.EnsureSuccessStatusCode();
            var person = await req.Content.ReadAsAsync<Person>(GetFormatter());

            var expectedPerson = PhoneBookController.Database.ElementAt(id - 1);
            Assert.Equal(expectedPerson.Id, person.Id);
            Assert.Equal(expectedPerson.Name, person.Name);
            Assert.Equal(expectedPerson.Email, person.Email);
            Assert.Equal(expectedPerson.Phones.Count, person.Phones.Count);
            Assert.Equal(expectedPerson.Phones.ElementAt(0).Number, person.Phones.ElementAt(0).Number);
            Assert.Equal(expectedPerson.Phones.ElementAt(0).Type, person.Phones.ElementAt(0).Type);

        }

        [Fact]
        public async Task UploadTest()
        {
            Person newPerson = New();

            using (var ms = new MemoryStream())
            {
                var gs = new CodedOutputStream(ms);
                newPerson.WriteTo(gs);
                gs.Flush();
                gs.Dispose();
                HttpContent content = new ByteArrayContent(ms.ToArray());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
                var res = await _client.PostAsync($"http://localhost:{_fixture.Port}/api/phone/update", content);
                res.EnsureSuccessStatusCode();
            }

            var added = PhoneBookController.Database.Last();
            Assert.NotNull(added);
            Assert.Equal(PhoneBookController.Database.Count, added.Id);
            Assert.Equal(newPerson.Name, added.Name);
            Assert.Equal(newPerson.Email, added.Email);
            Assert.Equal(newPerson.Phones.Count, added.Phones.Count);
            Assert.Equal(newPerson.Phones.ElementAt(0).Number, added.Phones.ElementAt(0).Number);
            Assert.Equal(newPerson.Phones.ElementAt(0).Type, added.Phones.ElementAt(0).Type);
            Assert.Equal(newPerson.Phones.ElementAt(1).Number, added.Phones.ElementAt(1).Number);
            Assert.Equal(newPerson.Phones.ElementAt(1).Type, added.Phones.ElementAt(1).Type);

        }



        [Fact]
        public async Task UploadTest_UnsupportedDataType()
        {
            var foo = new Foo { Data = "Hello World!" };
            var res = await _client.PostAsJsonAsync<Foo>($"http://localhost:{_fixture.Port}/api/phone/update", foo);
            Exception ex = Record.Exception(() => res.EnsureSuccessStatusCode());

            Assert.NotNull(ex);
        }

        [Fact]
        public async Task UploadTest_UnsupportedMediaType()
        {

            Person newPerson = New();
            var res = await _client.PostAsJsonAsync<Person>($"http://localhost:{_fixture.Port}/api/phone/update", newPerson);
            Exception ex = Record.Exception(() => res.EnsureSuccessStatusCode());

            Assert.NotNull(ex);

        }

        private Person New()
        {
            Person newPerson = new Person()
            {
                Email = "stephen@email.com",
                Name = "Stephen"
            };

            newPerson.Phones.Add(new Person.Types.PhoneNumber()
            {
                Number = "777-777-7777",
                Type = Person.Types.PhoneType.Mobile
            });

            newPerson.Phones.Add(new Person.Types.PhoneNumber()
            {
                Number = "888-888-8888",
                Type = Person.Types.PhoneType.Home
            });

            return newPerson;
        }

        private List<MediaTypeFormatter> GetFormatter()
        {
            var formatters = new List<MediaTypeFormatter>();
            formatters.Add(new ProtoBufFormatter());
            return formatters;
        }
    }

    public class Foo
    {
        public string Data { get; set; }
    }
}
