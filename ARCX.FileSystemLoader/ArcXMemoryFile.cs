using System;
using System.IO;
using ARCX.Core.Archive;

namespace ARCX.FileSystemLoader
{
    public class ArcXMemoryFile : AFileBase
    {
        private readonly ArcXFile filePtr;
        private Stream stream;

        public ArcXMemoryFile(ArcXFile filePtr)
        {
            this.filePtr = filePtr;
        }

        public override IntPtr NativePointerToInterfaceFile { get; } = IntPtr.Zero;

        private Stream FileStream => stream ?? (stream = filePtr.GetStream());

        public override bool IsValid()
        {
            return filePtr != null;
        }

        public override int Seek(int f_unPos, bool absolute_move)
        {
            return (int) FileStream.Seek(f_unPos, absolute_move ? SeekOrigin.Begin : SeekOrigin.Current);
        }

        public override int Read(ref byte[] f_byBuf, int f_nReadSize)
        {
            return FileStream.Read(f_byBuf, 0, f_nReadSize);
        }

        public override byte[] ReadAll()
        {
            byte[] result = new byte[filePtr.Size];
            FileStream.Seek(0, SeekOrigin.Begin);
            FileStream.Read(result, 0, result.Length);
            return result;
        }

        public override int Tell()
        {
            return (int) FileStream.Position;
        }

        public override int GetSize()
        {
            return (int) filePtr.Size;
        }

        protected override void Dispose(bool is_release_managed_code)
        {
            stream?.Dispose();
        }
    }
}