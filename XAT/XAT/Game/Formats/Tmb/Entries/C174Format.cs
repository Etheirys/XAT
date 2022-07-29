using PropertyChanged;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C174Format : TmbEntry
{
    public const string MAGIC = "C174";
    public override string Magic => MAGIC;

    public override int Size => 0x28;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public int Unk3 { get; set; } = 5;

    [UserType]
    public int Unk4 { get; set; } = 1;

    [UserType]
    public int Unk5 { get; set; } = 1;

    [UserType]
    public int Unk6 { get; set; } = 0;

    [UserType]
    public int Unk7 { get; set; } = 0;

    public C174Format()
    {

    }

    public C174Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();
        Unk5 = context.Reader.ReadInt32();
        Unk6 = context.Reader.ReadInt32();
        Unk7 = context.Reader.ReadInt32();
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
    }
}