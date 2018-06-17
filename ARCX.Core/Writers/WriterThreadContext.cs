using System.IO;
using ARCX.Core.Compressors;

namespace ARCX.Core.Writers
{
	internal class WriterThreadContext
	{
		public Stream ArchiveStream { get; protected set; }

		public ICompressor Compressor { get; protected set; }

		public WriterThreadContext(Stream stream, ICompressor compresor)
		{
			ArchiveStream = stream;
			Compressor = compresor;
		}
	}
}
