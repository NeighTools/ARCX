using System.IO;

namespace ARCX.Core.Compressors
{
	public class ZstdCompressor : BaseCompressor
	{
		protected ZstdNet.Compressor BaseCompressor;

		public ZstdCompressor(Stream stream, int level) : base(stream, level)
		{
			BaseCompressor = new ZstdNet.Compressor(new ZstdNet.CompressionOptions(CompressionLevel));
		}

		public override Stream GetStream()
		{
			return new MemoryStream(BaseCompressor.Wrap(BaseStream.ToBytes()));
		}

		public override void WriteTo(Stream destination)
		{
			byte[] compBytes = BaseCompressor.Wrap(BaseStream.ToBytes());

			destination.Write(compBytes, 0, compBytes.Length);
		}
	}

	public class ZstdDecompressor : BaseDecompressor
	{
		protected ZstdNet.Decompressor BaseDecompressor;

		public ZstdDecompressor(Stream stream) : base(stream)
		{
			BaseDecompressor = new ZstdNet.Decompressor();
		}

		public override Stream GetStream()
		{
			return new MemoryStream(BaseDecompressor.Unwrap(BaseStream.ToBytes()));
		}
	}
}
