using PropertyChanged;
using System.Text;

namespace XAT.Common.FFXIV.Files;

[AddINotifyPropertyChangedInterface]
public class Tmb
{
    private const string TMB_MAGIC = "TMLB";

    public byte[] RawData { get; set; }

    public Tmb(BinaryReader reader)
    {
        var startPos = reader.BaseStream.Position;

        // Magic
        string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
        if (magic != TMB_MAGIC)
            throw new Exception("Invalid tmb file - magic incorrect");

        int size = reader.ReadInt32();

        RawData = reader.ReadBytes(size - 8);       
    }

    public void Serialize(BinaryWriter writer)
    {
        // Magic
        writer.Write(Encoding.ASCII.GetBytes(TMB_MAGIC));

        writer.Write(RawData.Length + 8);

        writer.Write(RawData);
    }
}
