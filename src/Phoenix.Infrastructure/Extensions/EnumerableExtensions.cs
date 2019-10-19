﻿namespace Phoenix.Infrastructure.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    public static class EnumerableExtensions {
        public static DataTable ToDataTable<T>(this IEnumerable<T> source) {
            var dataTable = new DataTable(typeof (T).Name);

            var properties = typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
                dataTable.Columns.Add(property.Name, BaseType(property.PropertyType));

            foreach (var item in source) {
                var values = new object[properties.Length];

                for (var i = 0; i < properties.Length; i++)
                    values[i] = properties[i].GetValue(item, null);

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        private static Type BaseType(this Type type) {
            if (type != null && type.IsValueType && type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof (Nullable<>))
                return Nullable.GetUnderlyingType(type);
            return type;
        }
    }
}