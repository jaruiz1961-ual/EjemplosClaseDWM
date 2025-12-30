using BlazorSeguridad2026.Data.Modelo;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace BibliotecaContextosDb.Resources
{
    public static class DisplayNameExtensions
    {
        public static string GetDisplayName<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            if (expression.Body is not MemberExpression member)
                throw new ArgumentException("La expresión debe ser una propiedad", nameof(expression));

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException("La expresión debe ser una propiedad", nameof(expression));

            var displayAttr = propInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr is null)
                return propInfo.Name;

            if (displayAttr.ResourceType is not null && !string.IsNullOrEmpty(displayAttr.Name))
            {
                var rm = new ResourceManager(displayAttr.ResourceType);
                var value = rm.GetString(displayAttr.Name);
                return string.IsNullOrEmpty(value) ? displayAttr.Name : value;
            }

            return displayAttr.GetName() ?? propInfo.Name;
        }

        // Sobrecarga simplificada solo para UserModel
        public static string GetDisplayName<TValue>(Expression<Func<UserModel, TValue>> expression)
            => GetDisplayName<UserModel, TValue>(expression);
    }

}


