using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Concrete;

public static class ScriptManager
{
    public static Assembly cachedAssembly;

    public static byte[] RecompileScripts(string directoryToScan)
    {
        bool NotOfBinOrObj(string file)
        {
            var dir = Path.GetDirectoryName(file);
            var parts = dir.Split(Path.DirectorySeparatorChar);
            foreach (var part in parts) if (part == "bin" || part == "obj") return false;
            return true;
        }

        // scan entire project root recursively for all script files (excluding dotnet temps from bin and obj)
        var scriptPaths = Directory.GetFiles(directoryToScan, "*.cs", SearchOption.AllDirectories).Where(NotOfBinOrObj).ToList();

        if (scriptPaths.Count == 0)
        {
            Debug.Log("No scripts found to compile.");
            cachedAssembly = null;
            return null;
        }

        Debug.Log($"Compiling {scriptPaths.Count} script(s)...");

        var compiledAssembly = CompileScriptsToAssembly(scriptPaths, out var errors, out var dllbytes);

        if (compiledAssembly == null)
        {
            Debug.Log($"Script compilation failed with {errors.Count} errors");
            foreach (var error in errors) Debug.Log(error.ToString());
            cachedAssembly = null;
            return null;
        }

        cachedAssembly = compiledAssembly;

        Debug.Log("Scripts compiled and loaded successfully.");

        return dllbytes;
    }

    public static Assembly CompileScriptsToAssembly(List<string> paths, out List<Diagnostic> errors, out byte[] dllbytes)
    {
        dllbytes = null;
        errors = null;
        
        // parse scripts into syntax trees
        List<SyntaxTree> syntaxTrees = [];
        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            string source = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            syntaxTrees.Add(syntaxTree);
        }

        // get reference to the public api assembly
        string executableDirectory = AppContext.BaseDirectory;
        string sharedAssemblyPath = Path.Combine(executableDirectory, "Shared.dll");
        if (!File.Exists(sharedAssemblyPath))
        {
            Debug.Log($"Error: Shared.dll not found at '{sharedAssemblyPath}'");
            return null;
        }
        var sharedAssemblyReference = MetadataReference.CreateFromFile(sharedAssemblyPath);

        // get references to dotnet runtime
        string[] trustedPlatformAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
        var dotnetRuntimeReferences = trustedPlatformAssembliesPaths.Select(path => MetadataReference.CreateFromFile(path)).ToList();
        
        // combine all references
        List<MetadataReference> references = [];
        references.Add(sharedAssemblyReference);
        references.AddRange(dotnetRuntimeReferences);

        // compile scripts
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create("ScriptsAssembly", syntaxTrees, references, compilationOptions);

        // load il into memory
        using var memoryStream = new MemoryStream();
        var result = compilation.Emit(memoryStream);

        // check for compilation errors
        if (!result.Success)
        {
            errors = result.Diagnostics.Where(diagnosis => diagnosis.Severity == DiagnosticSeverity.Error).ToList();
            return null;
        }

        // return dll bytes
        dllbytes = memoryStream.ToArray();

        // load assembly from il in memory
        var assembly = Assembly.Load(memoryStream.ToArray());

        // return the assembly
        return assembly;
    }

    public static Type GetClassTypeOfScript(string scriptPath)
    {
        string source = File.ReadAllText(scriptPath);
        var regexMatch = Regex.Match(source, @"class\s+([A-Za-z0-9_]+)");
        string className = regexMatch.Groups[1].Value;

        if (cachedAssembly != null)
        {
            var non_recomp_type = cachedAssembly.GetTypes().FirstOrDefault(x => x.Name == className);

            // script assembly already had the compiled script
            if (non_recomp_type != null) return non_recomp_type;
            else
            {
                // script assembly existed but did not contain the script yet
                RecompileScripts(ProjectManager.projectRoot);
                var recomp_type = cachedAssembly.GetTypes().FirstOrDefault(x => x.Name == className);
                return recomp_type;
            }
        }
        else
        {
            // assembly was null, needed to recompile anyway
            RecompileScripts(ProjectManager.projectRoot);
            var recomp_type = cachedAssembly.GetTypes().FirstOrDefault(x => x.Name == className);
            return recomp_type;
        }
    }
}