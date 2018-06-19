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
        private static extern void asmContrastFilterPro(byte* bitmap, float coeff, int value);

        public void executeAsmContrastFilterPro(byte* bitmap, float coeff, int value)
        {
            asmContrastFilterPro(bitmap, coeff, value);
        }

        [DllImport("Asm.dll")]
        private static extern void asmByteToFloat(byte* bitmap, float* array, int length);

        public void executeAsmByteToFloat(byte* bitmap, float* array, int length)
        {
            asmByteToFloat(bitmap, array, length);
        }

        [DllImport("Asm.dll")]
        private static extern void asmFloatToByte(float* array, byte* bitmap, int length);

        public void executeAsmFloatToByte(float* array, byte* bitmap, int length)
        {
            asmFloatToByte(array, bitmap, length);
        }
    }
}
