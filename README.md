# C# Adaptive Tokenizer

⚠️ **IMPORTANT: This is an educational/proof-of-concept project and is NOT production-ready.**

A specialized tokenizer designed for C# source code that combines regex pattern matching with Byte-Pair Encoding (BPE) fallback strategy. This tokenizer is implemented as a gRPC service, making it easy to integrate into various applications.

## Limitations

This project has several important limitations that make it unsuitable for production use:

- No persistent storage for the vocabulary (vocabulary is lost on service restart)
- No vocabulary versioning or migration support
- Limited error handling and recovery mechanisms
- Limited testing in concurrent scenarios

Consider this project as a learning resource or a starting point for building a more robust solution.

## Features

- 🎯 Specialized for C# syntax tokenization
- 🔄 Adaptive token learning using regex patterns and BPE
- 📡 gRPC service interface
- ⚡ Cascading token processing strategy

## Getting Started

### Prerequisites

- .NET 9.0 or later
- gRPC tools installed

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/cs-adaptive-tokenizer.git
cd cs-adaptive-tokenizer
```

2. Build the project:
```bash
dotnet build
```

3. Run the service:
```bash
dotnet run --project CsAdaptiveTokenizer
```

## Usage

### Service Interface

The tokenizer provides three main operations through its gRPC interface:

1. Load - Initialize the tokenizer with C# source files:
```csharp
var loadReply = await client.LoadAsync(
    new LoadRequest { Path = "../SampleData" });
```

2. Encode - Convert C# code into token IDs:
```csharp
var encodeRequest = new EncodeRequest
{
    DataToEncode = @"
        public class Person
        {
            public string Name { get; set; }
        }"
};
var encodReply = await client.EncodeAsync(encodeRequest);
```

3. Decode - Convert token IDs back to C# code:
```csharp
var decodeRequest = new DecodeRequest();
decodeRequest.DataToDecode.AddRange(encodReply.EncodedData);
var decodeReply = await client.DecodeAsync(decodeRequest);
```

### Client Example

```csharp
using var channel = GrpcChannel.ForAddress("http://localhost:5065");
var client = new Tokenizer.TokenizerClient(channel);

// Load tokenizer with C# files
var loadReply = await client.LoadAsync(
    new LoadRequest { Path = "../SampleData" });

if (!loadReply.IsSuccess)
{
    Console.WriteLine("Failed to load tokenizer.");
    return;
}

// Encode some C# code
var encodeRequest = new EncodeRequest
{
    DataToEncode = "public class Example { }"
};
var encodReply = await client.EncodeAsync(encodeRequest);

// Decode back to text
var decodeRequest = new DecodeRequest();
decodeRequest.DataToDecode.AddRange(encodReply.EncodedData);
var decodeReply = await client.DecodeAsync(decodeRequest);
```

## Architecture

The tokenizer uses a cascading processor architecture:

1. **RegexProcessor**: Primary processor that handles known C# patterns
2. **BpeProcessor**: Fallback processor for unknown tokens using BPE algorithm

The processors are tried in order, with BPE only being used when regex patterns don't match.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
