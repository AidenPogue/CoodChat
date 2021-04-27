using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoodChat
{
    class AssemblyManager
    {
        public static bool TryBuildAssembly(string source, out byte[] bytes, out EmitResult result)
        {
            Console.WriteLine($"Attempting to build {source.Length} byte source...")
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
            var refs = AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.Location.Equals(string.Empty)).Select(asm => MetadataReference.CreateFromFile(asm.Location));
            CSharpCompilation compiler = CSharpCompilation.Create("MessageCompilation " + Guid.NewGuid(), new[] { syntaxTree }, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (MemoryStream ms = new MemoryStream())
            {
                result = compiler.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                GC.Collect();
                bytes = ms.ToArray();
                return result.Success;
            }
        }

        public static void ExecuteAssembly(Assembly asm, string entryPoint)
        {
            Console.WriteLine($"Executing assembly {asm.GetName().Name} at entry pount {entryPoint}");
            int lastPeriod = entryPoint.LastIndexOf('.');
            string classPath = entryPoint.Substring(0, lastPeriod);
            string method = entryPoint.Substring(lastPeriod + 1);
            object returned = asm.GetType(classPath).GetMethod(method).Invoke(null, null);
            if (returned != null) Console.WriteLine($"Returned : {returned}");
        }
    }
}
