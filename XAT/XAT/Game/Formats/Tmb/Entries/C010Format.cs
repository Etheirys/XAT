using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C010Format : TmbEntry
{
    public const string MAGIC = "C010";
    public override string Magic => MAGIC;

    public override int Size => 0x28;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public int Duration { get; set; } = 10;
    public int Unk1 { get; set; } = 0;
    public int Unk2 { get; set; } = 0;
    public int Unk3 { get; set; } = 0;
    public float Unk4 { get; set; } = 0;
    public string Path { get; set; } = string.Empty;
    public int Unk5 { get; set; } = 0;

    public C010Format()
    {

    }

    public C010Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadSingle();

        Path = context.ReadOffsetString();

        Unk5 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
        context.Writer.Write(Unk4);

        context.WriteOffsetString(Path);

        context.Writer.Write(Unk5);
    }
}