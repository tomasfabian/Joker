using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Joker.Collections
{
  /// <summary>
  /// SortedObservableCollection is a sorted ObservableCollection that does not allow entries with duplicate or null keys.
  /// </summary>
  public class SortedObservableCollection<TValue> : ObservableCollection<TValue>
    where TValue : class, INotifyPropertyChanged
  {
    private readonly IComparer<TValue> comparer;

    public SortedObservableCollection(IComparer<TValue> comparer)
    {
      this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

      keys = Array.Empty<TValue>();
    }

    public SortedObservableCollection(IComparer<TValue> comparer, IEnumerable<TValue> collection)
      : this(comparer)
    {
      foreach (var item in collection)
      {
        Add(item);
      }
    }

    internal bool SupportsRangeNotifications { get; set; } = false;

    private TValue[] keys;
    
    private void EnsureCapacity(int min)
    {
      int newCapacity = keys.Length == 0 ? DefaultCapacity : keys.Length * 2;

      if ((uint)newCapacity > MaxArrayLength) newCapacity = MaxArrayLength;
      if (newCapacity < min) newCapacity = min;

      Capacity = newCapacity;
    }
    
    private const int MaxArrayLength = 0X7FEFFFFF;
    private const int DefaultCapacity = 4;
    
    public int Capacity
    {
      get => keys.Length;
      set
      {
        if (value != keys.Length)
        {
          if (value < Count)
          {
            throw new ArgumentOutOfRangeException(nameof(value), value, "ArgumentOutOfRange small capacity");
          }

          if (value > 0)
          {
            TValue[] newKeys = new TValue[value];
            if (Count > 0)
            {
              Array.Copy(keys, 0, newKeys, 0, Count);
            }
            keys = newKeys;
          }
          else
          {
            keys = Array.Empty<TValue>();
          }
        }
      }
    }

    private int IndexOfKey(TValue key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));

      int index = Array.BinarySearch(keys, 0, Count, key, comparer);

      return index >= 0 ? index : -1;
    }

    private int IndexOfValue(TValue value)
    {
      return Array.IndexOf(Items.ToArray(), value, 0, Count);
    }

    protected override void ClearItems()
    {
      foreach (var item in Items)
      {
        item.PropertyChanged -= OnNotifyPropertyChanged;
      }

      Array.Clear(keys, 0, Count);

      if (SupportsRangeNotifications)
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Items.ToList()));

      base.ClearItems();
    }

    protected override void RemoveItem(int index)
    {
      var item = this[index];

      RemoveKeyAt(index);
      
      item.PropertyChanged -= OnNotifyPropertyChanged;  

      base.RemoveItem(index);
    }

    private void RemoveKeyAt(int index)
    {
      if (index < 0 || index >= Count)
        throw new ArgumentOutOfRangeException(nameof(index), index, "Argument out of range");

      int newSize = Count - 1;

      if (index < newSize)
        Array.Copy(keys, index + 1, keys, index, newSize - index);
     
      keys[newSize] = default(TValue);
    }

    protected override void InsertItem(int index, TValue item)
    {
      if(item == null)
        throw new ArgumentNullException();

      int i = Array.BinarySearch(keys, 0, Count, item, comparer);
      if (i >= 0)
        throw new ArgumentException("Adding duplicate");

      InsertKey(item, ~i);

      base.InsertItem(~i, item);

      item.PropertyChanged += OnNotifyPropertyChanged;    
    }

    private void OnNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      var item = sender as TValue;
      
      var index = IndexOfValue(item);

      int i = Array.BinarySearch(keys.Except(new [] {item}).ToArray(), 0, Count-1, item, comparer);
      
      if (index != ~i)
      {
        RemoveKeyAt(index);
        InsertKey(item, ~i);

        base.MoveItem(index, ~i);
      }
    }

    private void InsertKey(TValue key, int index)
    {
      if (Count == keys.Length) EnsureCapacity(Count + 1);
      if (index < Count)
      {
        Array.Copy(keys, index, keys, index + 1, Count - index);
      }
      keys[index] = key;
    }

    protected override void SetItem(int index, TValue item)
    {
      throw new NotSupportedException();
    }

    protected override void MoveItem(int oldIndex, int newIndex)
    {
      throw new NotSupportedException();
    }



    internal void AddRange(IEnumerable<TValue> items)
    {
      if (SupportsRangeNotifications)
      {
        var values = items as TValue[] ?? items.ToArray();
        
        foreach (var item in values)
        {
          Items.Add(item);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, values.ToList()));
      }
      else
      {
        foreach (var item in items)
        {
          Add(item);
        }
      }
    }

    internal void RemoveRange(IEnumerable<TValue> items)
    {
      if (SupportsRangeNotifications)
      {
        var values = items as TValue[] ?? items.ToArray();

        foreach (var item in values)
        {
          Items.Remove(item);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, values.ToList()));
      }
      else
      {
        foreach (var item in items)
        {
          Remove(item);
        }
      }
    }

    internal void Sort()
    {
      Array.Sort(keys, 0, keys.Length, comparer);

      for (int i = 0; i < Count; i++)
      {
        TValue item = Items[i];

        int newIndex = Array.BinarySearch(keys, 0, Count, item, comparer);

        if (i != newIndex)
          base.MoveItem(i, newIndex);
      }
    }
  }
}