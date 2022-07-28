using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using XAT.Services;
using XAT.UI.Misc;

namespace XAT.UI.Utils;

public class DialogUtils
{
    const string PopupIdentifier = "PopupOverlay";

    public static Snackbar? MainSnackbar;

    private static Stack<(object display, TaskCompletionSource tcs)> displayStack = new();

    public static Task Show(object input)
    {
        if (DialogHost.IsDialogOpen(PopupIdentifier))
            DialogHost.Close(PopupIdentifier);

        DialogHost.Show(input, PopupIdentifier);

        TaskCompletionSource tcs = new();
        displayStack.Push((input, tcs));

        return tcs.Task;
    }

    public static void Pop()
    {
        if (displayStack.Count == 0)
            return;

        DialogHost.Close(PopupIdentifier);

        var completion = displayStack.Pop();
        completion.tcs.SetResult();

        (object display, TaskCompletionSource tcs) nextUp;
        if (displayStack.TryPeek(out nextUp))
        {
            DialogHost.Show(nextUp.display, PopupIdentifier);
        }
    }

    public static ICommand PopCommand => new Command((_) =>
    {
        Pop();
    });

    public static void ShowSnackbar(string message)
    {
        MainSnackbar?.MessageQueue?.Enqueue(message);
    }

    public static async Task ShowErrorPopup(string content)
    {
        ErrorDialog dialog = new()
        {
            Message = content
        };

        await Show(dialog);
    }

    public static void ShowProgressPopup()
    {
        Show(new ProgressDialog());
    }
}
