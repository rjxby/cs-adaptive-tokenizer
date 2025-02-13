using Grpc.Net.Client;
using CsAdaptiveTokenizer.Contracts;

using var channel = GrpcChannel.ForAddress("http://localhost:5065");
var client = new Tokenizer.TokenizerClient(channel);
var loadReply = await client.LoadAsync(
    new LoadRequest { Path = "../SampleData" });
if (!loadReply.IsSuccess)
{
    Console.WriteLine("Failed to load tokenizer.");
    return;
}

Console.WriteLine("Tokenizer is succcessfully loaded.");

var encodeRequest = new EncodeRequest { DataToEncode = @"
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public void SayHello()
        {
            Console.WriteLine(""Hello, world!"");
        }
    }
" };
var encodReply = await client.EncodeAsync(encodeRequest);
Console.WriteLine("Encoded data: " + encodReply.EncodedData);

var decodeRequest = new DecodeRequest();
decodeRequest.DataToDecode.AddRange(encodReply.EncodedData);
var decodeReply = await client.DecodeAsync(decodeRequest);
Console.WriteLine("Decoded data: " + decodeReply.DecodedData);

Console.WriteLine("Press any key to exit...");
Console.ReadLine();
