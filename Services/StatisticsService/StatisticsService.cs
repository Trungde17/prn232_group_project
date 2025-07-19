using BusinessObjects.Enums;
using DataAccess;
using DTOs.StatisticsDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly HomestayDbContext _context;
        public StatisticsService(HomestayDbContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year)
        {
            var monthlyRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Completed && b.DateBooked.Year == year)
                .GroupBy(b => b.DateBooked.Month)
                .Select(g => new MonthlyRevenueDto
                {
                    Month = g.Key,
                    Revenue = g.Sum(b => b.TotalAmount)
                })
                .ToListAsync();

            // Ensure all months are present
            var result = Enumerable.Range(1, 12)
                .Select(m => new MonthlyRevenueDto
                {
                    Month = m,
                    Revenue = monthlyRevenue.FirstOrDefault(x => x.Month == m)?.Revenue ?? 0
                })
                .ToList();

            return result;
        }
    }
}
