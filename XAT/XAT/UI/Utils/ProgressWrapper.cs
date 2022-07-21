using System;
using XAT.UI.Services;

namespace XAT.UI.Utils;

public class ProgressWrapper : IDisposable
{
    public ProgressWrapper()
    {
        DialogService.Instance.ShowProgressPopup();
    }

    public void Dispose()
    {
        DialogService.Instance.CloseProgressPopup();
    }
}
