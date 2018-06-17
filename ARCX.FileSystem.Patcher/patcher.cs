using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ARCX.FileSystem.Patcher
{
    public static class Patcher
    {
        private static Action<AssemblyDefinition> CurrentPatcher;
	    private static AssemblyDefinition FileSystemLoader;

        private static readonly Dictionary<string, Action<AssemblyDefinition>> Patchers = new Dictionary<string, Action<AssemblyDefinition>>
        {
                {"Assembly-CSharp-firstpass.dll", PatchFirstPass},
                {"Assembly-CSharp.dll", PatchAssemblyCSharp}
        };

	    public static IEnumerable<string> TargetDLLs
	    {
		    get
		    {
			    foreach (var job in Patchers)
			    {
				    CurrentPatcher = job.Value;
				    yield return job.Key;
			    }
		    }
	    }

        public static void Patch(AssemblyDefinition ass)
        {
            CurrentPatcher?.Invoke(ass);
        }

        private static string CombinePaths(params string[] paths) => paths.Aggregate(Path.Combine);

        public static void Initialize()
        {
            FileSystemLoader = AssemblyDefinition.ReadAssembly(CombinePaths(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "ARCX.FileSystemLoader.dll"));
        }

	    public static void Finish()
	    {
			FileSystemLoader.Dispose();
	    }

        private static void PatchAssemblyCSharp(AssemblyDefinition assCSharp)
        {
            TypeDefinition gameUty = assCSharp.MainModule.GetType("GameUty");

            TypeDefinition hooks = FileSystemLoader.MainModule.GetType("ARCX.FileSystemLoader.Hooks");

            MethodDefinition initArcX = hooks.Methods.First(m => m.Name == "InitARCX");

            MethodDefinition initMethod = gameUty.Methods.First(m => m.Name == "Init");

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
    }
}