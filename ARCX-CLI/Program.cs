using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARCX.Core.Writers;

namespace ARCX_CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			args[0] = args[0].ToLowerInvariant();

			if (args[0].Contains('c'))
			{
				Compress(args);
			}
		}

		static void Compress(string[] args)
		{
			ArcXWriter writer = new ArcXWriter(new ArcXWriterSettings
			{
				CompressionLevel = args[0].Contains('h') ? 10 : 0
			});

			List<Tuple<string, string>> filesList = new List<Tuple<string, string>>();

			foreach (string arg in args.Skip(2))
			{
				if (File.Exists(arg))
				{
					filesList.Add(new Tuple<string, string>(Path.GetFileName(arg), arg));
				}
				else if (Directory.Exists(arg))
				{
					filesList.AddRange(Directory.EnumerateFiles(arg, "*", SearchOption.AllDirectories)
						.Select(x => new Tuple<string, string>(x.Substring(arg.Length), x)));
				}
			}

			foreach (var file in filesList)
			{
				writer.AddFile(new ArcXWriterFile(file.Item1, () => File.OpenRead(file.Item2)));
			}

			writer.Write(File.Open(args[1], FileMode.Create));
		}
	}
}
