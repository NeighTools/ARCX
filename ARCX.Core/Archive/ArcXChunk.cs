using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCX.Core.Archive
{
	public class ArcXChunk
	{
		public CompressionType CompressionType { get; protected set; }

		public CompressionFlags CompressionFlags { get; protected set; }

		public uint Crc32 { get; protected set; }

		public ulong Offset { get; protected set; }

		public ulong CompressedLength { get; protected set; }

		public ulong UncompressedLength { get; protected set; }

		protected ArcXChunk()
		{

		}
	}
}
