using System.IO;

namespace ARCX.Core.Compressors
{
	public class ZstdCompressor : BaseCompressor
	{
		protected ZstdNet.Compressor BaseCompressor;

		public ZstdCompressor(int compressionLevel) : base(compressionLevel)
		{
			BaseCompressor = new ZstdNet.Compressor(new ZstdNet.CompressionOptions(CompressionLevel));
		}

		public override Stream GetStream(Stream source)
		{
			return new MemoryStream(BaseCompressor.Wrap(source.ToBytes()));
		}

		public override void WriteTo(Stream source, Stream destination)
		{
			byte[] compBytes = BaseCompressor.Wrap(source.ToBytes());

			destination.Write(compBytes, 0, compBytes.Length);
		}

		public override void Dispose()
		{
			BaseCompressor.Dispose();
		}

		~ZstdCompressor()
		{
			Dispose();
		}
	}

	public class ZstdDecompressor : BaseDecompressor
	{
		protected ZstdNet.Decompressor BaseDecompressor = new ZstdNet.Decompressor();

		public override Stream GetStream(Stream source)
		{
			return new MemoryStream(BaseDecompressor.Unwrap(source.ToBytes()));
		}

		public override void Dispose()
		{
			BaseDecompressor.Dispose();
		}

		~ZstdDecompressor()
		{
			Dispose();
		}
	}
}
