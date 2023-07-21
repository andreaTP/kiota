using System.Linq;
using Kiota.Builder.CodeDOM;
using Kiota.Builder.Configuration;
using Zio;

namespace Kiota.Builder.CodeRenderers;
public class TypeScriptCodeRenderer : CodeRenderer
{
    public TypeScriptCodeRenderer(GenerationConfiguration configuration, IFileSystem fs) : base(configuration, fs) { }
    public override bool ShouldRenderNamespaceFile(CodeNamespace codeNamespace)
    {
        if (codeNamespace is null) return false;
        return codeNamespace.Interfaces.Any();
    }
}
