using System.IO;

namespace ARCX.Core.Compressors
{
	public class PassthroughCompressor : BaseCompressor, IDecompressor
	{
		public PassthroughCompressor(Stream stream) : base(stream)
		{

		}

		public override Stream GetStream()
		{
			return BaseStream;
		}

		public override void WriteTo(Stream stream)
		{
			BaseStream.CopyTo(stream);
		}
	}
}
