using Joker.Extensions;
using Joker.OData.Extensions.Expressions;

namespace Joker.OData.Providers.OData
{
  internal static class ODataCompoundKeyProvider
  {
    internal static void SetKeys(this object targetEntity, params object[] keys)
    {
      var targetEntityPropertyInfos = targetEntity.GetType().TryGetKeyProperties();

      targetEntityPropertyInfos
        .ForEach((propertyInfo, index) =>
        {
          var key = keys[index];
          propertyInfo.SetValue(targetEntity, key);
        });
    }
  }
}