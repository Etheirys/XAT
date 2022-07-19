using System.Windows;

namespace XAT.Utils.WPF.DependencyProperties;

public class DependencyBinding<T>
{
	private DependencyProperty dp;

	public DependencyBinding(DependencyProperty dp)
	{
		this.dp = dp;
	}

	public T Get(DependencyObject control)
	{
		return (T)control.GetValue(this.dp);
	}

	public void Set(DependencyObject control, T value)
	{
		T old = this.Get(control);

		if (old != null && old.Equals(value))
			return;

		control.SetValue(this.dp, value);
	}
}