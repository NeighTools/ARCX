using System.IO;

namespace ARCX.Core
{
	public static class Utility
	{
		public static void CopyTo(this Stream source, Stream destination)
		{
			CopyTo(source, destination, 4096);
		}

		public static void CopyTo(this Stream source, Stream destination, int bufferSize)
		{
			byte[] buffer = new byte[bufferSize];
			int read;

			while ((read = source.Read(buffer, 0, bufferSize)) > 0)
			{
				destination.Write(buffer, 0, read);
			}
		}

		public static byte[] ToBytes(this Stream stream)
		{
			byte[] buffer = new byte[stream.Length - stream.Position];

			stream.Read(buffer, 0, (int)(stream.Length - stream.Position));

			return buffer;
		}
	}
}
