using System.Linq.Expressions;
using System.Reflection;

namespace MyEntityMembershipService.Helper
{
    public static class SearchHelper
    {
        public static IQueryable<TEntity> Search<TEntity>(IQueryable<TEntity> source, string searchTerm) where TEntity : class
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return source;
            }

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var propertiesToSearch = new List<PropertyInfo>
            {
                typeof(TEntity).GetProperty("Name"),
                typeof(TEntity).GetProperty("Id"),
                typeof(TEntity).GetProperty("Email"),
                typeof(TEntity).GetProperty("CreationDate"),
                typeof(TEntity).GetProperty("Status")
            };

            Expression orExpression = null;
            var searchExpression = Expression.Constant(searchTerm.ToLower(), typeof(string));

            foreach (var property in propertiesToSearch)
            {
                if (property == null) continue;

                if (property.PropertyType == typeof(string))
                {
                    var propertyExpression = Expression.Property(parameter, property);
                    var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var lowerCaseProperty = Expression.Call(propertyExpression, toLowerMethod);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsExpression = Expression.Call(lowerCaseProperty, containsMethod, searchExpression);

                    var combinedExpression = Expression.AndAlso(nullCheck, containsExpression);
                    orExpression = orExpression == null ? (Expression)combinedExpression : Expression.OrElse(orExpression, combinedExpression);
                }
                else if (property.PropertyType == typeof(int) && int.TryParse(searchTerm, out int searchInt))
                {
                    var propertyExpression = Expression.Property(parameter, property);
                    var searchIntExpression = Expression.Constant(searchInt, typeof(int));
                    var equalsExpression = Expression.Equal(propertyExpression, searchIntExpression);

                    orExpression = orExpression == null ? (Expression)equalsExpression : Expression.OrElse(orExpression, equalsExpression);
                }
                else if (property.PropertyType.IsEnum)
                {
                    var propertyExpression = Expression.Property(parameter, property);
                    var toStringMethod = property.PropertyType.GetMethod("ToString", Type.EmptyTypes);
                    var enumToString = Expression.Call(propertyExpression, toStringMethod);
                    var lowerCaseEnum = Expression.Call(enumToString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    var containsEnumExpression = Expression.Call(lowerCaseEnum, typeof(string).GetMethod("Contains", new[] { typeof(string) }), searchExpression);

                    orExpression = orExpression == null ? (Expression)containsEnumExpression : Expression.OrElse(orExpression, containsEnumExpression);
                }
                else if (property.PropertyType == typeof(DateTime) && DateTime.TryParse(searchTerm, out DateTime searchDate))
                {
                    var propertyExpression = Expression.Property(parameter, property);
                    var searchDateExpression = Expression.Constant(searchDate, typeof(DateTime));
                    var equalsDateExpression = Expression.Equal(propertyExpression, searchDateExpression);

                    orExpression = orExpression == null ? (Expression)equalsDateExpression : Expression.OrElse(orExpression, equalsDateExpression);
                }
            }

            if (orExpression == null)
            {
                return source;
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(orExpression, parameter);
            return source.Where(lambda);
        }
    }
}
