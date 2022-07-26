using PropertyChanged;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmdhFormat : TmbItemWithIdFormat
{
    public const string MAGIC = "TMDH";
    public override string Magic => MAGIC;

    public override int Size => 0x10;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public short Unk1 { get; set; }
    public short Unk2 { get; set; }
    public short Unk3 { get; set; }

    public TmdhFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt16();
        Unk2 = context.Reader.ReadInt16();
        Unk3 = context.Reader.ReadInt16();

    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
    }
}