using ARCX.Core.Archive;

namespace ARCX.Core.Writers
{
	public class ArcXWriterSettings
	{
		public CompressionType CompressionType { get; set; } = CompressionType.Uncompressed;

		public CompressionFlags CompressionFlags { get; set; } = CompressionFlags.None;

		public int Threads { get; set; } = 1;

		public bool ChunkingEnabled { get; set; } = false;

		public uint TargetChunkSize { get; set; } = 16777216;

		public static ArcXWriterSettings DefaultSettings => new ArcXWriterSettings();
	}
}
