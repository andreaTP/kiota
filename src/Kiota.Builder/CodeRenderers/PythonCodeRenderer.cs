using Kiota.Builder.Configuration;
using Zio;

namespace Kiota.Builder.CodeRenderers;
public class PythonCodeRenderer : CodeRenderer
{
    public PythonCodeRenderer(GenerationConfiguration configuration, IFileSystem fs) : base(configuration, fs, new CodeElementOrderComparerPython()) { }
}
