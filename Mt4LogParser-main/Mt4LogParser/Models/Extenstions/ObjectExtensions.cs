using System.Reflection;

namespace Mt4LogParser.Models.Extenstions;

public static class ObjectExtensions
{
    public static T CreateEx<T>(this object source) where T : new()
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        T destination = new T();
        PropertyInfo[] sourceProperties = source.GetType().GetProperties();
        
        foreach (PropertyInfo sourceProp in sourceProperties)
        {
            PropertyInfo? destProp = typeof(T).GetProperty(sourceProp.Name);
            
            if (destProp != null && destProp.CanWrite)
            {
                object? valueToSet = sourceProp.GetValue(source, null);
                destProp.SetValue(destination, valueToSet, null);
            }
        }
        
        return destination;
    }
}