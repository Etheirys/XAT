using System;

namespace XAT.UI.Utils;

public class ProgressWrapper : IDisposable
{
    public ProgressWrapper()
    {
        DialogUtils.ShowProgressPopup();
    }

    public void Dispose()
    {
        DialogUtils.Pop();
    }
}
