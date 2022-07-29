using PropertyChanged;
using System.Numerics;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C093Format : TmbEntry
{
    public const string MAGIC = "C093";
    public override string Magic => MAGIC;

    public override int Size => 0x28;
    public override int ExtraSize => 4 * (3 + 3);
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 10;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public Vector4 Unk2 { get; set; } = new(1);

    [UserType]
    public Vector4 Unk3 { get; set; } = new(1);

    public int Unk4 { get; set; } = 0;

    public C093Format()
    {

    }

    public C093Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.ReadOffsetVector4();
        Unk3 = context.ReadOffsetVector4();
        Unk4 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk1);
        context.WriteExtraVector4(Unk2);
        context.WriteExtraVector4(Unk3);
        context.Writer.Write(Unk4);
    }
}