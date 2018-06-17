using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARCX.FileSystemLoader
{
    public class ArcXMemoryFile : AFileBase
    {
        public override bool IsValid()
        {
            throw new NotImplementedException();
        }

        public override int Seek(int f_unPos, bool absolute_move)
        {
            throw new NotImplementedException();
        }

        public override int Read(ref byte[] f_byBuf, int f_nReadSize)
        {
            throw new NotImplementedException();
        }

        public override byte[] ReadAll()
        {
            throw new NotImplementedException();
        }

        public override int Tell()
        {
            throw new NotImplementedException();
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool is_release_managed_code)
        {
            throw new NotImplementedException();
        }

        public override IntPtr NativePointerToInterfaceFile { get; } = IntPtr.Zero;
    }
}
