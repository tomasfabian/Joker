namespace Joker.MVVM.ViewModels
{
  public class SelectionChangedEventArgs<TValue> : System.EventArgs
  {
    public TValue OldValue { get; }
    public TValue NewValue { get; }

    public SelectionChangedEventArgs(TValue oldValue, TValue newValue)
    {
      OldValue = oldValue;
      NewValue = newValue;
    }
  }
}