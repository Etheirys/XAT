using Microsoft.Win32;
using PropertyChanged;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Pap;
using XAT.Game.Formats.Sklb;
using XAT.UI.Utils;

namespace XAT.UI.Animation;

[AddINotifyPropertyChangedInterface]
public partial class AnimationTab : UserControl
{

    public PapFormat? Pap { get; set; }

    public PapAnimation? Animation { get; set; }

    public SklbFormat? Skeleton { get; set; }


    public AnimationTab()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    private async Task<bool> OnLoadPap(string filePath)
    {
        using (new ProgressWrapper())
        {
            Log.Information($"Attempting to import pap '{filePath}'...");

            try
            {
                this.Pap = PapFormat.FromFile(filePath);
            }
            catch (Exception e)
            {
                this.Pap = null;
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                return false;
            }

            DialogUtils.ShowSnackbar("Successfully loaded source pap.");
            Log.Information("Successfully loaded source pap.");
        }

        return true;
    }

    private async Task<bool> OnLoadSklb(string filePath)
    {
        using (new ProgressWrapper())
        {
            Log.Information($"Attempting to import sklb '{filePath}'...");

            try
            {
                this.Skeleton = SklbFormat.FromFile(filePath);
            }
            catch (Exception e)
            {
                this.Skeleton = null;
                Log.Error($"Error importing: {e}", e);
                await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                return false;
            }

            DialogUtils.ShowSnackbar("Successfully loaded source sklb.");
            Log.Information("Successfully loaded source sklb.");
        }

        return true;
    }

    public ICommand SavePap => new Command(async (_) =>
    {
        if (Pap == null)
            return;

        SaveFileDialog dialog = new();

        dialog.Filter = "PAP File|*.pap";

        if (dialog.ShowDialog() == true)
        {
            using (new ProgressWrapper())
            {
                string path = dialog.FileName;

                try
                {
                    this.Pap.ToFile(path);
                }
                catch (Exception e)
                {
                    Log.Error($"Error saving: {e}", e);
                    await DialogUtils.ShowErrorPopup($"Error saving: {e.Message}");
                    return;
                }

                DialogUtils.ShowSnackbar("Successfully saved pap.");
                Log.Information("Successfully saved pap.");
            }
        }
    });
}
