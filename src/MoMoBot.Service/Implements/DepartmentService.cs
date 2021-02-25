using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class DepartmentService
        : IDepartmentService
    {
        private readonly MoMoDbContext _dbContext;
        public DepartmentService(MoMoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Department department)
        {
            _dbContext.Departments.Add(department);
        }

        public IQueryable<Department> Find(Expression<Func<Department, bool>> where)
        {
            return _dbContext.Departments.Where(where);
        }
        public IEnumerable<long> GetIntentPower(string intent)
        {
            string sql = "select \"DepartmentId\" FROM \"public\".\"QAPermissions\" " +
                "WHERE \"QAId\" IN (SELECT \"Id\" FROM \"public\".\"Answers\" where \"Intent\"=@intent) Group BY \"DepartmentId\"";
            //var connection = _dbContext.Database.GetDbConnection();
            //if (connection.State != System.Data.ConnectionState.Open)
            //{
            //    connection.Open();
            //}
            //var result = connection.Query<long>(sql, new { intent });
            var result = _dbContext.SqlQuery<long>(sql, new { intent });
            return result;
        }


        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="powers"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePowerAsync(string intent, long[] powers)
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            var tran = connection.BeginTransaction();
            try
            {
                string getKnowSql = "SELECT \"Id\" FROM \"public\".\"Answers\" WHERE \"Intent\" = @intent";
                List<Guid> intentOfKnows = connection.Query<Guid>(getKnowSql, new { intent })?.ToList(); //先拿到当前意图下所有的答案
                if (intentOfKnows?.Count > 0)
                {
                    var strBulidKnowSql = new StringBuilder();
                    for (var k = 0; k < intentOfKnows.Count; k++)
                    {
                        strBulidKnowSql.Append($"'{intentOfKnows[k]}',");
                    }
                    string strKnowSql = strBulidKnowSql.ToString().TrimEnd(',');
                    //先把该意图下所有答案的权限给删除
                    await connection.ExecuteAsync($"DELETE FROM \"public\".\"QAPermissions\" WHERE \"QAId\" IN ({strKnowSql});", transaction: tran);
                    //在修改知识的IsPublic属性
                    await connection.ExecuteAsync($"UPDATE \"public\".\"Answers\" SET \"IsPublic\"={powers.Length <= 0} WHERE \"Id\" IN ({strKnowSql});", transaction: tran);
                    if (powers.Length > 0)
                    {
                        //当权限数组不为0时，就执行添加语句
                        StringBuilder insertSql = new StringBuilder();
                        insertSql.Append("INSERT INTO \"public\".\"QAPermissions\"(\"DepartmentId\",\"QAId\") VALUES ");
                        for (var j = 0; j < intentOfKnows.Count(); j++)
                        {
                            for (var i = 0; i < powers.Length; i++)
                            {
                                insertSql.Append($"('{powers[i]}','{intentOfKnows[j]}'),");
                            }
                        }
                        string sql = insertSql.ToString().TrimEnd(','); //去除sql语句的最后一个,
                        await connection.ExecuteAsync(sql, transaction: tran);
                    }
                }
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                tran.Dispose();
            }
            return true;
        }


        public IEnumerable<long> GetModularPower(long id)
        {
            string sql = "SELECT \"DepartmentId\"  FROM \"public\".\"ModularPermissions\" WHERE \"ModularId\" = @modularid";
            //var connection = _dbContext.Database.GetDbConnection();
            //if (connection.State != System.Data.ConnectionState.Open)
            //{
            //    connection.Open();
            //}
            //var date = connection.Query<long>(sql, new { modularid = id });
            var data = _dbContext.SqlQuery<long>(sql, new { modularid = id });
            return data;
        }

        public async Task<bool> UpdateModularPowerAsync(long id, long[] powers)
        {
            var connection = _dbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            var tran = connection.BeginTransaction();
            try
            {
                //delete语句,先全部删除
                string deleteSql = "DELETE FROM \"public\".\"ModularPermissions\" WHERE \"ModularId\" = @modularid";
                await connection.ExecuteAsync(deleteSql, param: new { modularid = id }, transaction: tran);
                //在修改IsPublic属性
                string modifyIsPublicSql = $"UPDATE \"public\".\"Modulars\" SET \"IsPublic\" = {powers.Length <= 0} WHERE \"Id\" = {id} ";
                await connection.ExecuteAsync(modifyIsPublicSql, transaction: tran);
                if (powers.Length > 0)
                {
                    StringBuilder insertSql = new StringBuilder();
                    insertSql.Append("INSERT INTO \"public\".\"ModularPermissions\"(\"ModularId\",\"DepartmentId\") VALUES ");
                    foreach (var i in powers)
                    {
                        insertSql.Append($"({id},{i}),");
                    }
                    string sql = insertSql.ToString().TrimEnd(',');
                    await connection.ExecuteAsync(sql, transaction: tran);
                }
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                tran.Dispose();
            }
            return true;
        }


        public async Task<bool> SaveChangesAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public bool SaveChanges()
        {
            return _dbContext.SaveChanges() > 0;
        }

        public void Delete(Department depart)
        {
            _dbContext.Departments.Remove(depart);
        }

        public bool Existed(string name)
        {
            return _dbContext.Departments.Any(d => d.DepartName == name);
        }
    }
}
