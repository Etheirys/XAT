using PropertyChanged;
using XAT.Core;
using XAT.Game.Formats.Tmb.Entries;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmfcFormat : TmbItemFormat
{
    public const string MAGIC = "TMFC";
    public override string Magic => MAGIC;

    public override int Size => 0x20;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public int Unk3 { get; set; } = 0;

    [UserType]
    public int Unk4 { get; set; } = 0;
    [UserType]
    public int Unk5 { get; set; } = 0;
    [UserType]
    public int Unk6 { get; set; } = 0;

    public TmfcFormat()
    {

    }

    public TmfcFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();
        Unk5 = context.Reader.ReadInt32();
        Unk6 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
        context.Writer.Write(Unk4);
        context.Writer.Write(Unk5);
        context.Writer.Write(Unk6);
    }
}