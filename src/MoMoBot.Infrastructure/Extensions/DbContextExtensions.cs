using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.Extensions
{
    public static class DbContextExtensions
    {
        public static IEnumerable<T> SqlQuery<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.Query<T>(sql, param, transaction);
        }

        public static async Task<IEnumerable<T>> SqlQueryAsync<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return await connection.QueryAsync<T>(sql, param, transaction);
        }

        public static T SqlQueryFirst<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.QueryFirst<T>(sql, param, transaction);
        }

        public static async Task<T> SqlQueryFirstAsync<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return await connection.QueryFirstAsync<T>(sql, param, transaction);
        }

        public static T SqlQueryFirstOrDefault<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.QueryFirstOrDefault<T>(sql, param, transaction);
        }

        public static async Task<T> SqlQueryFirstOrDefaultAsync<T>(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public static int ExecuteSql(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.Execute(sql, param, transaction);
        }

        public static async Task<int> ExecuteSqlAsync(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return await connection.ExecuteAsync(sql, param, transaction);
        }

        public static IDataReader ExecuteReader(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection.ExecuteReader(sql, param, transaction);
        }

        public static async Task<IDataReader> ExecuteReaderAsync(this DbContext dbContext, string sql, object param = null, IDbTransaction transaction = null)
        {
            var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return await connection.ExecuteReaderAsync(sql, param, transaction);
        }
    }
}
