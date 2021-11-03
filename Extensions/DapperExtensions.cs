using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace fxl.codes.kisekae.Extensions
{
    public static class DapperExtensions
    {
        public static async Task<int> InsertAsync<T>(this IDbConnection connection, T item)
        {
            var info = GetTableInfo(typeof(T));
            var statement = $"insert into {info.TableName} ({string.Join(",", info.Columns)}) values ({string.Join(",", info.Parameters)}) returning id";
            return await connection.QuerySingleAsync<int>(statement, item);
        }

        public static async Task<int> InsertAsync<T>(this IDbConnection connection, IEnumerable<T> items)
        {
            var info = GetTableInfo(typeof(T));
            var statement = $"insert into {info.TableName} ({string.Join(",", info.Columns)}) values ({string.Join(",", info.Parameters)}) returning id";

            var count = 0;
            foreach (var item in items)
            {
                await connection.ExecuteAsync(statement, item);
                count++;
            }

            return count;
        }

        public static async Task<int> UpdateAsync<T>(this IDbConnection connection, T item)
        {
            var list = new List<T> { item };
            return await UpdateAsync<T>(connection, list);
        }

        public static async Task<int> UpdateAsync<T>(this IDbConnection connection, IEnumerable<T> items)
        {
            var info = GetTableInfo(typeof(T));
            var statement = new StringBuilder().AppendLine($"update {info.TableName} set");

            var sets = new List<string>();
            for (var index = 0; index < info.Columns.Length; index++)
                if (!info.Columns[index].Contains("_id"))
                    sets.Add($"{info.Columns[index]} = {info.Parameters[index]}");

            statement.AppendLine(string.Join(",", sets));
            statement.AppendLine("where id = @Id");

            var count = 0;
            foreach (var item in items)
            {
                await connection.ExecuteAsync(statement.ToString(), item);
                count++;
            }

            return count;
        }

        private static TableInfo GetTableInfo(Type type)
        {
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttribute?.Name ?? type.Name.ToLowerInvariant();

            var columns = new List<string>();
            var parameters = new List<string>();
            foreach (var property in type.GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance))
            {
                if (string.Equals(property.Name, "id", StringComparison.InvariantCultureIgnoreCase) || property.PropertyType.IsGenericType) continue;

                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                columns.Add($"\"{columnAttribute?.Name ?? property.Name.ToLowerInvariant()}\"");
                parameters.Add($"@{property.Name}");
            }

            return new TableInfo
            {
                TableName = tableName,
                Columns = columns.ToArray(),
                Parameters = parameters.ToArray()
            };
        }

        private class TableInfo
        {
            public string TableName { get; set; }
            public string[] Columns { get; set; }
            public string[] Parameters { get; set; }
        }
    }
}