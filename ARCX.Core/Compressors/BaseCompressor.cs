﻿using System.IO;

namespace ARCX.Core.Compressors
{
	public abstract class BaseCompressor : ICompressor
	{
		public Stream BaseStream {get; protected set; }

		protected BaseCompressor(Stream uncompressedStream)
		{
			BaseStream = uncompressedStream;
		}

		public virtual void Dispose()
		{
			BaseStream.Dispose();
		}

		public abstract Stream GetStream();

		public abstract void WriteTo(Stream stream);
	}
}