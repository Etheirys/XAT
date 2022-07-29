using PropertyChanged;
using System.IO;
using XAT.Core;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmTrUnknownData
{
    [UserType]
    public int Unk1 { get; set; }

    [UserType]
    public short Unk2 { get; set; }

    [UserType]
    public short Unk3 { get; set; }

    [UserType]
    public int Unk4 { get; set; }

    public TmTrUnknownData(BinaryReader reader)
    {
        Unk1 = reader.ReadInt32();
        Unk2 = reader.ReadInt16();
        Unk3 = reader.ReadInt16();
        Unk4 = reader.ReadInt32();
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Unk1);
        writer.Write(Unk2);
        writer.Write(Unk3);
        writer.Write(Unk4);
    }
}
