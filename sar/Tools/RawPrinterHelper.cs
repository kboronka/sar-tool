using System;
using System.Runtime.InteropServices;

namespace sar.Tools
{
	// code based on: http://support.microsoft.com/kb/322091
    public static class RawPrinterHelper
    {
        #region API declarions
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        #endregion

        private static bool SendBytesToPrinter(string printerName, string documentName, IntPtr data, Int32 length)
        {
            var printerHandle = new IntPtr(0);
            var documentInfo = new DOCINFOA();
            bool success = false;

            documentInfo.pDocName = documentName;
            documentInfo.pDataType = "RAW";

            if (OpenPrinter(printerName.Normalize(), out printerHandle, IntPtr.Zero))
            {
                if (StartDocPrinter(printerHandle, 1, documentInfo))
                {
                    if (StartPagePrinter(printerHandle))
                    {
                    	Int32 dwWritten = 0;
                        success = WritePrinter(printerHandle, data, length, out dwWritten);
                        EndPagePrinter(printerHandle);
                    }
                    
                    EndDocPrinter(printerHandle);
                }
                
                ClosePrinter(printerHandle);
            }
            
			Int32 dwError = 0;            
            if (success == false) dwError = Marshal.GetLastWin32Error();

            return success;
        }

        public static bool SendBytesToPrinter(string printerName, string documentName, Byte[] bytes)
        {
            // unmanaged pointer.
            var data = new IntPtr(0);

            // Allocate some unmanaged memory for those bytes.
            data = Marshal.AllocCoTaskMem(bytes.Length);

            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, data, bytes.Length);

            // Send the unmanaged bytes to the printer.
            bool success = SendBytesToPrinter(printerName, documentName, data, bytes.Length);

            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(data);

            return success;
        }
    }
}
