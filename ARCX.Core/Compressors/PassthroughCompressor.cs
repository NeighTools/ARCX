using System.IO;

namespace ARCX.Core.Compressors
{
	public class PassthroughCompressor : BaseCompressor, IDecompressor
	{
		public PassthroughCompressor() : base(0)
		{

		}

		public override Stream GetStream(Stream source)
		{
			return source;
		}
	}
}
