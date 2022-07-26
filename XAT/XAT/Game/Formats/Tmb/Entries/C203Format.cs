using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C203Format : TmbEntry
{
    public const string MAGIC = "C203";
    public override string Magic => MAGIC;

    public override int Size => 0x2C;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public int Unk1 { get; set; } = 0;
    public int Unk2 { get; set; } = 0;
    public int Unk3 { get; set; } = 0;
    public int Unk4 { get; set; } = 0;
    public int Unk5 { get; set; } = 0;
    public int Unk6 { get; set; } = 0;
    public int Unk7 { get; set; } = 0;
    public int Unk8 { get; set; } = 0;

    public C203Format()
    {

    }

    public C203Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();
        Unk5 = context.Reader.ReadInt32();
        Unk6 = context.Reader.ReadInt32();
        Unk7 = context.Reader.ReadInt32();
        Unk8 = context.Reader.ReadInt32();
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
        context.Writer.Write(Unk7);
        context.Writer.Write(Unk8);
    }
}