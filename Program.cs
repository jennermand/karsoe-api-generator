using karsoe_api_generator;

Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         OpenAPI C# Client Generator                          ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

try
{
    // Parse command-line arguments
    var options = GeneratorOptions.Parse(args);

    Console.WriteLine($"Input file: {options.InputPath}");
    Console.WriteLine($"Output directory: {options.OutputDirectory}");
    Console.WriteLine($"Namespace: {options.Namespace}");
    Console.WriteLine();

    // Validate input file exists
    if (!File.Exists(options.InputPath))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: Input file not found: {options.InputPath}");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Use --help for usage information.");
        return 1;
    }

    // Create output directory
    Directory.CreateDirectory(options.OutputDirectory);

    // Step 1: Parse OpenAPI specification
    Console.WriteLine("Step 1: Parsing OpenAPI specification...");
    Console.WriteLine("─────────────────────────────────────────");
    var parser = new OpenApiParser();
    parser.LoadFromFile(options.InputPath);

    // Step 2: Generate Models
    Console.WriteLine();
    Console.WriteLine("Step 2: Generating DTO models...");
    Console.WriteLine("─────────────────────────────────────────");
    var modelGenerator = new ModelGenerator(options);
    modelGenerator.GenerateModels(parser.GetSchemas(), options.OutputDirectory);

    // Step 3: Generate API Client
    Console.WriteLine();
    Console.WriteLine("Step 3: Generating API client...");
    Console.WriteLine("─────────────────────────────────────────");
    var clientGenerator = new ApiClientGenerator(options);
    clientGenerator.GenerateClient(parser, options.OutputDirectory);

    // Step 4: Generate README
    if (options.GenerateReadme)
    {
        Console.WriteLine();
        Console.WriteLine("Step 4: Generating README documentation...");
        Console.WriteLine("─────────────────────────────────────────");
        var readmeGenerator = new ReadmeGenerator(options, parser);
        readmeGenerator.GenerateReadme(
            options.OutputDirectory,
            modelGenerator.GeneratedModels,
            clientGenerator.GeneratedEndpoints);
    }

    // Summary
    Console.WriteLine();
    Console.WriteLine("═════════════════════════════════════════");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✓ Generation completed successfully!");
    Console.ResetColor();
    Console.WriteLine("═════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("Generated files:");
    Console.WriteLine($"  • {modelGenerator.GeneratedModels.Count} model(s) in {Path.Combine(options.OutputDirectory, "Models")}");
    Console.WriteLine($"  • 1 API client in {Path.Combine(options.OutputDirectory, "ApiClient.cs")}");
    if (options.GenerateReadme)
    {
        Console.WriteLine($"  • 1 README.md in {Path.Combine(options.OutputDirectory, "README.md")}");
    }
    Console.WriteLine();
    Console.WriteLine("Next steps:");
    Console.WriteLine("  1. Copy the generated files to your project");
    Console.WriteLine("  2. Install required NuGet packages (see README.md)");
    Console.WriteLine("  3. Configure the API client in your DI container");
    Console.WriteLine("  4. Start using the typed API client!");
    Console.WriteLine();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("═════════════════════════════════════════");
    Console.WriteLine("✗ Generation failed!");
    Console.WriteLine("═════════════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Stack trace:");
    Console.WriteLine(ex.StackTrace);
    Console.WriteLine();

    return 1;
}
