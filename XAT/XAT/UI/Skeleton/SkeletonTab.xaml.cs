using Microsoft.Win32;
using PropertyChanged;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Game.Formats.Sklb;
using XAT.UI.Utils;

namespace XAT.UI.Skeleton;

[AddINotifyPropertyChangedInterface]
public partial class SkeletonTab : UserControl
{
    public SklbFormat? Skeleton { get; set; }

    public SkeletonTab()
    {
        InitializeComponent();

        this.ContentArea.DataContext = this;
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

    public ICommand SaveSklb => new Command(async (_) =>
    {
        if (Skeleton == null)
            return;

        SaveFileDialog dialog = new();

        dialog.Filter = "SKLB File|*.sklb";

        if (dialog.ShowDialog() == true)
        {
            using (new ProgressWrapper())
            {
                string path = dialog.FileName;
                Log.Information($"Attempting to save sklb '{path}'...");

                try
                {
                    this.Skeleton.ToFile(path);
                }
                catch (Exception e)
                {
                    Log.Error($"Error saving: {e}", e);
                    await DialogUtils.ShowErrorPopup($"Error saving: {e.Message}");
                    return;
                }

                DialogUtils.ShowSnackbar("Successfully saved sklb.");
                Log.Information("Successfully saved sklb.");
            }
        }
    });
}
