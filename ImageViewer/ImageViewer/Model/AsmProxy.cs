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

        [DllImport("Asm.dll")]
        private static extern void asmContrastFilter(float* bitmap, float* coeff, int value);

        public void executeAsmContrastFilter(float* bitmap, float* coeff, int value)
        {
            asmContrastFilter(bitmap, coeff, value);
        }

        [DllImport("Asm.dll")]
        private static extern void asmSepiaFilter(byte* bitmap, int stop);

        public void executeAsmSepiaFilter(byte* bitmap, int stop)
        {
            asmSepiaFilter(bitmap, stop);
        }
    }
}
