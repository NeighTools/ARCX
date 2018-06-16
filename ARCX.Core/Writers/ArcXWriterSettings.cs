using ARCX.Core.Archive;

namespace ARCX.Core.Writers
{
	public class ArcXWriterSettings
	{
		public CompressionType CompressionType { get; set; } = CompressionType.Zstd;

		public CompressionFlags CompressionFlags { get; set; } = CompressionFlags.None;

		public int CompressionLevel { get; set; } = 3;

		public int Threads { get; set; } = 1;

		public bool ChunkingEnabled { get; set; } = true;

		public uint TargetChunkSize { get; set; } = 16777216;

		public static ArcXWriterSettings DefaultSettings => new ArcXWriterSettings();
	}
}
