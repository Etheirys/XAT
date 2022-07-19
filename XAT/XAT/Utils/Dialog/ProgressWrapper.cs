using System;

namespace XAT.Utils.Dialog;

public class ProgressWrapper : IDisposable
{
    public ProgressWrapper()
    {
        DialogUtils.ShowProgressPopup();
    }

    public void Dispose()
    {
        DialogUtils.CloseProgressPopup();
    }
}
