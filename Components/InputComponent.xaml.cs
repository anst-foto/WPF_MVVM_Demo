using System.Windows;
using System.Windows.Controls;

namespace WPF_MVVM_Demo.Components;

public partial class InputComponent : UserControl
{
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(object), typeof(InputComponent));

    public static readonly DependencyProperty InputValueProperty =
        DependencyProperty.Register(nameof(InputValue), typeof(string), typeof(InputComponent));

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(InputComponent));

    public object Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? InputValue
    {
        get => GetValue(InputValueProperty).ToString();
        set => SetValue(InputValueProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public InputComponent()
    {
        InitializeComponent();
    }
}