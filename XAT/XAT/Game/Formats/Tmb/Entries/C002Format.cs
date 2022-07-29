using PropertyChanged;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C002Format : TmbEntry
{
    public const string MAGIC = "C002";
    public override string Magic => MAGIC;

    public override int Size => 0x1C;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 10;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public string Path { get; set; } = string.Empty;


    public C002Format()
    {

    }
    public C002Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Path = context.ReadOffsetString();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteOffsetString(Path);
    }
}