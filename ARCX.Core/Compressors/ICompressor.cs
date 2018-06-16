using System;
using System.IO;

namespace ARCX.Core.Compressors
{
	public interface ICompressor : IDisposable
	{
		Stream BaseStream { get; }

		void WriteTo(Stream stream);

		Stream GetStream();
	}
}
