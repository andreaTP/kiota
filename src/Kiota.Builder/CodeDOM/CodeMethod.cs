﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kiota.Builder.CodeDOM;

public enum CodeMethodKind
{
    Custom,
    IndexerBackwardCompatibility,
    RequestExecutor,
    RequestGenerator,
    Serializer,
    Deserializer,
    Constructor,
    Getter,
    Setter,
    ClientConstructor,
    RequestBuilderBackwardCompatibility,
    RequestBuilderWithParameters,
    RawUrlConstructor,
    CommandBuilder,
    /// <summary>
    /// The method to be used during deserialization with the discriminator property to get a new instance of the target type.
    /// </summary>
    Factory,
    /// <summary>
    /// The method to be used during query parameters serialization to get the proper uri template parameter name.
    /// </summary>
    QueryParametersMapper,
}
public enum HttpMethod {
    Get,
    Post,
    Patch,
    Put,
    Delete,
    Options,
    Connect,
    Head,
    Trace
}

public class PagingInformation : ICloneable
{
    public string ItemName
    {
        get; set;
    }

    public string NextLinkName
    {
        get; set;
    }

    public string OperationName
    {
        get; set;
    }

    public object Clone()
    {
        return new PagingInformation
        {
            ItemName = ItemName?.Clone() as string,
            NextLinkName = NextLinkName?.Clone() as string,
            OperationName = OperationName?.Clone() as string,
        };
    }
}

public class CodeMethod : CodeTerminalWithKind<CodeMethodKind>, ICloneable, IDocumentedElement
{
    public static CodeMethod FromIndexer(CodeIndexer originalIndexer, string methodNameSuffix, bool parameterNullable)
    {
        ArgumentNullException.ThrowIfNull(originalIndexer);
        var method = new CodeMethod {
            IsAsync = false,
            IsStatic = false,
            Access = AccessModifier.Public,
            Kind = CodeMethodKind.IndexerBackwardCompatibility,
            Name = originalIndexer.PathSegment + methodNameSuffix,
            Documentation = new () {
                Description = originalIndexer.Documentation.Description,
            },
            ReturnType = originalIndexer.ReturnType.Clone() as CodeTypeBase,
            OriginalIndexer = originalIndexer,
        };
        method.ReturnType.IsNullable = false;
        var parameter = new CodeParameter {
            Name = "id",
            Optional = false,
            Kind = CodeParameterKind.Custom,
            Documentation = new() {
                Description = "Unique identifier of the item",
            },
            Type = originalIndexer.IndexType.Clone() as CodeTypeBase,
        };
        parameter.Type.IsNullable = parameterNullable;
        method.AddParameter(parameter);
        return method;
    }
    public HttpMethod? HttpMethod {get;set;}
    public string RequestBodyContentType { get; set; }
    private HashSet<string> acceptedResponseTypes;
    public HashSet<string> AcceptedResponseTypes {
        get
        {
            acceptedResponseTypes ??= new(StringComparer.OrdinalIgnoreCase);
            return acceptedResponseTypes;
        }
        set
        {
            acceptedResponseTypes = value;
        }
    }
    public AccessModifier Access {get;set;} = AccessModifier.Public;
    private CodeTypeBase returnType;
    public CodeTypeBase ReturnType {get => returnType;set {
        EnsureElementsAreChildren(value);
        returnType = value;
    }}
    private readonly ConcurrentDictionary<string, CodeParameter> parameters = new ();
    public void RemoveParametersByKind(params CodeParameterKind[] kinds) {
        parameters.Where(p => p.Value.IsOfKind(kinds))
                            .Select(static x => x.Key)
                            .ToList()
                            .ForEach(x => parameters.Remove(x, out var _));
    }

    public void ClearParameters()
    {
        parameters.Clear();
    }
    private readonly BaseCodeParameterOrderComparer parameterOrderComparer = new ();
    public IEnumerable<CodeParameter> Parameters { get => parameters.Values.OrderBy(static x => x, parameterOrderComparer); }
    public bool IsStatic {get;set;}
    public bool IsAsync {get;set;} = true;
    public CodeDocumentation Documentation { get; set; } = new();

    public PagingInformation PagingInformation
    {
        get; set;
    }

