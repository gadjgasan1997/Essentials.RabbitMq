using System.Reflection;

namespace Essentials.RabbitMq.Extensions;

internal static class PropertyInfoExtensions
{
    /// <summary>
    /// Возвращает названия списка свойств через разделитель
    /// </summary>
    /// <param name="properties">Свойства</param>
    /// <returns></returns>
    public static string GetNames(this IEnumerable<PropertyInfo> properties) =>
        string.Join("', '", properties.Select(info => info.Name));
}