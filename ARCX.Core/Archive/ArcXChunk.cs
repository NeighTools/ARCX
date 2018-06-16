using System.IO;
using ARCX.Core.Compressors;

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


		protected Stream BaseStream { get; set; }

		protected ArcXContainer Container { get; set; }


		internal ArcXChunk(Stream stream, ArcXContainer container)
		{
			BinaryReader reader = new BinaryReader(stream);

			ID = reader.ReadInt32();
			CompressionType = (CompressionType)reader.ReadByte();
			CompressionFlags = (CompressionFlags)reader.ReadByte();
			Crc32 = reader.ReadUInt32();
			Offset = reader.ReadUInt64();
			CompressedLength = reader.ReadUInt64();
			UncompressedLength = reader.ReadUInt64();

			BaseStream = stream;
			Container = container;
		}

		public Stream GetRawStream()
		{
			BaseStream.Position = (long)Offset;

			byte[] buffer = new byte[CompressedLength];

			BaseStream.Read(buffer, 0, (int)CompressedLength);

			return new MemoryStream(buffer);
		}

		public Stream GetStream()
		{
			return CompressorFactory.GetDecompressor(CompressionType).GetStream(GetRawStream());
		}
	}
}
