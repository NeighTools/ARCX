using System.IO;

namespace ARCX.Core.Compressors
{
	public abstract class BaseDecompressor : IDecompressor
	{
		protected BaseDecompressor()
		{
			
		}

		public virtual void Dispose()
		{

		}

		public abstract Stream GetStream(Stream source);
	}
}