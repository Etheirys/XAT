using MaterialDesignThemes.Wpf;
using System;
using System.Threading.Tasks;
using XAT.Views;

namespace XAT.Utils.Dialog;

public static class DialogUtils
{
    const string PopupIdentifier = "PopupOverlay";

    private static int progressDepth = 0;

    public static Snackbar? MainSnackbar;

    public static async Task ShowRaw(object input)
    {
        await DialogHost.Show(input, PopupIdentifier);
    }

    public static void CloseRaw()
    {
        DialogHost.Close(PopupIdentifier);
    }

    public static void ShowSnackbar(string message)
    {
        MainSnackbar?.MessageQueue?.Enqueue(message);
    }

    public static async Task<object?> ShowErrorPopup(string content)
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

    public static void ShowProgressPopup()
    {
        progressDepth = Math.Max(1, progressDepth + 1);
        UpdateProgressDisplay();     
    }

    public static void CloseProgressPopup()
    {
        progressDepth = Math.Min(0, progressDepth - 1);
        UpdateProgressDisplay();
    }

    private static void UpdateProgressDisplay(bool forceHide = false)
    {
        if(!forceHide && progressDepth > 0 && !DialogHost.IsDialogOpen(PopupIdentifier))
        {
            DialogHost.Show(new ProgressDialog(), PopupIdentifier);
        }
        
        if((progressDepth == 0 || forceHide) && DialogHost.IsDialogOpen(PopupIdentifier))
        {
            DialogHost.Close(PopupIdentifier);
        }
    }
}
