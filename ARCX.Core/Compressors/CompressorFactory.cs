using System.IO;
using ARCX.Core.Archive;

namespace ARCX.Core.Compressors
{
	public static class CompressorFactory
	{
		public static ICompressor GetCompressor(CompressionType type, Stream source)
		{
			switch (type)
			{
				case CompressionType.LZ4:
					return new LZ4Compressor(source);
				case CompressionType.Uncompressed:
				default:
					return new PassthroughCompressor(source);
			}
		}

		public static IDecompressor GetDecompressor(CompressionType type, Stream source)
		{
			switch (type)
			{
				case CompressionType.LZ4:
					return new LZ4Decompressor(source);
				case CompressionType.Uncompressed:
				default:
					return new PassthroughCompressor(source);
			}
		}
	}
}
