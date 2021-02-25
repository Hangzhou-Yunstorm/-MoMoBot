using MoMoBot.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IDepartmentService
    {
        IQueryable<Department> Find(Expression<Func<Department, bool>> where);

        /// <summary>
        /// 获得意图所对应的部门权限
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        IEnumerable<long> GetIntentPower(string intent);

        /// <summary>
        /// 更新意图权限
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="powers"></param>
        /// <returns></returns>
        Task<bool> UpdatePowerAsync(string intent, long[] powers);
        void Add(Department department);

        bool SaveChanges();
        Task<bool> SaveChangesAsync();
        void Delete(Department depart);

        bool Existed(string name);

        /// <summary>
        /// 获得模板所对应的部门权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<long> GetModularPower(long id);

        /// <summary>
        /// 更新模板权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="powers"></param>
        /// <returns></returns>
        Task<bool> UpdateModularPowerAsync(long id,long[] powers);

    }
}
