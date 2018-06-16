using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ARCX.Core.Archive;
using ARCX.Core.Compressors;
using ARCX.Core.External;

namespace ARCX.Core.Writers
{
	public class ArcXWriter
	{
		public ArcXWriterSettings Settings { get; protected set; }

		public List<ArcXWriterFile> Files { get; protected set; } = new List<ArcXWriterFile>();

		public ArcXWriter(ArcXWriterSettings settings = null)
		{
			if (settings == null)
				Settings = ArcXWriterSettings.DefaultSettings;
			else
				Settings = settings;
		}

		public void AddFile(ArcXWriterFile file)
		{
			Files.Add(file);
		}

		public void Write(Stream stream, bool keepStreamOpen = false)
		{
			if (!stream.CanSeek)
				throw new ArgumentException("Stream must be seekable.", nameof(stream));

			if (!stream.CanWrite)
				throw new ArgumentException("Stream must be able to be written to.", nameof(stream));

			BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

			//write header
			writer.Write(Encoding.ASCII.GetBytes(ArcXContainer.Magic));
			writer.Write(ArcXContainer.Version);

			long headerPointerOffset = stream.Position;
			writer.Write((ulong)0); //write dummy offset

			var generatedChunks = GenerateChunks();

			if (Settings.Threads > 1)
				throw new NotImplementedException("Multithreading is currently not implemented.");

			foreach (var chunk in generatedChunks)
			{
				long currentStreamOffset = stream.Position;

				using (MemoryStream mem = new MemoryStream())
				{
					foreach (var file in chunk.Value)
						using (Stream fileStream = file.GetStream())
							fileStream.CopyTo(mem);

					mem.Position = 0;


					using (ICompressor compressor = CompressorFactory.GetCompressor(Settings.CompressionType, mem, Settings.CompressionLevel))
					{
						compressor.WriteTo(stream);
					}
				}

				chunk.Key.Offset = (ulong)currentStreamOffset;
				chunk.Key.CompressedLength = (ulong)(stream.Position - currentStreamOffset);

				stream.Position = currentStreamOffset;
				chunk.Key.Crc32 = CRC32.Calculate(stream);
			}

			long headerOffset = stream.Position;

			//write file header
			writer.Write((ulong)generatedChunks.Sum(x => x.Value.LongCount()));

			foreach (var file in generatedChunks.SelectMany(x => x.Value))
			{
				file.Write(writer);
			}

			//write chunk header
			writer.Write(Settings.TargetChunkSize);
			writer.Write((ulong)generatedChunks.LongCount());

			foreach (var chunk in generatedChunks)
			{
				chunk.Key.Write(writer);
			}

			//write header offset
			stream.Seek(headerPointerOffset, SeekOrigin.Begin);
			writer.Write(headerOffset);
			stream.Seek(0, SeekOrigin.End);

			if (!keepStreamOpen)
				writer.Close();
		}

		protected IList<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>> GenerateChunks()
		{
			List<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>> chunks = new List<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>>();
			int currentID = 0;
			
			ArcXWriterChunk currentChunk;
			List<ArcXWriterFile> currentFiles;

			void Reset()
			{
				currentChunk = new ArcXWriterChunk
				{
					ID = currentID++,
					CompressionType = Settings.CompressionType,
					CompressionFlags = Settings.CompressionFlags,
				};

				currentFiles = new List<ArcXWriterFile>();
			}

			Reset();

			foreach (var file in Files)
			{
				if (Settings.ChunkingEnabled)
				{
					if (currentChunk.UncompressedLength + file.Size > Settings.TargetChunkSize && currentFiles.Any())
					{
						chunks.Add(new KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>(currentChunk, currentFiles));
						Reset();
					}

					file.Offset = currentChunk.UncompressedLength;
					currentChunk.UncompressedLength += file.Size;

					currentFiles.Add(file);
				}
				else
				{
					file.Offset = 0;
					currentChunk.UncompressedLength = file.Size;

					currentFiles.Add(file);

					chunks.Add(new KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>(currentChunk, currentFiles));

					Reset();
				}
			}

			if (currentFiles.Any())
				chunks.Add(new KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>(currentChunk, currentFiles));

			return chunks;
		}

		protected class ArcXWriterChunk
		{
			public int ID { get; set; }
			public CompressionType CompressionType { get; set; }
			public CompressionFlags CompressionFlags { get; set; }
			public uint Crc32 { get; set; }
			public ulong Offset { get; set; }
			public ulong CompressedLength { get; set; }
			public ulong UncompressedLength { get; set; }

			public void Write(BinaryWriter writer)
			{
				writer.Write(ID);
				writer.Write((byte)CompressionType);
				writer.Write((byte)CompressionFlags);
				writer.Write(Crc32);
				writer.Write(Offset);
				writer.Write(CompressedLength);
				writer.Write(UncompressedLength);
			}
		}
	}
}
