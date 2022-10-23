using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API
{
    public static class FileTools
    {
        /// <summary>
        /// Seeks to the offset in the Handle stream. 
        /// <para>If the stream doesn't support Seek, or the offset is invalid, returns 0.</para>
        /// </summary>
        /// <param name="Handle"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte GetByte(in FileStream Handle, int offset)
        {
            if (!Handle.CanSeek) return 255;
            Handle.Seek(offset, SeekOrigin.Begin);
            int value = Handle.ReadByte();
            if (value < 0) return 0;
            if (value > 255) return 255;
            return (byte)value;
        }

        /// <summary>
        /// Reads an 32 bit signed integer, at the index
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <param name="Position">The position to read at</param>
        /// <param name="nPosition">The new file position after reading</param>
        /// <returns></returns>
        public static Int32 ReadInt(byte[] fileData, int Position, out int nPosition)
        {
            nPosition = Position + 4;
            Array.Reverse(fileData);
            return BitConverter.ToInt32(fileData, Position);
        }
        /// <summary>
        /// Reads an 32 bit signed integer, at the filestream position
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <returns></returns>
        public static Int32 ReadInt(FileStream fileData)
        {
            byte[] buffer = new byte[4];
            fileData.Read(buffer, 0, 4);
            return ReadInt(buffer, 0, out _);
        }
        public static UInt32 ReadUInt(byte[] fileData, int Position, out int nPosition)
        {
            nPosition = Position + 4;
            Array.Reverse(fileData);
            return BitConverter.ToUInt32(fileData, Position);
        }
        /// <summary>
        /// Reads an 32 bit signed integer, at the filestream position
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <returns></returns>
        public static UInt32 ReadUInt(FileStream fileData)
        {
            byte[] buffer = new byte[4];
            fileData.Read(buffer, 0, 4);            
            return ReadUInt(buffer, 0, out _);
        }
        public static Int16 ReadShort(byte[] fileData, int Position, out int nPosition)
        {
            nPosition = Position + 2;
            return BitConverter.ToInt16(fileData, Position);
        }
        internal static short ReadShort(FileStream fileData)
        {
            byte[] buffer = new byte[2];
            fileData.Read(buffer, 0, 2);
            return ReadShort(buffer, 0, out _);
        }
        public static UInt16 ReadUShort(byte[] fileData, int Position, out int nPosition)
        {
            nPosition = Position + 2;
            Array.Reverse(fileData);
            return BitConverter.ToUInt16(fileData, Position);
        }
        internal static ushort ReadUShort(FileStream fileData)
        {
            byte[] buffer = new byte[2];
            fileData.Read(buffer, 0, 2);
            return ReadUShort(buffer, 0, out _);
        }

        /// <summary>
        /// Reads a string by taking the length first, then returning the string data
        /// </summary>
        /// <param name="fileData">The file data</param>
        /// <param name="Position">The position to read at</param>
        /// <param name="nPosition">The new file position after reading</param>
        /// <returns></returns>
        public static String ReadString(byte[] fileData, int Position, out int nPosition)
        {
            int strLen = ReadInt(fileData, Position, out Position);
            nPosition = Position + strLen;
            return Encoding.ASCII.GetString(fileData, Position, strLen); 
        }
        public static String ReadString(FileStream fileData, int length)
        {            
            byte[] buffer = new byte[length];
            fileData.Read(buffer, 0, length);
            return Encoding.ASCII.GetString(buffer);
        }

        /// <summary>
        /// Reads a Single at the filestream position
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public static float ReadFloat(FileStream fileData)
        {
            byte[] buffer = new byte[4];
            fileData.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }
    }
}
