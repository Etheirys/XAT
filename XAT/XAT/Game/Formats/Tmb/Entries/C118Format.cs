using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C118Format : TmbEntry
{
    public const string MAGIC = "C118";
    public override string Magic => MAGIC;

    public override int Size => 0x18;
    public override int ExtraSize => 0;
    public override int TimelineCount => 0;

    public int Unk1 { get; set; } = 0;
    public int Unk2 { get; set; } = 0;
    public int Unk3 { get; set; } = 100;


    public C118Format()
    {

    }
    public C118Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
    }
}