using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer.Model
{
    public unsafe class AsmProxy
    {
        [DllImport("Asm.dll")]
        private static extern byte[] asmNegativeFilter(byte[] bitmap);

        public byte[] executeAsmNegativeFilter(byte[] bitmap)
        {
            return asmNegativeFilter(bitmap);
        }
    }
}
