using System;
using System.IO;
using ARCX.Core.Archive;

namespace ARCX.Core.Writers
{
	public class ArcXWriterFile
	{
		public string Filename { get; set; }

		public ContentType ContentType { get; set; }

		internal int ChunkID { get; set; }

		internal ulong Offset { get; set; }

		public ulong Size { get; protected set; }


		public Func<Stream> StreamFunc { get; set; }



		public ArcXWriterFile(string filename, Func<Stream> streamFunc)
		{
			Filename = filename;
			StreamFunc = streamFunc;

			using (Stream stream = streamFunc())
				Size = (ulong)stream.Length;
		}

		public Stream GetStream()
		{
			return StreamFunc.Invoke();
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(Filename);
			writer.Write(ChunkID);
			writer.Write((ushort)ContentType);
			writer.Write(Offset);
			writer.Write(Size);
		}
	}
}
