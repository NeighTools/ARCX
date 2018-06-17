using System;
using System.IO;

namespace ARCX.Core.Compressors
{
	public interface ICompressor : IDisposable
	{
		void WriteTo(Stream source, Stream destination);

		Stream GetStream(Stream source);
	}
}
