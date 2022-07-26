using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C063Format : TmbEntry
{
    public const string MAGIC = "C063";
    public override string Magic => MAGIC;

    public override int Size => 0x20;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public int Unk1 { get; set; } = 1;
    public int Unk2 { get; set; } = 0;
    public string Path { get; set; } = string.Empty;
    public int SoundIndex { get; set; } = 1;
    public int Unk3 { get; set; } = 0;

    public C063Format()
    {

    }

    public C063Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Path = context.ReadOffsetString();
        SoundIndex = context.Reader.ReadInt32();

        Unk3 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteOffsetString(Path);
        context.Writer.Write(SoundIndex);

        context.Writer.Write(Unk3);
    }
}