using PropertyChanged;
using Serilog;
using System;
using System.IO;
using System.Text;
using XAT.Game.Formats.Utils;

namespace XAT.Game.Formats.Sklb;

[AddINotifyPropertyChangedInterface]
public class SklbFormat
{
    private const string MAGIC = "blks";

    public short Header1 { get; set; }
    public short Header2 { get; set; }

    [DependsOn(nameof(Header1), nameof(Header2))]
    public bool OldHeader
    {
        get
        {
            switch (Header2)
            {
                case 0x3132:
                    return true;

                case 0x3133:
                    return false;

                default:
                    Log.Warning($"Unknown sklb headers: Part 1: 0x{Header1.ToString("X")}, Part 2: 0x{Header2.ToString("X")} - Assume it's new for now");
                    return false;
            }
        }
    }

    public int Unk1 { get; set; }

    public int Skeleton { get; set; }
    public int[] ParentSkeletons { get; set; }

    public byte[] Unk2 { get; set; }
    public byte[] Unk3 { get; set; }

    public byte[] HavokData { get; set; }

    public SklbFormat(BinaryReader reader)
    {
        // Magic
        string magic = reader.ReadEncodedString(4);
        if (magic != MAGIC)
            throw new Exception("Invalid sklb file - magic incorrect");

        // Read header
        Header1 = reader.ReadInt16();
        Header2 = reader.ReadInt16();
        Log.Debug($"Sklb headers were: Part 1: 0x{Header1.ToString("X")}, Part 2: 0x{Header2.ToString("X")}");

        // Offsets
        int havokOffset;
        int unknownOffset;
        if (OldHeader)
        {
            unknownOffset = reader.ReadInt16();
            havokOffset = reader.ReadInt16();
        }
        else
        {
            unknownOffset = reader.ReadInt32();
            havokOffset = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
        }

        Skeleton = reader.ReadInt32();

        ParentSkeletons = new int[4];
        for (int i = 0; i < ParentSkeletons.Length; ++i)
        {
            ParentSkeletons[i] = reader.ReadInt32();
        }

        this.Unk2 = reader.ReadBytes((int)(unknownOffset - reader.BaseStream.Position));

        this.Unk3 = reader.ReadBytes((int)(havokOffset - reader.BaseStream.Position));

        this.HavokData = reader.ReadBytes((int)reader.BaseStream.Length - havokOffset);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.WriteEncodedString(MAGIC);
        writer.Write(Header1);
        writer.Write(Header2);

        if(OldHeader)
        {
            writer.Write((short)0);
            writer.Write((short)0);
        }
        else
        {
            writer.Write((int)0);
            writer.Write((int)0);
            writer.Write(Unk1);
        }

        writer.Write(Skeleton);
        for (int i = 0; i < ParentSkeletons.Length; ++i)
        {
            writer.Write(ParentSkeletons[i]);
        }

        writer.Write(Unk2);
        int firstOffset = (int) writer.BaseStream.Position;
        writer.Write(Unk3);

        int havokOffset = (int)writer.BaseStream.Position;
        writer.Write(this.HavokData);

        writer.BaseStream.Seek(8, SeekOrigin.Begin);
        if (OldHeader)
        {
            writer.Write((short)firstOffset);
            writer.Write((short)havokOffset);
        }
        else
        {
            writer.Write((int)firstOffset);
            writer.Write((int)havokOffset);
        }
    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        this.Serialize(new BinaryWriter(stream));
        return stream.ToArray();
    }

    public void ToFile(string filePath)
    {
        using var sklbStream = File.Open(filePath, FileMode.Create);
        this.Serialize(new BinaryWriter(sklbStream));
    }

    public static SklbFormat FromFile(string filePath)
    {
        using var sklbStream = File.Open(filePath, FileMode.Open);
        return new SklbFormat(new BinaryReader(sklbStream));
    }

    public static SklbFormat FromBytes(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return new SklbFormat(new BinaryReader(stream));
    }
}
