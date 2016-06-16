using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using Google.Protobuf;

namespace ProtobufLib
{
    public class ProtoBufFormatter : BufferedMediaTypeFormatter
    {
        public ProtoBufFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-protobuf"));
        }


        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var val = value as IMessage;
            val.WriteTo(writeStream);
        }

        public override object ReadFromStream(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            try
            {
                var messageParserType = typeof(MessageParser<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(messageParserType, MakeGenericFunc(type));
                var parserMethod = messageParserType.GetMethod("ParseFrom",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.Any,
                    new[] {typeof(Stream)},
                    null);

                return parserMethod.Invoke(instance, new[] {readStream});
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private T CallConstructor<T>() where T : new()
        {
            return new T();
        }

        private object MakeGenericFunc(Type type)
        {
            var generic = typeof(ProtoBufFormatter).GetMethod("CallConstructor",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var constructed = generic.MakeGenericMethod(type);
            var delType = typeof(Func<>).MakeGenericType(type);
            return constructed.CreateDelegate(delType, this);
        }


        public override bool CanReadType(Type type)
        {
            return CanReadWriteType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return CanReadWriteType(type);
        }

        private bool CanReadWriteType(Type type)
        {
            var interfaceType = typeof(IMessage<>).MakeGenericType(type);
            return type.GetInterfaces().Contains(interfaceType);
        }
    }
}
