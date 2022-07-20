using Microsoft.Win32;
using PropertyChanged;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using XAT.Utils;
using XAT.Utils.Dialog;

namespace XAT.Views;

[AddINotifyPropertyChangedInterface]
public partial class BoneExclusionPopup : UserControl
{
    public ObservableCollection<string> ExcludedBones { get; set; } = new();

    public List<string> IncludedBones { get; set; } = new();

    public List<string> AllBones { get; set; } = new();

    public string SelectedExcludeBone { get; set; } = string.Empty;

    public string SelectedIncludeBone { get; set; } = string.Empty;

    public ICommand ExcludeBone => new Command((_) =>
    {
        var boneName = this.SelectedIncludeBone;

        if (string.IsNullOrEmpty(boneName))
            return;

        this.ExcludedBones.Add(boneName);

        this.UpdateBoneLists();
    });

    public ICommand IncludeBone => new Command((_) =>
    {
        var boneName = this.SelectedExcludeBone;


        if (string.IsNullOrEmpty(boneName))
            return;

        this.ExcludedBones.Remove(boneName);

        this.UpdateBoneLists();
    });

    public ICommand LoadExclusions => new Command(async (_) =>
    {
        OpenFileDialog fileDialog = new();
        fileDialog.Filter = "XAT Bone Exclusion File (*.xbe)|*.xbe";

        if(fileDialog.ShowDialog() == true)
        {
            var lines = await File.ReadAllLinesAsync(fileDialog.FileName);
            foreach(var line in lines)
            {
                var boneName = line.Trim();
                if(!string.IsNullOrEmpty(boneName))
                {
                    if (this.AllBones.Contains(boneName) && !this.ExcludedBones.Contains(boneName))
                    {
                        this.ExcludedBones.Add(boneName);
                    }
                }
            }

            this.UpdateBoneLists();
        }

    });

    public ICommand SaveExclusions => new Command(async (_) =>
    {
        SaveFileDialog fileDialog = new();
        fileDialog.Filter = "XAT Bone Exclusion File (*.xbe)|*.xbe";

        if (fileDialog.ShowDialog() == true)
        {
            StringBuilder sb = new();
            foreach (var boneName in ExcludedBones)
            {
                sb.AppendLine(boneName);
            }

            await File.WriteAllTextAsync(fileDialog.FileName, sb.ToString());

            this.UpdateBoneLists();
        }
    });

    public ICommand ClosePopup => new Command((_) =>
    {
        DialogUtils.CloseRaw();
    });

    public void OnExcludedBonesChanged()
    {
        this.UpdateBoneLists();
    }

    public BoneExclusionPopup()
    {
        InitializeComponent();
        this.ContentArea.DataContext = this;
    }

    public void UpdateBoneLists()
    {
        var tempList = new List<string>(this.AllBones);
        tempList.Remove("n_root");

        foreach (var excluded in this.ExcludedBones)
        {
            tempList.Remove(excluded);
        }

        this.IncludedBones = tempList;
    }
}
