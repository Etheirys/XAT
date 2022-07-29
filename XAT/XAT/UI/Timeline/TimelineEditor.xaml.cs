using PropertyChanged;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using XAT.Game.Formats.Tmb;
using XAT.UI.Utils;
using XAT.UI.Utils.DependencyProperties;
using XAT.Game.Formats.Tmb.Entries;
using System.Collections.ObjectModel;
using System;
using Serilog;
using Microsoft.Win32;

namespace XAT.UI.Timeline;

[AddINotifyPropertyChangedInterface]
public partial class TimelineEditor : UserControl
{
    public static readonly DependencyBinding<TmbFormat?> TimelineProperty = Binder.Register<TmbFormat?, TimelineEditor>(nameof(Timeline));

    public TmbFormat? Timeline
    {
        get => TimelineProperty.Get(this);
        set => TimelineProperty.Set(this, value);
    }

    public ReadOnlyDictionary<string, Type> EntryTypes => TmbUtils.EntryTypes;

    public TmacFormat? SelectedActor { get; set; }
    public TmtrFormat? SelectedTrack { get; set; }
    public Type? SelectedNewEntryType { get; set; }

    public TimelineEditor()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    public ICommand AddActor => new Command((_) =>
    {
        if (Timeline == null)
            return;

        var newActor = new TmbPointer<TmacFormat>(new TmacFormat());
        this.Timeline.ActorList.Actors.Add(newActor);

        this.SelectedActor = newActor.Item;
    });

    public ICommand DeleteActor => new Command((_) =>
    {
        if (this.Timeline == null || this.SelectedActor == null)
            return;

        var toRemove = this.Timeline.ActorList.Actors.Where(x => x.Item == this.SelectedActor).First();

        if (toRemove != null)
            this.Timeline.ActorList.Actors.Remove(toRemove);
    });

    public ICommand AddTrack => new Command((_) =>
    {
        if (this.SelectedActor == null)
            return;

        var newTrack = new TmbPointer<TmtrFormat>(new TmtrFormat());
        this.SelectedActor.Tracks.Add(newTrack);

        this.SelectedTrack = newTrack.Item;
    });

    public ICommand DeleteTrack => new Command((_) =>
    {
        if (this.SelectedActor == null || this.SelectedTrack == null)
            return;

        var toRemove = this.SelectedActor.Tracks.Where(x => x.Item == this.SelectedTrack).First();

        if (toRemove != null)
            this.SelectedActor.Tracks.Remove(toRemove);
    });

    public ICommand DeleteEntry => new Command((item) =>
    {
        if (SelectedTrack == null || item is not TmbEntry)
            return;

        var entry = (TmbEntry)item;

        var toRemove = this.SelectedTrack.Entries.Where(x => x.Item == entry).First();

        if (toRemove != null)
            this.SelectedTrack.Entries.Remove(toRemove);
    });

    public ICommand DeleteUnknownExtraEntry => new Command((item) =>
    {
        if (SelectedTrack == null || item is not TmTrUnknownData)
            return;

        var entry = (TmTrUnknownData)item;
        this.SelectedTrack.UnknownExtraEntries.Remove(entry);
    });

    private void AddEntry(object sender, MouseButtonEventArgs e)
    {
        if (this.SelectedTrack == null)
            return;

        ListBox? lb = sender as ListBox;

        if (lb == null)
            return;

        Type? type = lb.SelectedValue as Type;

        if (type != null && type.IsAssignableTo(typeof(TmbItemWithTimeFormat)))
        {
            TmbItemWithTimeFormat? entry = null;

            var newCtr = type.GetConstructor(new Type[] { });
            if (newCtr != null)
            {
                entry = newCtr.Invoke(new object[] { }) as TmbItemWithTimeFormat;
            }

            if (entry == null)
                throw new Exception("Could not construct");

            this.SelectedTrack.Entries.Add(new TmbPointer<TmbItemWithTimeFormat>(entry));

            MaterialDesignThemes.Wpf.DrawerHost.CloseDrawerCommand.Execute(this, this.DrawerHost);
        }
    }

    public ICommand ImportTimeline => new Command(async (item) =>
    {

        OpenFileDialog dialog = new();
        dialog.Filter = "TMB File|*.tmb";

        if (dialog.ShowDialog() == true)
        {
            using (new ProgressWrapper())
            {
                string filePath = dialog.FileName;

                Log.Information($"Attempting to import tmb '{filePath}'...");

                try
                {
                    this.Timeline = TmbFormat.FromFile(filePath);
                }
                catch (Exception e)
                {
                    this.Timeline = null;
                    Log.Error($"Error importing: {e}", e);
                    await DialogUtils.ShowErrorPopup($"Error importing: {e.Message}");
                }

                DialogUtils.ShowSnackbar("Successfully imported tmb.");
                Log.Information("Successfully imported tmb.");
            }
        }
    });

    public ICommand ExportTimeline => new Command(async (item) =>
    {
        if (Timeline == null)
            return;

        SaveFileDialog dialog = new();
        dialog.Filter = "TMB File|*.tmb";

        if (dialog.ShowDialog() == true)
        {
            using (new ProgressWrapper())
            {
                string filePath = dialog.FileName;

                Log.Information($"Attempting to export tmb '{filePath}'...");

                try
                {
                    this.Timeline.ToFile(filePath);
                }
                catch (Exception e)
                {
                    this.Timeline = null;
                    Log.Error($"Error exporting: {e}", e);
                    await DialogUtils.ShowErrorPopup($"Error exporting: {e.Message}");
                }

                DialogUtils.ShowSnackbar("Successfully exported tmb.");
                Log.Information("Successfully exported tmb.");
            }
        }
    });
}