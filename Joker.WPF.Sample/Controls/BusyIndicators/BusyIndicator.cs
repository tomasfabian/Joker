using System.Windows;
using System.Windows.Controls;

namespace Joker.WPF.Sample.Controls.BusyIndicators
{
  public class BusyIndicator : ContentControl
  {
    #region IsBusy

    public bool IsBusy
    {
      get => (bool) GetValue(IsBusyProperty);
      set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty IsBusyProperty =
      DependencyProperty.Register(nameof(IsBusy), typeof(bool),
        typeof(BusyIndicator),
        new PropertyMetadata(null));

    #endregion

    #region IconFontSize

    public double IconFontSize
    {
      get => (double) GetValue(IconFontSizeProperty);
      set => SetValue(IconFontSizeProperty, value);
    }

    public static readonly DependencyProperty IconFontSizeProperty =
      DependencyProperty.Register(nameof(IconFontSize), typeof(double),
        typeof(BusyIndicator),
        new PropertyMetadata(null));

    #endregion
  }
}