using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using XAT.Core;
using XAT.UI.Utils.DependencyProperties;

namespace XAT.UI.Common.Inspector;

[AddINotifyPropertyChangedInterface]
public partial class InspectorView : UserControl
{
	public static readonly DependencyBinding<object?> TargetProperty = Utils.DependencyProperties.Binder.Register<object?, InspectorView>(nameof(Target), null, BindMode.OneWay, (t, _, n) => OnTargetChanged(t, n));

	public object? Target
	{
		get => TargetProperty.Get(this);
		set => TargetProperty.Set(this, value);
	}

	public ObservableCollection<Entry> Entries { get; set; } = new();


	public InspectorView()
	{
		this.InitializeComponent();
		this.ContentArea.DataContext = this;

		OnTargetChanged(this, this.Target);
	}


	[SuppressPropertyChangedWarnings]
	private static void OnTargetChanged(InspectorView sender, object? newValue)
	{
		sender.Entries.Clear();

		if (newValue == null)
			return;

		Stack<PropertyInfo> stack = new();

		Type? targetType = newValue.GetType();

		if (targetType == null)
			return;

		do
        {
			PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => x.GetCustomAttribute<UserTypeAttribute>() != null).OrderBy(x => -x.GetCustomAttribute<UserTypeAttribute>()!.Order).ToArray();

			foreach(var property in properties)
            {
				stack.Push(property);
            }

			targetType = targetType?.BaseType;
		}while(targetType != null);

		foreach (PropertyInfo property in stack)
		{
			sender.Entries.Add(new(newValue, property));
		}
	}

	[AddINotifyPropertyChangedInterface]
	public class Entry : INotifyPropertyChanged
	{
		public Entry(object target, PropertyInfo property)
		{
			this.Property = property;
			this.Target = target;

			if (this.Target is INotifyPropertyChanged changed)
			{
				changed.PropertyChanged += this.OnTargetPropertyChanged;
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public object Target { get; private set; }
		public PropertyInfo Property { get; private set; }

		public string Name => this.Property.Name;
		public Type Type => this.Property.PropertyType;

		public bool CanWrite => this.Property.CanWrite;

		public object? Value
		{
			get => this.Property.GetValue(this.Target);
			set
			{
				if (!this.CanWrite)
					return;

				this.Property.SetValue(this.Target, value);
			}
		}

        [SuppressPropertyChangedWarnings]
		private void OnTargetPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != this.Property.Name)
				return;

			this.PropertyChanged?.Invoke(this, new(nameof(this.Value)));
		}
	}
}

public class TemplateSelector : DataTemplateSelector
{
	public override DataTemplate SelectTemplate(object? item, DependencyObject container)
	{
		if (item is not InspectorView.Entry entry)
			return base.SelectTemplate(item, container);

		DataTemplate? drawer = Drawers.GetDrawer(entry.Type);

		if (drawer != null)
			return drawer;

		return base.SelectTemplate(item, container);
	}
}