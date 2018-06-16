using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARCX.Core.Archive;
using ARCX.Core.Writers;

namespace ARCX_CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			args[0] = args[0].ToLowerInvariant();

			Dictionary<string, string> arguments = new Dictionary<string, string>();
			List<string> additionalArgs = new List<string>();

			for (int i = 1; i < args.Length; i++)
			{
				if (!args[i].StartsWith("-")
					&& !args[i].StartsWith("--")
				    && !args[i].StartsWith("/"))
				{
					additionalArgs.AddRange(args.Skip(i));
					break;
				}

				string key;

				if (args[i].StartsWith("--"))
					key = args[i].Substring(2);
				else
					key = args[i].Substring(1);

				arguments.Add(key, args[++i]);
			}

			if (args[0].Contains('c'))
			{
				Compress(arguments, additionalArgs);
			}
		}

		static void Compress(Dictionary<string, string> arguments, List<string> additionalArgs)
		{
			ArcXWriterSettings settings = new ArcXWriterSettings();

			if (arguments.ContainsKey("c:type"))
				switch (arguments["c:type"].ToLowerInvariant())
				{
					case "lz4":
						settings.CompressionType = CompressionType.LZ4;
						break;
					case "zstd":
					default:
						settings.CompressionType = CompressionType.Zstd;
						break;
					case "brotli":
						settings.CompressionType = CompressionType.Brotli;
						break;
					case "lzham":
						settings.CompressionType = CompressionType.LZHAM;
						break;
					case "lzma":
						settings.CompressionType = CompressionType.LZMA;
						break;
				}

			if (arguments.ContainsKey("c:level"))
				settings.CompressionLevel = int.Parse(arguments["c:level"]);

			if (arguments.ContainsKey("c:threads"))
				settings.Threads = int.Parse(arguments["c:threads"]);

			ArcXWriter writer = new ArcXWriter(settings);

			List<Tuple<string, string>> filesList = new List<Tuple<string, string>>();

			foreach (string arg in additionalArgs.Skip(1))
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

			writer.Write(File.Open(additionalArgs[0], FileMode.Create));
		}
	}
}
