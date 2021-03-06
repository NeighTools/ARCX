﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARCX.Core.Writers;
using System.IO;
using System.Linq;
using System.Text;
using ARCX.Core.Archive;

namespace ARCX.Core.Tests.Writers
{
	[TestClass]
	public class ArcXWriterTests
	{
		public byte[] TestData = Encoding.ASCII.GetBytes("testdata".PadRight(512, 'X'));

		[TestMethod]
		public void WriteTest()
		{
			var writer = new ArcXWriter();

			writer.AddFile(new ArcXWriterFile("dir/testfilename", () => new MemoryStream(TestData)));

			using (MemoryStream ms = new MemoryStream())
			{
				writer.Write(ms, true);

				ms.Position = 0;

				ArcXContainer container = new ArcXContainer(ms);


				container.Files.First().GetStream();

				using (Stream fs = container.Files.First().GetStream())
				{
					Assert.AreEqual(TestData.Length, fs.Length);

					byte[] buffer = new byte[TestData.Length];

					fs.Read(buffer, 0, TestData.Length);

					for (int i = 0; i < TestData.Length; i++)
					{
						if (buffer[i] != TestData[i])
						{
							Assert.Fail("Incorrect data read from file.");
						}
					}
				}

				container.Files.First().GetStream();
			}
		}

		[TestMethod]
		public void WriteMultipleChunkTest()
		{
			var writer = new ArcXWriter(new ArcXWriterSettings
			{
				TargetChunkSize = 512
			});
			
			writer.AddFile(new ArcXWriterFile("dir/testfilename", () => new MemoryStream(TestData)));
			writer.AddFile(new ArcXWriterFile("dir/testfilename2", () => new MemoryStream(TestData)));

			using (MemoryStream ms = new MemoryStream())
			{
				writer.Write(ms, true);

				ms.Position = 0;

				ArcXContainer container = new ArcXContainer(ms);

				Assert.AreEqual(2, container.Chunks.Count());
			}

			
			writer = new ArcXWriter(new ArcXWriterSettings
			{
				TargetChunkSize = 1024
			});
			
			writer.AddFile(new ArcXWriterFile("dir/testfilename", () => new MemoryStream(TestData)));
			writer.AddFile(new ArcXWriterFile("dir/testfilename2", () => new MemoryStream(TestData)));

			using (MemoryStream ms = new MemoryStream())
			{
				writer.Write(ms, true);

				ms.Position = 0;

				ArcXContainer container = new ArcXContainer(ms);

				Assert.AreEqual(1, container.Chunks.Count());
			}

			
			writer = new ArcXWriter(new ArcXWriterSettings
			{
				ChunkingEnabled = false
			});
			
			writer.AddFile(new ArcXWriterFile("dir/testfilename", () => new MemoryStream(TestData)));
			writer.AddFile(new ArcXWriterFile("dir/testfilename2", () => new MemoryStream(TestData)));

			using (MemoryStream ms = new MemoryStream())
			{
				writer.Write(ms, true);

				ms.Position = 0;

				ArcXContainer container = new ArcXContainer(ms);

				Assert.AreEqual(2, container.Chunks.Count());
			}
		}
	}
}