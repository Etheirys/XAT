using PropertyChanged;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Controls;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Common;

public partial class Vector4Editor : UserControl, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public static readonly DependencyBinding<Vector4> ValueProperty = Binder.Register<Vector4, Vector4Editor>(nameof(Value), changed: OnValueChanged);

	public Vector4 Value
	{
		get => ValueProperty.Get(this);
		set => ValueProperty.Set(this, value);
	}

	public Vector4Editor()
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
			this.Value = new Vector4(value, this.Y, this.Z, this.W);
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
			this.Value = new Vector4(this.X, value, this.Z, this.W);
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
			this.Value = new Vector4(this.X, this.Y, value, this.W);
		}
	}

	[AlsoNotifyFor(nameof(Value))]
	[DependsOn(nameof(Value))]
	public float W
	{
		get
		{
			return this.Value.W;
		}

		set
		{
			this.Value = new Vector4(this.X, this.Y, this.Z, value);
		}
	}

	[SuppressPropertyChangedWarnings]
	private static void OnValueChanged(Vector4Editor sender, Vector4 _, Vector4 value)
	{
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(X)));
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Y)));
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(Z)));
		sender.PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(nameof(W)));
	}
}