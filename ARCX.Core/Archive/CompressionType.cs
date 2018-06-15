using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCX.Core.Archive
{
	public enum CompressionType : byte
	{
		Uncompressed = 0,
		LZ4 = 1,
		Zstd = 2,
		Brotli = 3,
		LZHAM = 4,
		LZMA = 5,
	}
}
