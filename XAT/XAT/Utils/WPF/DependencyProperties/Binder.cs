using System;
using System.Windows;
using System.Windows.Data;

namespace XAT.Utils.WPF.DependencyProperties;

public class Binder
{

	public static DependencyBinding<TValue> Register<TValue, TOwner>(string propertyName, TValue? defaultValue = default, BindMode mode = BindMode.TwoWay, Action<TOwner, TValue, TValue>? changed = null)
	{
		Action<DependencyObject, DependencyPropertyChangedEventArgs> callback = (d, e) =>
		{
			if (d is TOwner owner)
			{
				TValue oldValue = (TValue)e.OldValue;
				TValue newValue = (TValue)e.NewValue;
				changed?.Invoke(owner, oldValue, newValue);
			}
		};

		FrameworkPropertyMetadata meta = new FrameworkPropertyMetadata(defaultValue, new PropertyChangedCallback(callback));
		meta.BindsTwoWayByDefault = mode == BindMode.TwoWay;
		meta.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		meta.Inherits = true;
		DependencyProperty dp = DependencyProperty.Register(propertyName, typeof(TValue), typeof(TOwner), meta);
		DependencyBinding<TValue> dpv = new DependencyBinding<TValue>(dp);
		return dpv;
	}
	
}

public enum BindMode
{
	OneWay,
	TwoWay,
}
