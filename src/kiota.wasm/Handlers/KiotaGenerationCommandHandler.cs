using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Kiota.Builder;
using Kiota.Builder.Extensions;

using Microsoft.Extensions.Logging;

namespace kiota.Handlers;

public class KiotaGenerationCommandHandler : BaseKiotaCommandHandler
{
    public async Task<int> Invoke()
    {
        string output = "/tmp/test-wasm/test";
        GenerationLanguage language = GenerationLanguage.Java;
        string openapi = "/Users/aperuffo/workspace/apicurio-registry/common/src/main/resources/META-INF/openapi.json";
        bool backingStore = false;
        bool clearCache = true;
        bool includeAdditionalData = false;
        string className = "MyClient";
        string namespaceName = "io.dummy";
        List<string> serializer = new List<string>();
        List<string> deserializer = new List<string>();
        List<string> includePatterns = new List<string>();
        List<string> excludePatterns = new List<string>();
        List<string> disabledValidationRules = new List<string>();
        bool cleanOutput = true;
        List<string> structuredMimeTypes = new List<string>();
        CancellationToken cancellationToken = CancellationToken.None;
        AssignIfNotNullOrEmpty(output, (c, s) => c.OutputPath = s);
        AssignIfNotNullOrEmpty(openapi, (c, s) => c.OpenAPIFilePath = s);
        AssignIfNotNullOrEmpty(className, (c, s) => c.ClientClassName = s);
        AssignIfNotNullOrEmpty(namespaceName, (c, s) => c.ClientNamespaceName = s);
        Configuration.Generation.UsesBackingStore = backingStore;
        Configuration.Generation.IncludeAdditionalData = includeAdditionalData;
        Configuration.Generation.Language = language;
        if(serializer.Any())
            Configuration.Generation.Serializers = serializer.Select(x => x.TrimQuotes()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if(deserializer.Any())
            Configuration.Generation.Deserializers = deserializer.Select(x => x.TrimQuotes()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if(includePatterns.Any())
            Configuration.Generation.IncludePatterns = includePatterns.Select(x => x.TrimQuotes()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if(excludePatterns.Any())
            Configuration.Generation.ExcludePatterns = excludePatterns.Select(x => x.TrimQuotes()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if(disabledValidationRules.Any())
            Configuration.Generation.DisabledValidationRules = disabledValidationRules
                                                                    .Select(x => x.TrimQuotes())
                                                                    .SelectMany(x => x.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                                                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if(structuredMimeTypes.Any())
            Configuration.Generation.StructuredMimeTypes = structuredMimeTypes.SelectMany(x => x.Split(new[] {' '}))
                                                            .Select(x => x.TrimQuotes())
                                                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Configuration.Generation.OpenAPIFilePath = GetAbsolutePath(Configuration.Generation.OpenAPIFilePath);
        Configuration.Generation.OutputPath = NormalizeSlashesInPath(GetAbsolutePath(Configuration.Generation.OutputPath));
        Configuration.Generation.CleanOutput = cleanOutput;
        Configuration.Generation.ClearCache = clearCache;

        var logger = new ConsoleLogger();
        logger.LogTrace("configuration: {configuration}", JsonSerializer.Serialize(Configuration));

        try {
            var result = await new KiotaBuilder(logger, Configuration.Generation, null).GenerateClientAsync(cancellationToken);
            if (result)
                DisplaySuccess("Generation completed successfully");
            else {
                DisplaySuccess("Generation skipped as no changes were detected");
                DisplayCleanHint("generate");
            }
            DisplayInfoHint(language, Configuration.Generation.OpenAPIFilePath);
            DisplayGenerateAdvancedHint(includePatterns, excludePatterns, Configuration.Generation.OpenAPIFilePath);
            return 0;
            } catch (Exception ex) {
    #if DEBUG
                logger.LogCritical(ex, "error generating the client: {exceptionMessage}", ex.Message);
                throw; // so debug tools go straight to the source of the exception when attached
    #else
                logger.LogCritical("error generating the client: {exceptionMessage}", ex.Message);
                return 1;
    #endif
            }
    }
}

class ConsoleLogger : ILogger<KiotaBuilder>
{
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
        Console.WriteLine(formatter(state, exception));
    }
}

class DummyDisposable : IDisposable
{
    public void Dispose()
    {
        // Do nothing
    }
}
