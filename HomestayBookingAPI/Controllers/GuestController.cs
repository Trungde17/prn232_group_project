using AutoMapper;
using BusinessObjects.Homestays;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Repositories.HomeStayRepository;
using Repositories;          // nơi có ISupportRepository
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HomestayBookingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GuestController : ControllerBase
{
    private readonly IHomeStayRepository _homestayRepository;
    private readonly IMapper _mapper;
 
    public GuestController(
        IHomeStayRepository homestayRepository,
        IMapper mapper)
    {
        _homestayRepository = homestayRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// [2] Tìm kiếm homestay theo tên / địa chỉ / phường
    /// </summary>
     [HttpGet("homestays/search")]
    public async Task<IActionResult> SearchHomestays([FromQuery] HomestaySearchDTO search)
    {
            Expression<Func<Homestay, bool>> predicate = h =>
            (string.IsNullOrEmpty(search.Name) || EF.Functions.Like(h.Name.ToLower(), "%" + search.Name.ToLower() + "%")) &&
            (string.IsNullOrEmpty(search.Address) || EF.Functions.Like(h.StreetAddress.ToLower(), "%" + search.Address.ToLower() + "%")) &&
            (string.IsNullOrEmpty(search.Ward) || EF.Functions.Like(h.Ward.Name.ToLower(), "%" + search.Ward.ToLower() + "%")) &&
            (string.IsNullOrEmpty(search.District) || EF.Functions.Like(h.Ward.District.Name.ToLower(), "%" + search.District.ToLower() + "%"));

            // Lọc theo ngày nếu có CheckIn và CheckOut
            var homestays = await _homestayRepository.SearchWithInfoAsync(predicate, search.CheckIn, search.CheckOut);


            // Ánh xạ sang DTO
            var result = _mapper.Map<List<HomestayListDTO>>(homestays);
            return Ok(result);
     }






    /// <summary>
    /// [3] Xem chi tiết 1 homestay
    /// </summary>
    [HttpGet("homestays/{id}")]
    public async Task<IActionResult> GetHomestayDetail(int id)
    {
        var homestay = await _homestayRepository.GetDetailByIdAsync(id);
        if (homestay == null || !homestay.Status)
            return NotFound("Homestay not found");

        var result = _mapper.Map<HomestayDetailDTO>(homestay);
        return Ok(result);
    }

}




