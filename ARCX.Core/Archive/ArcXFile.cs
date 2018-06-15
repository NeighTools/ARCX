using System;
using System.Collections.Generic;
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

		protected ArcXFile()
		{

		}
	}
}
