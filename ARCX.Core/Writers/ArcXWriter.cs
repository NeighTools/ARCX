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

		#region Threading
		
		internal WriterThreadContext[] ThreadContexts;

		protected Thread[] Threads;

		protected ConcurrentQueue<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>> QueuedChunks;

		public void CompressCallback(object threadContext)
		{
			var context = (WriterThreadContext)threadContext;

			while (QueuedChunks.Count > 0)
			{
				bool result = QueuedChunks.TryDequeue(out var chunk, 250);

				if (!result) 
					continue;

				long currentStreamOffset = context.ArchiveStream.Position;

				using (MemoryStream mem = new MemoryStream())
				{
					foreach (var file in chunk.Value)
						using (Stream fileStream = file.GetStream())
							fileStream.CopyTo(mem);

					mem.Position = 0;

					using (Stream compressedBuffer = context.Compressor.GetStream(mem))
					{
						lock (context.ArchiveStream)
						{
							compressedBuffer.CopyTo(context.ArchiveStream);

							chunk.Key.Offset = (ulong)currentStreamOffset;
							chunk.Key.CompressedLength = (ulong)(context.ArchiveStream.Position - currentStreamOffset);

							context.ArchiveStream.Position = currentStreamOffset;
							chunk.Key.Crc32 = CRC32.Calculate(context.ArchiveStream);
						}
					}
				}

				//Aggressive GC collections
				GC.Collect();
			}
		}

		protected void InitializeThreads(int threads, IEnumerable<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>> chunks, Stream fileStream, CompressionType compressionType, int compressionLevel)
		{
			QueuedChunks = new ConcurrentQueue<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>>(chunks);
			
			Threads = new Thread[threads];
			ThreadContexts = new WriterThreadContext[threads];

			for (int i = 0; i < threads; i++)
			{
				Threads[i] = new Thread(CompressCallback);

				var ctx = new WriterThreadContext(fileStream, CompressorFactory.GetCompressor(compressionType, compressionLevel));
				
				ThreadContexts[i] = ctx;
				Threads[i].Start(ctx);
			}
		}

		protected void FinalizeThreads()
		{
			foreach (WriterThreadContext ctx in ThreadContexts)
			{
				ctx.Compressor.Dispose();
			}
		}

		protected void WaitForThreadCompletion()
		{
			foreach (Thread thread in Threads)
				thread.Join();
		}

		#endregion

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

			//do work
			InitializeThreads(Settings.Threads, generatedChunks, stream, Settings.CompressionType, Settings.CompressionLevel);
			
			WaitForThreadCompletion();

			FinalizeThreads();

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

		protected void PreprocessFiles()
		{
			Files = Files
				.OrderBy(x => Path.GetExtension(x.Filename))
				.ThenBy(x => x.Size)
				.ToList();
		}

		protected IList<KeyValuePair<ArcXWriterChunk, IList<ArcXWriterFile>>> GenerateChunks()
		{
			PreprocessFiles();

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
