using PropertyChanged;
using Serilog;
using System.Text;

namespace XAT.Common.FFXIV.Files;

[AddINotifyPropertyChangedInterface]
public class Sklb
{
    private const string SKLB_MAGIC = "blks";

    public byte[] PreHavokData { get; set; }
    public byte[] HavokData { get; set; }

    public Sklb(BinaryReader reader)
    {
        // Magic
        string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (magic != SKLB_MAGIC)
            throw new Exception("Invalid sklb file - magic incorrect");

        // Read header
        int header1 = reader.ReadInt16();
        int header2 = reader.ReadInt16();
        bool oldHeader;

        Log.Debug($"Sklb headers were: Part 1: 0x{header1.ToString("X")}, Part 2: 0x{header2.ToString("X")}");

        switch(header2)
        {
            case 0x3132:
                oldHeader = true;
                break;

            case 0x3133:
                oldHeader = false;
                break;

            default:
                Log.Warning($"Unknown sklb headers: Part 1: 0x{header1.ToString("X")}, Part 2: 0x{header2.ToString("X")} - Assume it's new for now");
                oldHeader = false;
                break;
        }

        // Havok offset
        int havokOffset;
        if (oldHeader)
        {
            reader.BaseStream.Seek(10, SeekOrigin.Begin);
            havokOffset = reader.ReadInt16();
        }
        else
        {
            reader.BaseStream.Seek(12, SeekOrigin.Begin);
            havokOffset = reader.ReadInt32();
        }

        // Havok data
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        this.PreHavokData = reader.ReadBytes(havokOffset);
        this.HavokData = reader.ReadBytes((int)reader.BaseStream.Length - havokOffset);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(this.PreHavokData);
        writer.Write(this.HavokData);
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

    public static Sklb FromFile(string filePath)
    {
        using var sklbStream = File.Open(filePath, FileMode.Open);
        return new Sklb(new BinaryReader(sklbStream));
    }

    public static Sklb FromBytes(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return new Sklb(new BinaryReader(stream));
    }
}
