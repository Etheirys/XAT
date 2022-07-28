using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XAT.Core;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmacFormat : TmbItemWithTimeFormat
{
    public const string MAGIC = "TMAC";
    public override string Magic => MAGIC;

    public override int Size => 0x1C;
    public override int ExtraSize => 0;
    public override int TimelineCount => Tracks.Count;

    [UserType]
    public int Unk1 { get; set; }

    [UserType]
    public int Unk2 { get; set; }

    public ObservableCollection<TmbPointer<TmtrFormat>> Tracks { get; set; }


    public TmacFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Unk1 = context.Reader.ReadInt32();
        Unk2 = context.Reader.ReadInt32();

        Tracks = new(context.ReadOffsetTimeline<TmtrFormat>());
    }

    public TmacFormat()
    {
        Tracks = new();
    }

    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.Writer.Write(Unk1);
        context.Writer.Write(Unk2);

        context.WriteOffsetTimeline(Tracks);
    }
}