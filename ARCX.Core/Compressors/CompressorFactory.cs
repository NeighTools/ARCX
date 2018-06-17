using System.IO;
using ARCX.Core.Archive;

namespace ARCX.Core.Compressors
{
	public static class CompressorFactory
	{
		public static ICompressor GetCompressor(CompressionType type, int compressionLevel = 1)
		{
			switch (type)
			{
				case CompressionType.LZ4:
					return new LZ4Compressor(compressionLevel);
				case CompressionType.Zstd:
					return new ZstdCompressor(compressionLevel);
				case CompressionType.Uncompressed:
				default:
					return new PassthroughCompressor();
			}
		}

		public static IDecompressor GetDecompressor(CompressionType type)
		{
			switch (type)
			{
				case CompressionType.LZ4:
					return new LZ4Decompressor();
				case CompressionType.Zstd:
					return new ZstdDecompressor();
				case CompressionType.Uncompressed:
				default:
					return new PassthroughCompressor();
			}
		}
	}
}
