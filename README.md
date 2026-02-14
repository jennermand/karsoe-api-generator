# Karsoe API Generator

A powerful .NET command-line tool that generates strongly-typed C# API clients from OpenAPI/Swagger specifications. Built with modern .NET practices and comprehensive testing.

[![Coverage](https://img.shields.io/badge/coverage-92.39%25-brightgreen)](./Tests/TestResults/CoverageReport/index.html)
[![Tests](https://img.shields.io/badge/tests-123%20passed-success)](./Tests)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

## 🚀 Features

- **Strongly-Typed API Clients** - Generate type-safe C# clients from OpenAPI 3.0+ specifications
- **DTO Model Generation** - Automatic creation of data transfer objects with validation attributes
- **Polly Integration** - Built-in retry policies and resilience patterns
- **Logging Support** - Integrated logging infrastructure with Microsoft.Extensions.Logging
- **Authentication Ready** - Optional authentication handler generation
- **Comprehensive Documentation** - Automatic README generation with usage examples
- **Validation Attributes** - DataAnnotations for required fields, ranges, and patterns
- **XML Documentation** - Full IntelliSense support with generated comments
- **Easy Installation** - Available as a .NET global tool

## 📦 Installation

### As a Global Tool (Recommended)

```bash
# Install from NuGet (after publishing)
dotnet tool install --global Karsoe.ApiGenerator

# Or install from local build
dotnet pack --configuration Release
dotnet tool install --global --add-source ./bin/Release Karsoe.ApiGenerator

# If updating an existing installation:
dotnet tool update --global Karsoe.ApiGenerator

# Or manually uninstall and reinstall:
dotnet tool uninstall --global Karsoe.ApiGenerator
dotnet tool install --global --add-source ./bin/Release Karsoe.ApiGenerator
```

### From Source

```bash
git clone https://github.com/karsoe/api-generator.git
cd karsoe-api-generator
dotnet build
dotnet run -- --help
```

## 🎯 Quick Start

1. **Prepare your OpenAPI specification file** (JSON or YAML)

2. **Generate the API client:**

```bash
karsoe-api-gen -i swagger.json -o ./Generated -n MyApp.ApiClient
```

3. **Copy generated files to your project** and install dependencies:

```bash
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.Extensions.Http.Polly
dotnet add package Polly
dotnet add package System.ComponentModel.Annotations
```

4. **Configure in your DI container:**

```csharp
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
})
.AddPolicyHandler(GetRetryPolicy());
```

5. **Use the generated client:**

```csharp
var client = serviceProvider.GetRequiredService<ApiClient>();
var products = await client.GetProductsAsync();
```

## 📖 Usage

### Basic Usage

```bash
karsoe-api-gen -i <input-file> -o <output-directory> -n <namespace>
```

### Command-Line Options

| Option | Alias | Description | Default |
|--------|-------|-------------|---------|
| `--input` | `-i` | Path to OpenAPI specification file (JSON/YAML) | `swagger.json` |
| `--output` | `-o` | Output directory for generated files | `./Generated` |
| `--namespace` | `-n` | Namespace for generated code | `GeneratedApi` |
| `--no-auth` | - | Skip authentication handler generation | (enabled) |
| `--no-retry` | - | Skip Polly retry policy generation | (enabled) |
| `--no-logging` | - | Skip logging infrastructure | (enabled) |
| `--no-readme` | - | Skip README.md generation | (enabled) |
| `--no-validation` | - | Skip validation attributes on models | (enabled) |
| `--help` | `-h` | Show help message | - |

### Examples

#### Generate with all defaults

```bash
karsoe-api-gen -i api-spec.json
```

Output: `./Generated/` with namespace `GeneratedApi`

#### Custom namespace and output

```bash
karsoe-api-gen -i swagger.json -o ./src/ApiClient -n MyCompany.ApiClient
```

#### Minimal generation (no auth, retry, or logging)

```bash
karsoe-api-gen -i api.json -o ./client -n Api --no-auth --no-retry --no-logging
```

#### Generate without validation attributes

```bash
karsoe-api-gen -i swagger.json --no-validation
```

## 📂 Generated Output

The tool generates the following structure:

```
Generated/
├── ApiClient.cs           # Main API client with all endpoints
├── README.md              # Usage documentation
└── Models/                # DTO models folder
    ├── ProductDTO.cs
    ├── OrderDTO.cs
    └── ...
```

### Generated API Client Features

- **Type-safe methods** for all API endpoints
- **Async/await** support with proper cancellation
- **HttpClient** best practices
- **Exception handling** with meaningful error messages
- **Query string** and **route parameter** handling
- **Request/response** serialization with System.Text.Json
- **Optional retry policies** with Polly
- **Logging support** for debugging and monitoring

### Generated Model Features

- **Nullable reference types** support
- **Data validation attributes** (Required, Range, StringLength, etc.)
- **JSON property names** with proper casing
- **XML documentation** from OpenAPI descriptions
- **Nested object** support
- **Collection types** (List, Array, Dictionary)

## 🛠️ Development

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022, VS Code, or Rider

### Building

```bash
dotnet restore
dotnet build
```

### Running

```bash
dotnet run -- -i swagger.json -o ./Generated -n TestApi
```

### Testing

The project includes comprehensive unit tests with **92.39% code coverage**.

```bash
# Run all tests
dotnet test

# Run tests with coverage
cd Tests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML coverage report
reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:TestResults/CoverageReport -reporttypes:Html
```

### Test Coverage

- **123 unit tests** across all components
- **92.39% line coverage** on core functionality
- **98.73% method coverage**

Coverage breakdown by component:
- TypeMapper: 97.05%
- ModelGenerator: 98.50%
- ReadmeGenerator: 98.76%
- ApiClientGenerator: 87.69%
- OpenApiParser: 80.00%
- GeneratorOptions: 65.07%

### Project Structure

```
karsoe-api-generator/
├── ApiClientGenerator.cs     # API client code generation
├── ModelGenerator.cs          # DTO model generation
├── OpenApiParser.cs           # OpenAPI spec parsing
├── ReadmeGenerator.cs         # Documentation generation
├── TypeMapper.cs              # OpenAPI to C# type mapping
├── GeneratorOptions.cs        # Command-line options
├── Program.cs                 # Entry point
├── Tests/                     # Unit tests
│   ├── ApiClientGeneratorTests.cs
│   ├── ModelGeneratorTests.cs
│   ├── OpenApiParserTests.cs
│   ├── ReadmeGeneratorTests.cs
│   ├── TypeMapperTests.cs
│   └── GeneratorOptionsTests.cs
└── Generated/                 # Sample generated output
```

## 🔧 Advanced Usage

### Integration with CI/CD

```yaml
# GitHub Actions example
- name: Generate API Client
  run: |
    dotnet tool install --global Karsoe.ApiGenerator
    karsoe-api-gen -i api/swagger.json -o src/ApiClient -n MyApp.Client
    
- name: Commit generated code
  run: |
    git add src/ApiClient
    git commit -m "Update API client"
```

### Custom Configuration

For advanced scenarios, you can modify the generated code:

1. **Custom HttpClient configuration:**

```csharp
services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

2. **Custom retry policy:**

```csharp
.AddTransientHttpErrorPolicy(policy => 
    policy.WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

3. **Authentication:**

```csharp
services.AddHttpClient<ApiClient>()
    .AddHttpMessageHandler<AuthenticationHandler>();
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with [Microsoft.OpenApi.Readers](https://github.com/microsoft/OpenAPI.NET)
- Code generation powered by [Scriban](https://github.com/scriban/scriban)
- Resilience patterns with [Polly](https://github.com/App-vNext/Polly)

## ❓ Troubleshooting

### Tool Installation Issues

**Problem**: `dotnet tool install` fails with error

**Solution**:
1. Uninstall existing version first:
   ```bash
   dotnet tool uninstall --global Karsoe.ApiGenerator
   ```

2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet pack --configuration Release
   ```

3. Install from Release folder:
   ```bash
   dotnet tool install --global --add-source ./bin/Release Karsoe.ApiGenerator
   ```

**Problem**: Tool command not found after installation

**Solution**:
1. Verify installation:
   ```bash
   dotnet tool list --global
   ```

2. Ensure .NET tools path is in your PATH environment variable:
   - Windows: `%USERPROFILE%\.dotnet\tools`
   - macOS/Linux: `$HOME/.dotnet/tools`

**Problem**: Pack fails with errors

**Solution**:
1. Check .NET SDK version (requires .NET 10.0+):
   ```bash
   dotnet --version
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/karsoe/api-generator/issues)
- **Discussions**: [GitHub Discussions](https://github.com/karsoe/api-generator/discussions)

## � Publishing to NuGet

This project includes automated GitHub Actions workflows for publishing to NuGet.

### Setup

1. **Get a NuGet API Key**:
   - Go to https://www.nuget.org/
   - Sign in or create an account
   - Go to API Keys → Create
   - Name: `GitHub Actions - Karsoe.ApiGenerator`
   - Select scopes: `Push new packages and package versions`
   - Glob pattern: `Karsoe.*`

2. **Add the API Key to GitHub Secrets**:
   - Go to your GitHub repository → Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Paste your NuGet API key
   - Click "Add secret"

### Publishing a New Version

1. **Update the version** in [karsoe-api-generator.csproj](karsoe-api-generator.csproj):
   ```xml
   <Version>1.0.1</Version>
   ```

2. **Commit and tag the release**:
   ```bash
   git add karsoe-api-generator.csproj
   git commit -m "Bump version to 1.0.1"
   git tag v1.0.1
   git push origin main --tags
   ```

3. The workflow will automatically:
   - ✅ Run all tests (must pass 85% coverage threshold)
   - ✅ Build the project in Release mode
   - ✅ Pack the .NET tool
   - ✅ Publish to NuGet.org
   - ✅ Create a GitHub Release with the package

### Workflows

- **CI Build** (`.github/workflows/ci.yml`): Runs on every push/PR to main
  - Tests on Ubuntu, Windows, and macOS
  - Validates 85% code coverage threshold
  - Generates coverage reports
  
- **Publish to NuGet** (`.github/workflows/publish-nuget.yml`): Runs on version tags
  - Runs full test suite
  - Publishes to NuGet.org
  - Creates GitHub Release

## 🗺️ Roadmap

- [ ] Support for OpenAPI 2.0 (Swagger)
- [ ] Custom template support
- [ ] Multiple language support (TypeScript, Python, etc.)
- [ ] GraphQL support
- [ ] Watch mode for auto-regeneration
- [x] NuGet package publication
- [x] Automated CI/CD with GitHub Actions

---

**Made with ❤️ by Karsoe**
