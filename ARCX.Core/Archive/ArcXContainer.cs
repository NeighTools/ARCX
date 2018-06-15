using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCX.Core.Archive
{
	public class ArcXContainer
	{
		public string Magic { get; } = "ARCX";

		public ushort Version { get; } = 1;

		public ulong HeaderOffset { get; protected set; }

		public IEnumerable<ArcXChunk> Chunks;

		public IEnumerable<ArcXFile> Files { get; protected set; }

		public uint TargetChunkSize { get; protected set; }

		protected ArcXContainer()
		{

		}
	}
}
