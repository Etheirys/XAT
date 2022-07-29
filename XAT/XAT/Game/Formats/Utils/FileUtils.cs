using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace XAT.Game.Formats.Utils;

public static class FileUtils
{
    public static void WriteEncodedString(this BinaryWriter writer, string str, bool writeNull = false)
    {
        writer.Write(Encoding.ASCII.GetBytes(str));
        if (writeNull) writer.Write((byte)0);
    }

    public static string ReadEncodedString(this BinaryReader reader, int count = -1)
    {
        if(count == -1)
        {
            List<byte> strBytes = new();
            byte b;

            while ((b = reader.ReadByte()) != 0x00)
                strBytes.Add(b);

            return Encoding.ASCII.GetString(strBytes.ToArray());
        }
        else
        {
            return Encoding.ASCII.GetString(reader.ReadBytes(count));
        }
    }

    public static Vector3 ReadVector3(this BinaryReader reader) =>
        new Vector3()
        {
            X = reader.ReadSingle(),
            Y = reader.ReadSingle(),
            Z = reader.ReadSingle()
        };

    public static Vector4 ReadVector4(this BinaryReader reader) =>
        new Vector4()
        {
            X = reader.ReadSingle(),
            Y = reader.ReadSingle(),
            Z = reader.ReadSingle(),
            W = reader.ReadSingle()
        };

    public static void WriteVector3(this BinaryWriter writer, Vector3 input)
    {
        writer.Write(input.X);
        writer.Write(input.Y);
        writer.Write(input.Z);
    }

    public static void WriteVector4(this BinaryWriter writer, Vector4 input)
    {
        writer.Write(input.X);
        writer.Write(input.Y);
        writer.Write(input.Z);
        writer.Write(input.W);
    }

    public static byte[] ToArray(this BinaryWriter writer)
    {
        var stream = writer.BaseStream as MemoryStream;

        if (stream == null)
            throw new Exception("Stream is not a MemoryStream");

        return stream.ToArray();
    }
}
