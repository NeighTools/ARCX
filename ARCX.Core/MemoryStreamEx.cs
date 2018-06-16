using System.IO;
using System.Reflection;

namespace ARCX.Core
{
	public class MemoryStreamEx : MemoryStream
	{
		public MemoryStreamEx() : base()
		{
			//set buffer to be exposed, to allow GetBuffer() (why is this even a thing?)
			typeof(MemoryStreamEx).GetField("_exposable", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, true);
		}

		public void TrimBuffer()
		{
			SetLength(Position);
		}
	}
}
