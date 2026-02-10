using PropertyChanged;
using XAT.Core;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C216Format : TmbEntry
{
    public const string MAGIC = "C216";
    public override string Magic => MAGIC;

    public override int Size => 0x30;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    [UserType]
    public bool Enabled { get; set; } = false;

    [UserType]
    public int Unk2 { get; set; } = 0;

    [UserType]
    public int SubtitleType { get; set; } = 0;

    [UserType]
    public int TextId { get; set; } = 0;

    [UserType]
    public int SpeakerId { get; set; } = 0;

    [UserType]
    public float Duration { get; set; } = 0;

    [UserType]
    public int Unk7 { get; set; } = 0;

    [UserType]
    public int Unk8 { get; set; } = 0;

    [UserType]
    public int Unk9 { get; set; } = 0;

    public C216Format()
    {

    }

    public C216Format(TmbReadContext context)
    {
        ReadHeader(context);

        Enabled = context.Reader.ReadBoolean();
        Unk2 = context.Reader.ReadInt32();
        SubtitleType = context.Reader.ReadInt32();
        TextId = context.Reader.ReadInt32();
        SpeakerId = context.Reader.ReadInt32();
        Duration = context.Reader.ReadSingle();
        Unk7 = context.Reader.ReadInt32();
        Unk8 = context.Reader.ReadInt32();
        Unk9 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Enabled);
        context.Writer.Write(Unk2);
        context.Writer.Write(SubtitleType);
        context.Writer.Write(TextId);
        context.Writer.Write(SpeakerId);
        context.Writer.Write(Duration);
        context.Writer.Write(Unk7);
        context.Writer.Write(Unk8);
        context.Writer.Write(Unk9);
    }
}