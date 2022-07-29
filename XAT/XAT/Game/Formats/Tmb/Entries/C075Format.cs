using PropertyChanged;
using System.Numerics;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C075Format : TmbEntry
{
    public const string MAGIC = "C075";
    public override string Magic => MAGIC;

    public override int Size => 0x40;
    public override int ExtraSize => 4 * (3 + 3 + 3 + 4);
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 10;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public Vector3 Scale { get; set; } = new(1);

    [UserType]
    public Vector3 Rotation { get; set; } = new(0);

    [UserType]
    public Vector3 Position { get; set; } = new(0);

    [UserType]
    public Vector4 RGBA { get; set; } = new(1);

    public C075Format()
    {

    }

    public C075Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Scale = context.ReadOffsetVector3();
        Rotation = context.ReadOffsetVector3();
        Position = context.ReadOffsetVector3();
        RGBA = context.ReadOffsetVector4();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteExtraVector3(Scale);
        context.WriteExtraVector3(Rotation);
        context.WriteExtraVector3(Position);
        context.WriteExtraVector4(RGBA);

    }
}