using System.IO;

namespace ARCX.Core.Compressors
{
	public abstract class BaseCompressor : ICompressor
	{
		public int CompressionLevel { get; protected set; }

		protected BaseCompressor(int compressionLevel)
		{
			CompressionLevel = compressionLevel;
		}

		public virtual void Dispose()
		{

		}

		public abstract Stream GetStream(Stream source);

		public virtual void WriteTo(Stream source, Stream destination)
		{
			using (Stream stream = GetStream(source))
				stream.CopyTo(destination);
		}
	}
}