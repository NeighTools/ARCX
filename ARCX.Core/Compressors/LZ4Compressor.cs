using System.IO;
using System.IO.Compression;
using LZ4;

namespace ARCX.Core.Compressors
{
	public class LZ4Compressor : BaseCompressor
	{
		public LZ4Compressor(int compressionLevel) : base(compressionLevel)
		{

		}

		public override Stream GetStream(Stream source)
		{
			MemoryStream mem = new MemoryStream();

			LZ4StreamFlags flags = LZ4StreamFlags.IsolateInnerStream |
			                       (CompressionLevel > 1 ? LZ4StreamFlags.HighCompression : LZ4StreamFlags.None);

			using (var lz4 = new LZ4Stream(mem, CompressionMode.Compress, flags, 4194304))
			{
				source.CopyTo(lz4);
			}

			mem.SetLength(mem.Length); //trims excess off buffer
			mem.Position = 0;

			return mem;
		}

		public override void WriteTo(Stream source, Stream destination)
		{
			LZ4StreamFlags flags = LZ4StreamFlags.IsolateInnerStream |
			                       (CompressionLevel > 1 ? LZ4StreamFlags.HighCompression : LZ4StreamFlags.None);

			using (var lz4 = new LZ4Stream(destination, CompressionMode.Compress, flags, 4194304))
			{
				source.CopyTo(lz4);
			}
		}
	}

	public class LZ4Decompressor : BaseDecompressor
	{
		public override Stream GetStream(Stream source)
		{
			return new LZ4Stream(source, CompressionMode.Decompress, LZ4StreamFlags.IsolateInnerStream);
		}
	}
}
