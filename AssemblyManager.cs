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
        public static bool TryBuildAndExecuteAssembly(string source, string entryPoint)
        {
            Console.WriteLine($"\nAttempting to build {source.Length} byte source...");
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
            var refs = AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.Location.Equals(string.Empty)).Select(asm => MetadataReference.CreateFromFile(asm.Location));
            CSharpCompilation compiler = CSharpCompilation.Create("MessageCompilation " + Guid.NewGuid(), new[] { syntaxTree }, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (MemoryStream ms = new MemoryStream())
            {
                var result = compiler.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                GC.Collect();
                if (result.Success)
                {
                    var asm = Assembly.Load(ms.ToArray());
                    try
                    {
                        ExecuteAssembly(asm, entryPoint);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Caught exception:\n{e}");
                        return false;
                    }
                    
                    
                }
                else
                {
                    Console.WriteLine("Compiler Error:");
                    foreach (Diagnostic diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic.ToString());
                    }
                    return false;
                }
            }
        }

        private static void ExecuteAssembly(Assembly asm, string entryPoint)
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
