using Kiota.Builder.PathSegmenters;
using Zio;

namespace Kiota.Builder.Writers.Java;
public class JavaWriter : LanguageWriter
{
    public JavaWriter(string rootPath, string clientNamespaceName, IFileSystem fs)
    {
        PathSegmenter = new JavaPathSegmenter(rootPath, clientNamespaceName, fs);
        var conventionService = new JavaConventionService();
        AddOrReplaceCodeElementWriter(new CodeClassDeclarationWriter(conventionService));
        AddOrReplaceCodeElementWriter(new CodeBlockEndWriter());
        AddOrReplaceCodeElementWriter(new CodeEnumWriter(conventionService));
        AddOrReplaceCodeElementWriter(new CodeMethodWriter(conventionService));
        AddOrReplaceCodeElementWriter(new CodePropertyWriter(conventionService));
        AddOrReplaceCodeElementWriter(new CodeTypeWriter(conventionService));
    }
}
