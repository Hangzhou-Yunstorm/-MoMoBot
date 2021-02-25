using Microsoft.EntityFrameworkCore;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class UnknownService
        : IUnknownService
    {
        private readonly MoMoDbContext _context;
        public UnknownService(MoMoDbContext context)
        {
            _context = context;
        }

        public async Task<PagingQueryResult<Unknown>> GetAllUnknowns(int limit = 10, int offset = 0)
        {
            var total = await _context.Unknowns.CountAsync(u => !u.IsResolved);
            var data = await _context.Unknowns
                .Where(u => !u.IsResolved)
                .OrderByDescending(u => u.TimeOfOccurrence)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return new PagingQueryResult<Unknown> { Data = data, Total = total };
        }

        public async Task<int> GetUnknowCount()
        {
            return await Task.FromResult(_context.Unknowns.Where(u => !u.IsResolved).Count());
        }

        public async Task MarkResolved(Guid id, string intent = null)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteInTransactionAsync(async () =>
            {
                var unknow = _context.Unknowns.FirstOrDefault(u => u.Id == id);
                if (unknow != null)
                {
                    unknow.IsResolved = true;
                    _context.Entry(unknow).State = EntityState.Modified;

                    if (string.IsNullOrEmpty(intent) == false)
                    {
                        await _context.MetadataSet.AddAsync(new Metadata
                        {
                            Intent = intent,
                            Question = unknow.Content
                        });
                    }

                    await _context.SaveChangesAsync();
                }
            }, async () => await _context.Unknowns.AnyAsync(u => u.Id == id && u.IsResolved));
        }

        public async Task Record(Unknown issue)
        {
            await _context.Unknowns.AddAsync(issue);
            await _context.SaveChangesAsync();
        }
    }
}
