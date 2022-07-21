using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using XAT.Services;
using XAT.UI.Views;

namespace XAT.UI.Services;

public class DialogService : ServiceBase<DialogService>
{
    const string PopupIdentifier = "PopupOverlay";

    private int progressDepth = 0;

    public Snackbar? MainSnackbar;

    public async Task ShowRaw(object input)
    {
        await DialogHost.Show(input, PopupIdentifier);
    }

    public void CloseRaw()
    {
        DialogHost.Close(PopupIdentifier);
    }

    public void ShowSnackbar(string message)
    {
        MainSnackbar?.MessageQueue?.Enqueue(message);
    }

    public async Task<object?> ShowErrorPopup(string content)
    {
        UpdateProgressDisplay(true);

        ErrorDialog dialog = new()
        {
            Message = content
        };

        var result = await DialogHost.Show(dialog, PopupIdentifier);

        UpdateProgressDisplay();

        return result;
    }

    public void ShowProgressPopup()
    {
        progressDepth = Math.Max(1, progressDepth + 1);
        UpdateProgressDisplay();
    }

    public void CloseProgressPopup()
    {
        progressDepth = Math.Min(0, progressDepth - 1);
        UpdateProgressDisplay();
    }

    private void UpdateProgressDisplay(bool forceHide = false)
    {
        if (!forceHide && progressDepth > 0 && !DialogHost.IsDialogOpen(PopupIdentifier))
        {
            DialogHost.Show(new ProgressDialog(), PopupIdentifier);
        }

        if ((progressDepth == 0 || forceHide) && DialogHost.IsDialogOpen(PopupIdentifier))
        {
            DialogHost.Close(PopupIdentifier);
        }
    }
}
