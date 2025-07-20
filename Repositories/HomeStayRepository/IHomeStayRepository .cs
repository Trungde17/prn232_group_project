using BusinessObjects.Homestays;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.HomeStayRepository
{
    public interface IHomeStayRepository : IGenericRepository<Homestay>
    {
        Task<Homestay> GetDetailByIdAsync(int id);
        Task<List<HomestayListDTO>> SearchWithInfoAsync(Expression<Func<Homestay, bool>> predicate);
    }
}
