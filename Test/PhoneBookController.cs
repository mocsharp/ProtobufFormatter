using Mocsharp.WebApi.Formatters.Protobuf.Test.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mocsharp.WebApi.Formatters.Protobuf.Test
{
    [RoutePrefix("api/phone")]
    public class PhoneBookController : ApiController
    {
        private static int id = 1;
        public static List<Person> Database = new List<Person>();
        


        static PhoneBookController()
        {
            AddNew(id++, "John", "john@email.com", "111-111-1111", Person.Types.PhoneType.Home);
            AddNew(id++, "Mary", "mary@email.com", "222-222-2222", Person.Types.PhoneType.Mobile);
            AddNew(id++, "Dan", "dan@email.com", "333-333-3333", Person.Types.PhoneType.Work);
        }


        [Route("get/{id}")]
        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            var match = Database.FirstOrDefault(p => p.Id == id);
            return Request.CreateResponse(match == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, match);
        }

        [Route("update")]
        [HttpPost]
        public HttpResponseMessage Update(Person person)
        {
            var match = Database.FirstOrDefault(p => p.Id == person.Id);
            if (match == null)
            {
                person.Id = id++;
                Database.Add(person);
            }
            else
            {
                match.MergeFrom(person);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void AddNew(int id, string name, string email, string phone, Person.Types.PhoneType type)
        {
            Person p = new Person()
            {
                Id = id,
                Name = name,
                Email = email
            };
            p.Phones.Add(new Person.Types.PhoneNumber()
            {
                Number = phone,
                Type = type
            });
            Database.Add(p);
        }
    }
}
