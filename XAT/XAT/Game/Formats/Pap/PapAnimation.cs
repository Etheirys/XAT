using PropertyChanged;
using XAT.Game.Formats.Tmb;

namespace XAT.Game.Formats.Pap;


[AddINotifyPropertyChangedInterface]
public record class PapAnimation(PapAnimDataFormat Data, TmbFormat Timeline);