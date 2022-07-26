using PropertyChanged;
using System.Collections.Generic;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmacFormat : TmbItemWithTimeFormat
{
    public const string MAGIC = "TMAC";
    public override string Magic => MAGIC;

    public override int Size => 0x1C;
    public override int ExtraSize => 0;
    public override int TimelineCount => Tracks.Count;

    public int Unk1 { get; set; }
    public int Unk2 { get; set; }

    public List<TmbPointer<TmtrFormat>> Tracks;


    public TmacFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Tracks = context.ReadOffsetTimeline<TmtrFormat>();
    }
    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteOffsetTimeline(Tracks);
    }
}