using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ARCX.FileSystem.Patcher
{
    public static class Patcher
    {
        private static PatcherFunc CurrentPatcher;

        private static readonly Dictionary<string, PatcherFunc> Patchers = new Dictionary<string, PatcherFunc>
        {
                {"Assembly-CSharp-firstpass.dll", PatchFirstPass},
                {"Assembly-CSharp.dll", PatchAssemblyCSharp}
        };

        private static AssemblyDefinition FileSystemLoader;


        public static IEnumerable<string> TargetDLLs => ProcessAssemblies();

        public static void Patch(AssemblyDefinition ass)
        {
            CurrentPatcher?.Invoke(ass);
        }

        public static void Initialize()
        {
            FileSystemLoader = AssemblyDefinition.ReadAssembly("..\\arcx\\ARCX.FileSystemLoader.dll");
        }

        private static IEnumerable<string> ProcessAssemblies()
        {
            foreach (KeyValuePair<string, PatcherFunc> job in Patchers)
            {
                CurrentPatcher = job.Value;
                yield return job.Key;
            }
        }

        private static void PatchAssemblyCSharp(AssemblyDefinition assCSharp)
        {
            TypeDefinition gameUty = assCSharp.MainModule.GetType("GameUty");
            TypeDefinition hooks = FileSystemLoader.MainModule.GetType("ARCX.FileSystemLoader.Hooks");

            MethodDefinition initArcX = hooks.Methods.FirstOrDefault(m => m.Name == "InitARCX");

            MethodDefinition initMethod = gameUty.Methods.FirstOrDefault(m => m.Name == "Init");

            Instruction ins = initMethod.Body.Instructions[3];
            ILProcessor il = initMethod.Body.GetILProcessor();

            il.InsertAfter(ins, il.Create(OpCodes.Call, assCSharp.MainModule.ImportReference(initArcX)));
        }

        private static void PatchFirstPass(AssemblyDefinition firstpass)
        {
            TypeDefinition fsArchive = firstpass.MainModule.GetType("FileSystemArchive");

            // Make it all virtual so we can override important methods
            foreach (MethodDefinition methodDefinition in fsArchive.Methods.Where(m => !m.IsConstructor && !m.IsVirtual))
                methodDefinition.IsVirtual = true;
        }

        private delegate void PatcherFunc(AssemblyDefinition ass);
    }
}