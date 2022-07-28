using PropertyChanged;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C009Format : TmbEntry
{
    public const string MAGIC = "C009";
    public override string Magic => MAGIC;

    public override int Size => 0x18;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 10;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public string Path { get; set; } = string.Empty;


    public C009Format()
    {

    }
    public C009Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk1 = context.Reader.ReadInt32();

        Path = context.ReadOffsetString();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk1);

        context.WriteOffsetString(Path);
    }
}