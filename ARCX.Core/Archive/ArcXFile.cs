using System;
using System.Collections.Generic;
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

		internal ArcXFile(Stream stream, bool closeStream = true)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

			Filename = reader.ReadString();
			ChunkID = reader.ReadInt32();
			ContentType = (ContentType)reader.ReadUInt16();
			Offset = reader.ReadUInt64();
			Size = reader.ReadUInt64();

			if (closeStream)
				reader.Close();
		}
	}
}
