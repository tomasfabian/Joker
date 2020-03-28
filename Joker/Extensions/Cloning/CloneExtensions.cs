using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Joker.Extensions.Cloning
{
  public static class CloneExtensions
  {
    public static TModel CloneObjectSerializable<TModel>(this TModel obj)
      where TModel: class 
    {  
      var memoryStream = new MemoryStream();  
      var binaryFormatter = new BinaryFormatter();  

      binaryFormatter.Serialize(memoryStream, obj);  

      memoryStream.Position = 0;  

      object result = binaryFormatter.Deserialize(memoryStream);  

      memoryStream.Close();  

      return (TModel) result;  
    } 
  }
}