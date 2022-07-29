using PropertyChanged;
using System.Numerics;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C012Format : TmbEntry
{
    public const string MAGIC = "C012";
    public override string Magic => MAGIC;

    public override int Size => 0x48;
    public override int ExtraSize => 4 * (3 + 3 + 3 + 4);
    public override int TimelineCount => 0;

    [UserType]
    public int Duration { get; set; } = 10;

    [UserType]
    public int Unk1 { get; set; } = 0;

    [UserType]
    public string Path { get; set; } = string.Empty;

    [UserType]
    public short BindPoint1 { get; set; } = 1;

    [UserType]
    public short BindPoint2 { get; set; } = 0xFF;

    [UserType]
    public short BindPoint3 { get; set; } = 2;

    [UserType]
    public short BindPoint4 { get; set; } = 0xFF;

    [UserType]
    public Vector3 Scale { get; set; } = new(1);

    [UserType]
    public Vector3 Rotation { get; set; } = new(0);

    [UserType]
    public Vector3 Position { get; set; } = new(0);

    [UserType]
    public Vector4 RGBA { get; set; } = new(1);

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public int Unk3 { get; set; } = 0;

    public C012Format()
    {
        
    }

    public C012Format(TmbReadContext context)
    {
        ReadHeader(context);

        Duration = context.Reader.ReadInt32();
        Unk1 = context.Reader.ReadInt32();

        Path = context.ReadOffsetString();

        BindPoint1 = context.Reader.ReadInt16();
        BindPoint2 = context.Reader.ReadInt16();
        BindPoint3 = context.Reader.ReadInt16();
        BindPoint4 = context.Reader.ReadInt16();

        Scale = context.ReadOffsetVector3();
        Rotation = context.ReadOffsetVector3();
        Position = context.ReadOffsetVector3();
        RGBA = context.ReadOffsetVector4();

        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Duration);
        context.Writer.Write(Unk1);

        context.WriteOffsetString(Path);   

        context.Writer.Write(BindPoint1);
        context.Writer.Write(BindPoint2);
        context.Writer.Write(BindPoint3);
        context.Writer.Write(BindPoint4);

        context.WriteExtraVector3(Scale);
        context.WriteExtraVector3(Rotation);
        context.WriteExtraVector3(Position);
        context.WriteExtraVector4(RGBA);

        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);

    }
}