using PropertyChanged;
using System.Collections.Generic;

namespace XAT.Game.Formats.Tmb;

[AddINotifyPropertyChangedInterface]
public class TmalFormat : TmbItemFormat
{
    public const string MAGIC = "TMAL";
    public override string Magic => MAGIC;

    public override int Size => 0x10;
    public override int ExtraSize => 0;
    public override int TimelineCount => Actors.Count;

    public List<TmbPointer<TmacFormat>> Actors { get; init; }

    public TmalFormat(TmbReadContext context)
    {
        ReadHeader(context);

        Actors = context.ReadOffsetTimeline<TmacFormat>();

    }
    public override void Serialize(TmbWriteContext context)
    {
        WriteHeader(context);

        context.WriteOffsetTimeline(Actors);

    }
}