    /// <summary>
    /// The combination of the path, query and header parameters for the current URL.
    /// Only use this property if the language you are generating for doesn't support fluent API style (e.g. Shell/CLI)
    /// </summary>
    public IEnumerable<CodeParameter> PathQueryAndHeaderParameters
    {
        get; private set;
    }
    public void AddPathQueryOrHeaderParameter(params CodeParameter[] parameters)
    {
        if (parameters == null || !parameters.Any()) return;
        foreach (var parameter in parameters)
        {
            EnsureElementsAreChildren(parameter);
        }
        if (PathQueryAndHeaderParameters == null)
            PathQueryAndHeaderParameters = new List<CodeParameter>(parameters);
        else if (PathQueryAndHeaderParameters is List<CodeParameter> cast)
            cast.AddRange(parameters);
    }
    /// <summary>
    /// The property this method accesses to when it's a getter or setter.
    /// </summary>
    public CodeProperty AccessedProperty { get; set; }
    public bool IsAccessor { 
        get => IsOfKind(CodeMethodKind.Getter, CodeMethodKind.Setter);
    }
    public HashSet<string> SerializerModules { get; set; }
    public HashSet<string> DeserializerModules { get; set; }
    /// <summary>
    /// Indicates whether this method is an overload for another method.
    /// </summary>
    public bool IsOverload { get { return OriginalMethod != null; } }
    /// <summary>
    /// Provides a reference to the original method that this method is an overload of.
    /// </summary>
    public CodeMethod OriginalMethod { get; set; }
    /// <summary>
    /// The original indexer codedom element this method replaces when it is of kind IndexerBackwardCompatibility.
    /// </summary>
    public CodeIndexer OriginalIndexer { get; set; }
    /// <summary>
    /// The base url for every request read from the servers property on the description.
    /// Only provided for constructor on Api client
    /// </summary>
    public string BaseUrl { get; set;
    }

    /// <summary>
    /// This is currently used for CommandBuilder methods to get the original name without the Build prefix & Command suffix.
    /// Avoids regex operations
    /// </summary>
    public string SimpleName { get; set; } = string.Empty;

    private ConcurrentDictionary<string, CodeTypeBase> errorMappings = new();
    
    /// <summary>
    /// Mapping of the error code and response types for this method.
    /// </summary>
    public IOrderedEnumerable<KeyValuePair<string, CodeTypeBase>> ErrorMappings
    {
        get
        {
            return errorMappings.OrderBy(static x => x.Key);
        }
    }
    public void ReplaceErrorMapping(CodeTypeBase oldType, CodeTypeBase newType)
    {
        var codes = errorMappings.Where(x => x.Value == oldType).Select(x => x.Key).ToArray();
        foreach (var code in codes)
        {
            errorMappings[code] = newType;
        }
    }
    public object Clone()
    {
        var method = new CodeMethod {
            Kind = Kind,
            ReturnType = ReturnType?.Clone() as CodeTypeBase,
            Name = Name.Clone() as string,
            HttpMethod = HttpMethod,
            IsAsync = IsAsync,
            Access = Access,
            IsStatic = IsStatic,
            RequestBodyContentType = RequestBodyContentType?.Clone() as string,
            BaseUrl = BaseUrl?.Clone() as string,
            AccessedProperty = AccessedProperty,
            SerializerModules = SerializerModules == null ? null : new (SerializerModules),
            DeserializerModules = DeserializerModules == null ? null : new (DeserializerModules),
            OriginalMethod = OriginalMethod,
            Parent = Parent,
            OriginalIndexer = OriginalIndexer,
            errorMappings = errorMappings == null ? null : new (errorMappings),
            acceptedResponseTypes = acceptedResponseTypes == null ? null : new (acceptedResponseTypes),
            PagingInformation = PagingInformation?.Clone() as PagingInformation,
            Documentation = Documentation?.Clone() as CodeDocumentation,
        };
        if(Parameters?.Any() ?? false)
            method.AddParameter(Parameters.Select(x => x.Clone() as CodeParameter).ToArray());
        return method;
    }

    public void AddParameter(params CodeParameter[] methodParameters)
    {
        if(methodParameters == null || methodParameters.Any(x => x == null))
            throw new ArgumentNullException(nameof(methodParameters));
        if(!methodParameters.Any())
            throw new ArgumentOutOfRangeException(nameof(methodParameters));
        EnsureElementsAreChildren(methodParameters);
        methodParameters.ToList().ForEach(x => parameters.TryAdd(x.Name, x));
    }
    public void AddErrorMapping(string errorCode, CodeTypeBase type)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrEmpty(errorCode);
        errorMappings.TryAdd(errorCode, type);
    }
    public CodeTypeBase GetErrorMappingValue(string key)
    {
        ArgumentException.ThrowIfNullOrEmpty(key);
        if(errorMappings.TryGetValue(key, out var value))
            return value;
        return null;
    }
}
