using System.Collections.Generic;
using System.IO;
using ARCX.Core.Writers;
using CM3D2.Toolkit.Arc;

namespace ARCX_CLI
{
	public static class ArcReader
	{
		public static bool Read(string path, out List<ArcXWriterFile> files)
		{
			files = new List<ArcXWriterFile>();

			ArcFileSystem fs = new ArcFileSystem();

			if (!fs.LoadArc(path))
				return false;

			foreach (var file in fs.Files)
			{
				files.Add(new ArcXWriterFile(file.FullName.Replace("CM3D2ToolKit:\\\\", ""), () => new MemoryStream(file.Pointer.Compressed ? file.Pointer.Decompress().Data : file.Pointer.Data)));
			}

			return true;
		}
	}
}
