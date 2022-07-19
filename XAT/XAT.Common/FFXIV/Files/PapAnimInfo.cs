using PropertyChanged;
using System.Text;

namespace XAT.Common.FFXIV.Files;

[AddINotifyPropertyChangedInterface]
public class PapAnimInfo
{
    public string Name { get; set; }
    public short HavokIndex { get; set; }
    public short Unk1 { get; private set; }
    public int Unk2 { get; private set; }


    public PapAnimInfo(BinaryReader reader)
    {
        // Name
        Name = Encoding.ASCII.GetString(reader.ReadBytes(32));

        // Unknown 1
        Unk1 = reader.ReadInt16();

        // Havok Index
        HavokIndex = reader.ReadInt16();

        // Unknown 2
        Unk2 = reader.ReadInt32();
    }

    public void Serialize(BinaryWriter writer)
    {
        // Name
        writer.Write(Encoding.ASCII.GetBytes(this.Name.PadRight(32, '\0')));

        // Unknown 1
        writer.Write(this.Unk1);

        // Havok Index
        writer.Write(this.HavokIndex);

        // Unknown 2
        writer.Write(this.Unk2);
    }

    public override string ToString()
    {
        return this.Name;
    }
}
