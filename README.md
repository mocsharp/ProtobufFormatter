# ProtobufFormatter
A Protobuf WebAPI formatter

See [Protocol Buffer Basics: C#](https://developers.google.com/protocol-buffers/docs/csharptutorial) to get started.

#Server 
```csharp
HttpConfiguration config = new HttpConfiguration();
config.Formatters.Insert(0, new ProtoBufFormatter());
...
app.UseWebApi(config);
```

#Client
##Get
```csharp
HttpClient client = new HttpClient();
client.DefaultRequestHeaders.Accept.Clear(); //optional
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));
var req = await client.GetAsync("http://localhost:12345/api/home");
req.EnsureSuccessStatusCode();
var person = await req.Content.ReadAsAsync<Person>(GetFormatter());

//----------------------------------------

private static List<MediaTypeFormatter> GetFormatter()
{
  var formatters = new List<MediaTypeFormatter>();
  formatters.Add(new HolxProtoBufFormatter());
  return formatters;
}
```
##Post
```csharp
Person newPerson = new Person()
{
  Email = "my@email.com",
  Id = 50,
  Name = "Joe"
};

using (var ms = new MemoryStream())
{
  using(var gs = new CodedOutputStream(ms))
  {
    newPerson.WriteTo(gs);
  }
  var content = new ByteArrayContent(ms.ToArray());
  content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
  var res = await client.PostAsync("http://localhost:12345/api/home/some-post-api", content);
  res.EnsureSuccessStatusCode();
}
```
