using System;
using System.IO;

namespace ARCX.Core.Compressors
{
	public interface IDecompressor : IDisposable
	{
		Stream GetStream(Stream source);
	}
}
