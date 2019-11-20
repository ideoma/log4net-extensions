namespace log4net.Petabyte.Extensions
{
    public unsafe class AccessorHelper
    {
        internal static void Memcpy(byte* dest, byte* src, int len)
        {
            // AMD64 implementation uses longs instead of ints where possible 
            //
            // <STRIP>This is a faster memcpy implementation, from
            // COMString.cpp.  For our strings, this beat the processor's
            // repeat & move single byte instruction, which memcpy expands into. 
            // (You read that correctly.)
            // This is 3x faster than a simple while loop copying byte by byte, 
            // for large copies.</STRIP> 
            if (len >= 16)
            {
                do
                {
#if AMD64
                    ((long*)dest)[0] = ((long*)src)[0]; 
                    ((long*)dest)[1] = ((long*)src)[1];
#else
                    ((int*) dest)[0] = ((int*) src)[0];
                    ((int*) dest)[1] = ((int*) src)[1];
                    ((int*) dest)[2] = ((int*) src)[2];
                    ((int*) dest)[3] = ((int*) src)[3];
#endif
                    dest += 16;
                    src += 16;
                } while ((len -= 16) >= 16);
            }

            if (len > 0) // protection against negative len and optimization for len==16*N 
            {
                if ((len & 8) != 0)
                {
#if AMD64
                    ((long*)dest)[0] = ((long*)src)[0];
#else
                    ((int*) dest)[0] = ((int*) src)[0];
                    ((int*) dest)[1] = ((int*) src)[1];
#endif
                    dest += 8;
                    src += 8;
                }

                if ((len & 4) != 0)
                {
                    ((int*) dest)[0] = ((int*) src)[0];
                    dest += 4;
                    src += 4;
                }

                if ((len & 2) != 0)
                {
                    ((short*) dest)[0] = ((short*) src)[0];
                    dest += 2;
                    src += 2;
                }

                if ((len & 1) != 0)
                    *dest++ = *src++;
            }
        }
    }
}