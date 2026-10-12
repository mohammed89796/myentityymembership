namespace MyEntityMembershipService.Helper
{
    public static class ConvertHelper
    {
        public static object? ConvertToPropertyType(Type targetType , string responseValue)
        {
            try
            {
                #region Old

                //if (targetType == typeof(int?))
                //    return int.TryParse(responseValue, out var intValue) ? (int?)intValue : null;
                //else if (targetType == typeof(DateTime?))
                //    return DateTime.TryParse(responseValue, out var dateValue) ? (DateTime?)dateValue : null;
                //else if (targetType == typeof(bool?))
                //    return bool.TryParse(responseValue, out var boolValue) ? (bool?)boolValue : null;
                //else if (targetType.IsEnum)
                //    return Enum.TryParse(targetType, responseValue, out var enumValue) ? enumValue : null;
                //else if (Nullable.GetUnderlyingType(targetType)?.IsEnum == true) // For nullable enums
                //{
                //    var enumType = Nullable.GetUnderlyingType(targetType);
                //    return Enum.TryParse(enumType, responseValue, true, out var nullableEnumValue) ? nullableEnumValue : null;
                //}
                //return Convert.ChangeType(responseValue, targetType); 

                #endregion

                switch (targetType)
                {
                    case Type t when t == typeof(int?):
                        return int.TryParse(responseValue, out var intValue) ? (int?)intValue : null;

                    case Type t when t == typeof(DateTime?):
                        return DateTime.TryParse(responseValue, out var dateValue) ? (DateTime?)dateValue : null;

                    case Type t when t == typeof(DateOnly?):
                        return DateOnly.TryParse(responseValue, out var datOnlyValue) ? (DateOnly?)datOnlyValue : null;
                    case Type t when t == typeof(bool?):
                        return bool.TryParse(responseValue, out var boolValue) ? (bool?)boolValue : null;

                    case Type t when t.IsEnum:
                        return Enum.TryParse(targetType, responseValue, out var enumValue) ? enumValue : null;

                    case Type t when Nullable.GetUnderlyingType(t)?.IsEnum == true:
                        var enumType = Nullable.GetUnderlyingType(targetType);
                        return Enum.TryParse(enumType, responseValue, true, out var nullableEnumValue) ? nullableEnumValue : null;

                    default:
                        return Convert.ChangeType(responseValue, targetType);
                }

            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
