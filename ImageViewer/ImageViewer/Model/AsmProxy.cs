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
        private static extern void asmNegativeFilter(byte* bitmap, int length);

        public void executeAsmNegativeFilter(byte* bitmap, int length)
        {
            asmNegativeFilter(bitmap, length);
        }

        [DllImport("Asm.dll")]
        private static extern void asmBrightnessFilter(byte* bitmap, int length, byte value);

        public void executeAsmBrightnessFilter(byte* bitmap, int length, byte value)
        {
            asmBrightnessFilter(bitmap, length, value);
        }
    }
}
