using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARCX.Core.Writers;
using System.IO;
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
				var fg = container.Chunks;
			}
		}
	}
}