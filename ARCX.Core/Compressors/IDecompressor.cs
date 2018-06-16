using System;
using System.IO;

namespace ARCX.Core.Compressors
{
	public interface IDecompressor : IDisposable
	{
		Stream BaseStream { get; }

		Stream GetStream();
	}
}
