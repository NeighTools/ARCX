using System.IO;
using System.Linq;
using System.Text;

namespace ARCX.Core.Archive
{
	public class ArcXFile
	{
		public string Filename { get; protected set; }

		public ContentType ContentType { get; protected set; }
		
		public int ChunkID { get; protected set; }

		public ulong Size { get; protected set; }

		public ulong Offset { get; protected set; }


		protected Stream BaseStream { get; set; }

		public ArcXContainer Container { get; set; }

		public ArcXChunk Chunk => Container.Chunks.FirstOrDefault(x => x.ID == ChunkID);


		internal ArcXFile(Stream stream, ArcXContainer container)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

			Filename = reader.ReadString();
			ChunkID = reader.ReadInt32();
			ContentType = (ContentType)reader.ReadUInt16();
			Offset = reader.ReadUInt64();
			Size = reader.ReadUInt64();

			BaseStream = stream;
			Container = container;
		}

		public Stream GetStream()
		{
			byte[] buffer = new byte[Size];

			using (Stream stream = Chunk.GetStream())
			{
				stream.Read(buffer, (int)Offset, (int)Size);
			}

			return new MemoryStream(buffer);
		}
	}
}
