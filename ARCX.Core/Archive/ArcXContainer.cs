using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ARCX.Core.Archive
{
	public class ArcXContainer
	{
		public static string Magic { get; } = "ARCX";

		public static ushort Version { get; } = 1;

		public ulong HeaderOffset { get; protected set; }

		public IEnumerable<ArcXChunk> Chunks;

		public IEnumerable<ArcXFile> Files { get; protected set; }

		public uint TargetChunkSize { get; protected set; }

		public ArcXContainer(Stream stream, bool closeStream = true)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

			if (Magic != Encoding.ASCII.GetString(reader.ReadBytes(4)))
				throw new InvalidDataException("File is not in an ARCX format.");

			ushort version = reader.ReadUInt16();

			if (Version != version)
				throw new InvalidDataException($"ARCX archive is version {version}, when this reader only supports version {Version}.");

			HeaderOffset = reader.ReadUInt64();

			stream.Position = (long)HeaderOffset;

			//Read header
			ulong fileCount = reader.ReadUInt64();
			List<ArcXFile> files = new List<ArcXFile>((int)fileCount);

			for (ulong i = 0; i < fileCount; i++)
				files.Add(new ArcXFile(stream, false));

			Files = files;

			TargetChunkSize = reader.ReadUInt32();

			ulong chunkCount = reader.ReadUInt64();
			List<ArcXChunk> chunks = new List<ArcXChunk>((int)chunkCount);

			for (ulong i = 0; i < chunkCount; i++)
				chunks.Add(new ArcXChunk(stream, false));

			if (closeStream)
				reader.Close();
		}
	}
}
