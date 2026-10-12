using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MyEntityMembershipService.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute == null ? value.ToString() : attribute.Name;
        }
    }

}
