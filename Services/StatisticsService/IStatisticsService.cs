using DTOs.StatisticsDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StatisticsService
{
    public interface IStatisticsService
    {
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int year);
    }
}
