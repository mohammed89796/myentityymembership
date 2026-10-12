using System.Reflection;

namespace MyEntityMembershipService.Helper
{
    public static class SortingHelper
    {
        public static IEnumerable<TEntity> SortAscending<TEntity, TKey>(IEnumerable<TEntity> entities, string sortBy) where TEntity : class
        {
            var propertyInfo = typeof(TEntity).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Invalid sort property.");
            }
            return entities.OrderBy(x =>
            {
                var value = propertyInfo.GetValue(x);

                if (value is string strValue)
                {
                    return strValue.Replace(" ", "").ToLower();
                }
                return value;
            });
        }

        public static IEnumerable<TEntity> SortDescending<TEntity, TKey>(IEnumerable<TEntity> entities, string sortBy) where TEntity : class
        {
            var propertyInfo = typeof(TEntity).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException("Invalid sort property.");
            }
            return entities.OrderByDescending(x =>
            {
                var value = propertyInfo.GetValue(x);
                if (value is string strValue)
                {
                    return strValue.Replace(" ", "").ToLower();
                }
                return value;
            });



        }
    }
}
