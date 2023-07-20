using System;
using System.Threading.Tasks;
using Kiota.Builder;
using Kiota.Builder.Configuration;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Runtime.Versioning;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using System.Linq;
using Zio;

// Console.WriteLine("Hello, Wasi Console!");
// KiotaClientGen.Main(args);
await KiotaClientGen.Generate();

public partial class KiotaClientGen
{
    private static readonly CancellationTokenSource source = new CancellationTokenSource();
    private static readonly CancellationToken token = source.Token;

    public static void Main(string[] args)
    {
        // Display the number of command line arguments.
        Console.WriteLine(args.Length);
        Console.WriteLine("Avanti!");

        // File.Create("example-from-wasi.txt");
    }

    // For testing:
    internal async static Task Generate()
    {
        var spec = @"openapi: 3.0.0
info:
  title: Test
  version: 1.0.0
  description: something
paths:
  /api/metrics/v1:
    get:
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/TestList'
          description: Test
      description: Returns a test list
  /api/permissions/v1:
    get:
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/app-permissions'
          description: Test
      description: Returns a app permissions
components:
  schemas:
    app-permissions:
      title: App Permissions
      type: object
      description: The permissions granted to the user-to-server access token.
      properties:
        pages:
          type: string
          description:
            The level of permission to grant the access token to retrieve
            Pages statuses, configuration, and builds, as well as create new builds.
          enum:
            - read
            - write
    TestList:
      type: object
      properties:
        items:
          type: array
          items:
            allOf:
              - $ref: '#/components/schemas/Value'
    Value:
      type: object
      properties:
        additional:
          type: object
          additionalProperties:
            type: string
        values:
          type: array
          items:
            type: string";
        var language = "Java";
        var clientClassName = "ApiClient";
        var namespaceName = "io.demo";
        var includePatterns = String.Empty;
        var excludePatterns = String.Empty;
        // internal async static Task<string> Generate(string spec, string language, string clientClassName, string namespaceName, string includePatterns, string excludePatterns)
        // {
        var cl = new ConsoleLogger();
        ILogger<KiotaBuilder> consoleLogger = cl;

        try
        {
            Console.WriteLine($"Starting to Generate with parameters: {language}, {clientClassName}, {namespaceName}");

            var aggregateFs = new Zio.FileSystems.AggregateFileSystem();
            var memoryFs = new Zio.FileSystems.MemoryFileSystem();
            var zipFs = new Zio.FileSystems.ZipArchiveFileSystem();
            aggregateFs.AddFileSystem(memoryFs);
            aggregateFs.AddFileSystem(zipFs);

            var defaultConfiguration = new GenerationConfiguration();

            string OutputPath = Path.Combine(Path.GetTempPath(), "kiota", "generation", "client");

            Console.WriteLine("1");

            if (memoryFs.FileExists(OutputPath))
            {
                Console.WriteLine("Deleting OutputPath");
                memoryFs.DeleteFile(OutputPath);
            }
            memoryFs.CreateDirectory(OutputPath);

            Console.WriteLine("2");

            string filename = "openapi.";
            if (isJson(spec))
            {
                filename += "json";
            }
            else
            {
                filename += "yaml";
            }

            Console.WriteLine("3");

            string OpenapiFile = Path.Combine(Path.GetTempPath(), filename);
            if (memoryFs.FileExists(OpenapiFile))
            {
                Console.WriteLine("Deleting OpenapiFile");
                memoryFs.DeleteFile(OpenapiFile);
            }
            memoryFs.WriteAllText(OpenapiFile, spec);

            Console.WriteLine("4");

            if (!Enum.TryParse<GenerationLanguage>(language, out var parsedLanguage))
            {
                throw new ArgumentOutOfRangeException($"Not supported language: {language}");
            }

            Console.WriteLine("5");

            var generationConfiguration = new GenerationConfiguration
            {
                OpenAPIFilePath = OpenapiFile,
                IncludePatterns = (includePatterns is null) ? new() : includePatterns?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(static x => x.Trim()).ToHashSet(),
                ExcludePatterns = (excludePatterns is null) ? new() : excludePatterns?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(static x => x.Trim()).ToHashSet(),
                Language = parsedLanguage,
                OutputPath = OutputPath,
                ClientClassName = clientClassName,
                ClientNamespaceName = namespaceName,
                IncludeAdditionalData = false,
                UsesBackingStore = false,
                Serializers = defaultConfiguration.Serializers,
                Deserializers = defaultConfiguration.Deserializers,
                StructuredMimeTypes = defaultConfiguration.StructuredMimeTypes,
                DisabledValidationRules = new(),
                CleanOutput = true,
                ClearCache = true,
            };

            Console.WriteLine("6");

            var builder = new KiotaBuilder(consoleLogger, generationConfiguration, null, memoryFs);
            var result = await builder.GenerateClientAsync(token).ConfigureAwait(false);

            Console.WriteLine("7");

            var zipFilePath = Path.Combine(Path.GetTempPath(), "kiota", "clients", "client.zip");

            if (zipFs.FileExists(zipFilePath))
                zipFs.DeleteFile(zipFilePath);
            else
                zipFs.CreateDirectory(Path.GetDirectoryName(zipFilePath)!);

            Console.WriteLine("8");

            ZipFile.CreateFromDirectory(OutputPath, zipFilePath); // This is not going to work ... right?

            Console.WriteLine(zipFs.ReadAllText(zipFilePath));
        }
        catch (Exception e)
        {
            var errorMessage = "Error:\n" + e + "\nLogs:\n" + cl.GetAllLogs();
            throw new Exception(errorMessage);
        }
    }

    private static bool isJson(string str)
    {
        try
        {
            JsonValue.Parse(str);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}

class ConsoleLogger : ILogger<KiotaBuilder>
{
    private StringBuilder sb = new StringBuilder();
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return new DummyDisposable();
    }

    bool ILogger.IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        sb.AppendLine(formatter(state, exception));
        Console.WriteLine(formatter(state, exception));
    }

    public string GetAllLogs()
    {
        return sb.ToString();
    }
}

class DummyDisposable : IDisposable
{
    public void Dispose()
    {
        // Do nothing
    }
}


// fails with the following exception: - skipping the null check in KiotaBuilder for now
// Unhandled Exception:
// System.Exception: Error:
// System.PlatformNotSupportedException: System.Net.Security is not supported on this platform.
//    at System.Net.Security.SslClientAuthenticationOptions..ctor()
//    at System.Net.Http.SocketsHttpHandler.get_SslOptions()
//    at System.Net.Http.HttpClientHandler.ThrowForModifiedManagedSslOptionsIfStarted()
//    at System.Net.Http.HttpClientHandler.set_ClientCertificateOptions(ClientCertificateOption value)
//    at System.Net.Http.HttpClientHandler..ctor()
//    at System.Net.Http.HttpClient..ctor()
//    at DummyHttpClient..ctor()
//    at KiotaClientGen.Generate()
// Logs:

//    at KiotaClientGen.Generate()
//    at Program.<Main>$(String[] args)
//    at Program.<Main>(String[] args)
class DummyHttpClient : System.Net.Http.HttpClient
{
    public DummyHttpClient()
    {
        // Do nothing - skip initialization that's not supported in wasi
    }
}
