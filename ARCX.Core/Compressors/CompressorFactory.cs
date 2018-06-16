﻿using System.IO;
using ARCX.Core.Archive;

namespace ARCX.Core.Compressors
{
	public static class CompressorFactory
	{
		public static ICompressor GetCompressor(CompressionType type, Stream source, int compressionLevel = 1)
		{
			switch (type)
			{
				case CompressionType.LZ4:
					return new LZ4Compressor(source, compressionLevel);
				case CompressionType.Zstd:
					return new ZstdCompressor(source, compressionLevel);
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
				case CompressionType.Zstd:
					return new ZstdDecompressor(source);
				case CompressionType.Uncompressed:
				default:
					return new PassthroughCompressor(source);
			}
		}
	}
}
