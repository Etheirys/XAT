using PropertyChanged;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C215Format : TmbEntry
{
    public const string MAGIC = "C215";
    public override string Magic => MAGIC;

    public override int Size => 0x1C;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 0;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public int Unk3 { get; set; } = 0;

    [UserType]
    public int Unk4 { get; set; } = 0;

    public C215Format()
    {

    }

    public C215Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
        context.Writer.Write(Unk4);
    }
}