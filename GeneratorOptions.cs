namespace karsoe_api_generator;

/// <summary>
/// Configuration options for the OpenAPI client generator.
/// </summary>
public class GeneratorOptions
{
    /// <summary>
    /// Path to the OpenAPI specification file (JSON or YAML).
    /// </summary>
    public required string InputPath { get; set; }

    /// <summary>
    /// Output directory for generated files.
    /// </summary>
    public required string OutputDirectory { get; set; }

    /// <summary>
    /// Namespace for the generated code.
    /// </summary>
    public required string Namespace { get; set; }

    /// <summary>
    /// Whether to generate authentication handler.
    /// </summary>
    public bool GenerateAuthHandler { get; set; } = true;

    /// <summary>
    /// Whether to include retry logic with Polly.
    /// </summary>
    public bool EnableRetryPolicy { get; set; } = true;

    /// <summary>
    /// Whether to include logging infrastructure.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Whether to add async suffix to method names.
    /// </summary>
    public bool UseAsyncSuffix { get; set; } = true;

    /// <summary>
    /// Whether to generate README.md file.
    /// </summary>
    public bool GenerateReadme { get; set; } = true;

    /// <summary>
    /// Whether to add validation attributes to models.
    /// </summary>
    public bool AddValidationAttributes { get; set; } = true;

    /// <summary>
    /// Parses command-line arguments and creates GeneratorOptions.
    /// </summary>
    public static GeneratorOptions Parse(string[] args)
    {
        var options = new GeneratorOptions
        {
            InputPath = "swagger.json",
            OutputDirectory = "./Generated",
            Namespace = "GeneratedApi"
        };

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLowerInvariant())
            {
                case "--input":
                case "-i":
                    if (i + 1 < args.Length)
                        options.InputPath = args[++i];
                    break;

                case "--output":
                case "-o":
                    if (i + 1 < args.Length)
                        options.OutputDirectory = args[++i];
                    break;

                case "--namespace":
                case "-n":
                    if (i + 1 < args.Length)
                        options.Namespace = args[++i];
                    break;

                case "--no-auth":
                    options.GenerateAuthHandler = false;
                    break;

                case "--no-retry":
                    options.EnableRetryPolicy = false;
                    break;

                case "--no-logging":
                    options.EnableLogging = false;
                    break;

                case "--no-readme":
                    options.GenerateReadme = false;
                    break;

                case "--no-validation":
                    options.AddValidationAttributes = false;
                    break;

                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
            }
        }

        return options;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("OpenAPI C# Client Generator");
        Console.WriteLine();
        Console.WriteLine("Usage: dotnet run -- [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -i, --input <path>       Path to OpenAPI spec file (default: swagger.json)");
        Console.WriteLine("  -o, --output <path>      Output directory (default: ./Generated)");
        Console.WriteLine("  -n, --namespace <name>   Namespace for generated code (default: GeneratedApi)");
        Console.WriteLine("  --no-auth                Skip authentication handler generation");
        Console.WriteLine("  --no-retry               Skip retry policy generation");
        Console.WriteLine("  --no-logging             Skip logging infrastructure");
        Console.WriteLine("  --no-readme              Skip README.md generation");
        Console.WriteLine("  --no-validation          Skip validation attributes");
        Console.WriteLine("  -h, --help               Show this help message");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  dotnet run -- -i swagger.json -o ./Generated -n MyApp.ApiClient");
    }
}
