using System.IO;
using System.Text;

namespace ARCX.Core.Archive
{
	public class ArcXChunk
	{
		public int ID { get; protected set; }

		public CompressionType CompressionType { get; protected set; }

		public CompressionFlags CompressionFlags { get; protected set; }

		public uint Crc32 { get; protected set; }

		public ulong Offset { get; protected set; }

		public ulong CompressedLength { get; protected set; }

		public ulong UncompressedLength { get; protected set; }

		internal ArcXChunk(Stream stream, bool closeStream = true)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

			ID = reader.ReadInt32();
			CompressionType = (CompressionType)reader.ReadByte();
			CompressionFlags = (CompressionFlags)reader.ReadByte();
			Crc32 = reader.ReadUInt32();
			Offset = reader.ReadUInt64();
			CompressedLength = reader.ReadUInt64();
			UncompressedLength = reader.ReadUInt64();

			if (closeStream)
				reader.Close();
		}
	}
}
