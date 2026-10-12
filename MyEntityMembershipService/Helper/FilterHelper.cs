using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MyEntityMembershipService.Helper
{
    public static class FilterHelper
    {
        public static IEnumerable<TEntity> Filter<TEntity>(IEnumerable<TEntity> entities, Dictionary<string, string[]> filters)
        {
            if (filters == null || !filters.Any())
            {
                throw new ArgumentException("No filter criteria provided.", nameof(filters));
            }

            foreach (var filter in filters)
            {
                var propertyName = filter.Key;
                var filterValues = filter.Value;

                if (filterValues == null || filterValues.Length == 0)
                {
                    continue;
                }

                entities = entities.Where(c => filterValues.Contains(GetPropertyValue(c, propertyName)));
            }

            return entities;
        }
        private static string GetPropertyValue<TEntity>(TEntity entity, string propertyName)
        {
            var propertyInfo = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo?.GetValue(entity)?.ToString();
        }
    }
}