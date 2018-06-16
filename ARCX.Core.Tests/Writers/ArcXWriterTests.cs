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
		[TestMethod]
		public void WriteTest()
		{
			var writer = new ArcXWriter();

			writer.AddFile(new ArcXWriterFile("dir/testfilename", () => new MemoryStream(Encoding.ASCII.GetBytes("testdata"))));

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