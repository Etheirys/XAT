using PropertyChanged;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Controls;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Common;

public partial class Vector3Editor : UserControl, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public static readonly DependencyBinding<Vector3> ValueProperty = Binder.Register<Vector3, Vector3Editor>(nameof(Value), changed: OnValueChanged);

	public Vector3 Value
	{
		get => ValueProperty.Get(this);
		set => ValueProperty.Set(this, value);
	}

	public Vector3Editor()
	{
		InitializeComponent();

		this.ContentArea.DataContext = this;
	}

	[AlsoNotifyFor(nameof(Value))]
	[DependsOn(nameof(Value))]
	public float X
	{
		get
		{
			return this.Value.X;
		}

		set
		{
			this.Value = new Vector3(value, this.Y, this.Z);
		}
	}

	[AlsoNotifyFor(nameof(Value))]
	[DependsOn(nameof(Value))]
	public float Y
	{
		get
		{
			return this.Value.Y;
		}

		set
		{
			this.Value = new Vector3(this.X, value, this.Z);
		}
	}

	[AlsoNotifyFor(nameof(Value))]
	[DependsOn(nameof(Value))]
	public float Z
	{
		get
		{
			return this.Value.Z;
		}

		set
		{
			this.Value = new Vector3(this.X, this.Y, value);
		}
	}

    [SuppressPropertyChangedWarnings]	
	private static void OnValueChanged(Vector3Editor sender, Vector3 _, Vector3 value)
	{
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(X)));
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Y)));
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Z)));
	}
}