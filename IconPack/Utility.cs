using System.IO;
using System.Runtime.InteropServices;

namespace IconPack
{
    /// <summary>
    /// Holds a set of utilities.
    /// </summary>
    public static class Utility
    {
        #region Stream Utilities
        /// <summary>
        /// Reads a structure of type T from the input stream.
        /// </summary>
        /// <typeparam name="T">The structure type to be read.</typeparam>
        /// <param name="inputStream">The input stream to read from.</param>
        /// <returns>A structure of type T that was read from the stream.</returns>
        public static T ReadStructure<T>(Stream inputStream) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var buffer = new byte[size];
            inputStream.Read(buffer, 0, size);
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr, size);
            var ret = Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return (T)ret;
        }
        /// <summary>
        /// Writes as structure of type T to the output stream.
        /// </summary>
        /// <typeparam name="T">The structure type to be written.</typeparam>
        /// <param name="outputStream">The output stream to write to.</param>
        /// <param name="structure">The structure to be written.</param>
        public static void WriteStructure<T>(Stream outputStream, T structure) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var buffer = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);
            outputStream.Write(buffer, 0, size);
        }
        #endregion
    }
}
