using System.IO;
using System.IO.Compression;
using LZ4;

namespace ARCX.Core.Compressors
{
	public class LZ4Compressor : BaseCompressor
	{
		public LZ4Compressor(Stream stream) : base(stream)
		{

		}

		public override Stream GetStream()
		{
			MemoryStream mem = new MemoryStream();
			using (var lz4 = new LZ4Stream(mem, CompressionMode.Compress, LZ4StreamFlags.IsolateInnerStream))
			{
				BaseStream.CopyTo(lz4);
			}

			mem.Position = 0;

			return mem;
		}
	}

	public class LZ4Decompressor : BaseDecompressor
	{
		public LZ4Decompressor(Stream stream) : base(stream)
		{

		}

		public override Stream GetStream()
		{
			return new LZ4Stream(BaseStream, CompressionMode.Decompress, LZ4StreamFlags.IsolateInnerStream);
		}
	}
}
