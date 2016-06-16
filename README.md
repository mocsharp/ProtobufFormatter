# ProtobufFormatter
A Protobuf WebAPI formatter

See [Protocol Buffer Basics: C#](https://developers.google.com/protocol-buffers/docs/csharptutorial) to get started.

#Server 
```
HttpConfiguration config = new HttpConfiguration();
config.Formatters.Insert(0, new ProtoBufFormatter());
...
app.UseWebApi(config);
```

#Client
```
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
