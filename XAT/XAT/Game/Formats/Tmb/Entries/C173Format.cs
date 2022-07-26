using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C173Format : TmbEntry
{
    public const string MAGIC = "C173";
    public override string Magic => MAGIC;

    public override int Size => 0x44;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public int Unk1 { get; set; } = 30;
    public int Unk2 { get; set; } = 0;

    public string Path { get; set; } = string.Empty;

    public short Bindpoint1 { get; set; } = 1;
    public short Bindpoint2 { get; set; } = 0xFF;

    public int Unk3 { get; set; } = 0;
    public int Unk4 { get; set; } = 0;
    public int Unk5 { get; set; } = 0;
    public int Unk6 { get; set; } = 0;
    public int Unk7 { get; set; } = 0;
    public int Unk8 { get; set; } = 0;
    public int Unk9 { get; set; } = 0;
    public int Unk10 { get; set; } = 0;
    public int Unk11 { get; set; } = 0;
    public int Unk12 { get; set; } = 0;

    public C173Format()
    {

    }

    public C173Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Path = context.ReadOffsetString();

        Bindpoint1 = context.Reader.ReadInt16();
        Bindpoint2 = context.Reader.ReadInt16();

        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();
        Unk5 = context.Reader.ReadInt32();
        Unk6 = context.Reader.ReadInt32();
        Unk7 = context.Reader.ReadInt32();
        Unk8 = context.Reader.ReadInt32();
        Unk9 = context.Reader.ReadInt32();
        Unk10 = context.Reader.ReadInt32();
        Unk11 = context.Reader.ReadInt32();
        Unk12 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteOffsetString(Path);

        context.Writer.Write(Bindpoint1);
        context.Writer.Write(Bindpoint2);

        context.Writer.Write(Unk3);
        context.Writer.Write(Unk4);
        context.Writer.Write(Unk5);
        context.Writer.Write(Unk6);
        context.Writer.Write(Unk7);
        context.Writer.Write(Unk8);
        context.Writer.Write(Unk9);
        context.Writer.Write(Unk10);
        context.Writer.Write(Unk11);
        context.Writer.Write(Unk12);
    }
}