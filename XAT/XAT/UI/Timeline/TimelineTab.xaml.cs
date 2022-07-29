using Microsoft.Win32;
using PropertyChanged;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Tmb;
using XAT.UI.Utils;

namespace XAT.UI.Timeline;

[AddINotifyPropertyChangedInterface]
public partial class TimelineTab : UserControl
{
    public TmbFormat? Tmb { get; set; }


    public TimelineTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    private async Task<bool> OnLoadTmb(string filePath)
    {
        using (new ProgressWrapper())
        {
            Log.Information($"Attempting to import tmb '{filePath}'...");

            try
            {
                this.Tmb = TmbFormat.FromFile(filePath);
            }
            catch (Exception e)
            {
                this.Tmb = null;
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                return false;
            }

            DialogUtils.ShowSnackbar("Successfully loaded source tmb.");
            Log.Information("Successfully loaded source tmb.");
        }

        return true;
    }

    public ICommand SaveTmb => new Command(async (_) =>
    {
        if (Tmb == null)
            return;

        using (new ProgressWrapper())
        {
            SaveFileDialog dialog = new();

            dialog.Filter = "TMB File|*.tmb";

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                try
                {
                    this.Tmb.ToFile(path);
                }
                catch (Exception e)
                {
                    Log.Error($"Error saving: {e}", e);
                    await DialogUtils.ShowErrorPopup($"Error saving: {e.Message}");
                    return;
                }

                DialogUtils.ShowSnackbar("Successfully saved tmb.");
                Log.Information("Successfully saved tmb.");
            }
        }
    });
}
