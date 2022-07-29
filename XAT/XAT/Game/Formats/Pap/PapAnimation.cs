using PropertyChanged;
using XAT.Game.Formats.Tmb;

namespace XAT.Game.Formats.Pap;


[AddINotifyPropertyChangedInterface]
public record class PapAnimation(PapFormat Container, int Index, PapAnimDataFormat Data, TmbFormat Timeline);