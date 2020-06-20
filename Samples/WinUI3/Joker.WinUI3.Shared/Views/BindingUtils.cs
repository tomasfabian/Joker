using System.Collections;
using System.Linq;

namespace Joker.WinUI3.Sample.Views
{
  public static class BindingUtils
  {
    public static IEnumerable Fix(IEnumerable x) => x.OfType<object>();
  }
}