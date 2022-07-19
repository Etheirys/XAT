using PropertyChanged;
using System.Text;

namespace XAT.Common.FFXIV.Files;

[AddINotifyPropertyChangedInterface]
public class Pap
{
    private const string PAP_MAGIC = "pap ";

    public int Skeleton { get; set; }
    public byte[] HavokData { get; set; }
    public byte[] TimelineData { get; set; }
    public List<PapAnimInfo> AnimInfos { get; private set; } = new();

    public Pap(BinaryReader reader)
    {
        // Magic
        string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (magic != PAP_MAGIC)
            throw new Exception("Invalid pap file - magic incorrect");

        // Version
        _ = reader.ReadInt32();

        // Num Anims
        int numAnims = reader.ReadInt16();

        // Skeleton
        this.Skeleton = reader.ReadInt32();

        // Offsets
        int infoOffset = reader.ReadInt32(); // Anim Info Offset
        int havokOffset = reader.ReadInt32(); // Havok Offset
        int timelineOffset = reader.ReadInt32(); // Timeline Offset

        // Anims
        reader.BaseStream.Seek(infoOffset, SeekOrigin.Begin);
        for (int i = 0; i < numAnims; i++)
        {
            PapAnimInfo animInfo = new PapAnimInfo(reader);
            this.AnimInfos.Add(animInfo);
        }

        // Havok Data
        reader.BaseStream.Seek(havokOffset, SeekOrigin.Begin);
        this.HavokData = reader.ReadBytes(timelineOffset - havokOffset);

        // Timeline Data
        reader.BaseStream.Seek(timelineOffset, SeekOrigin.Begin);
        this.TimelineData = reader.ReadBytes((int)reader.BaseStream.Length - timelineOffset);
    }

    public void Serialize(BinaryWriter writer)
    {
        int startPos = (int)writer.BaseStream.Position;

        // Magic
        writer.Write(Encoding.ASCII.GetBytes(PAP_MAGIC));

        // Version
        writer.Write(0x00020001);

        // Num Anims
        writer.Write((short)this.AnimInfos.Count);

        // Skeleton
        writer.Write(this.Skeleton);

        // Offsets
        int offsetsPosition = (int)writer.BaseStream.Position; // We will calculate offsets later
        writer.Write(0); // Anim Info Offset
        writer.Write(0); // Havok Offset
        writer.Write(0); // Timeline Offset

        // Anims
        int infoOffset = (int)writer.BaseStream.Position;
        foreach (var animInfo in this.AnimInfos)
        {
            animInfo.Serialize(writer);
        }

        // Havok Data
        int havokOffset = (int)writer.BaseStream.Position;
        writer.Write(this.HavokData);

        // Timeline Data
        int timelineOffset = (int)writer.BaseStream.Position;
        writer.Write(this.TimelineData);

        // Fix Offsets
        writer.BaseStream.Seek(offsetsPosition, SeekOrigin.Begin);
        writer.Write(infoOffset - startPos); // Anim Info Offset
        writer.Write(havokOffset - startPos); // Havok Offset
        writer.Write(timelineOffset - startPos); // Timeline Offset
    }

    public byte[] ToBytes()
    {
        using var stream = new MemoryStream();
        this.Serialize(new BinaryWriter(stream));
        return stream.ToArray();
    }

    public void ToFile(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Create);
        this.Serialize(new BinaryWriter(stream));
    }

    public static Pap FromFile(string filePath)
    {
        using var stream = File.Open(filePath, FileMode.Open);
        return new Pap(new BinaryReader(stream));
    }

    public static Pap FromBytes(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return new Pap(new BinaryReader(stream));
    }
}
