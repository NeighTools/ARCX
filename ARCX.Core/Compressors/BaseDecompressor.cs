using System.IO;

namespace ARCX.Core.Compressors
{
	public abstract class BaseDecompressor : IDecompressor
	{
		public Stream BaseStream { get; protected set; }

		protected BaseDecompressor(Stream uncompressedStream)
		{
			BaseStream = uncompressedStream;
		}

		public virtual void Dispose()
		{
			BaseStream.Dispose();
		}

		public abstract Stream GetStream();
	}
}