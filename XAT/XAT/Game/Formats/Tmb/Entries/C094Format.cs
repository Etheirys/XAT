using PropertyChanged;

namespace XAT.Game.Formats.Tmb.Entries;

[AddINotifyPropertyChangedInterface]
public class C094Format : TmbEntry
{
    public const string MAGIC = "C094";
    public override string Magic => MAGIC;

    public override int Size => 0x20;
    public override int ExtraSize => 0x14;
    public override int TimelineCount => 0;

    public int Unk1 { get; set; } = 0;
    public int Unk2 { get; set; } = 0;
    public int Unk3 { get; set; } = 0;
    public int Unk4 { get; set; } = 0;

    public int Unk5 { get; set; } = 0;
    public int Unk6 { get; set; } = 0;
    public int Unk7 { get; set; } = 0;
    public int Unk8 { get; set; } = 0;
    public int Unk9 { get; set; } = 0;

    public C094Format()
    {

    }

    public C094Format(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();
        Unk3 = context.Reader.ReadInt32();
        Unk4 = context.Reader.ReadInt32();

        context.ReadAtOffset((reader) =>
        {
            Unk5 = reader.ReadInt32();
            Unk6 = reader.ReadInt32();
            Unk7 = reader.ReadInt32();
            Unk8 = reader.ReadInt32();
            Unk9 = reader.ReadInt32();
        });
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);
        context.Writer.Write(Unk3);
        context.Writer.Write(Unk4);

        context.WriteExtra((writer) =>
        {
            writer.Write(Unk5);
            writer.Write(Unk6);
            writer.Write(Unk7);
            writer.Write(Unk8);
            writer.Write(Unk9);
        });
    }
}