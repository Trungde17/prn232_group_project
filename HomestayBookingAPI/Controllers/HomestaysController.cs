
﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusinessObjects.Homestays;
using DTOs.HomestayDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

﻿using DTOs.HomestayDtos;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services.HomestayServices;


namespace HomestayBookingAPI.Controllers
{

    public class HomestaysController : ODataController
    {
        private readonly IHomestayService _homestayService;

        public HomestaysController(IHomestayService homestayService)
        {
            _homestayService = homestayService;
        }

        // GET: odata/Homestays
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var homestays = await _homestayService.GetAllHomestaysAsync();
            return Ok(homestays);
        }

        // GET: odata/Homestays(5)
        [EnableQuery]
        [HttpGet("({key})")]

        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var homestay = await _homestayService.GetHomestayByIdAsync(key);
            if (homestay == null)
                return NotFound();

            return Ok(homestay);
        }


       



        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableQuery]
        [HttpGet] // Absolute route
        public async Task<IActionResult> MyHomestays()
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userIdClaim = "59dcaa38-8f31-4bed-b2db-81d383b933cd";
            if (userIdClaim == null)
                return StatusCode(500, "Cannot retrieve user ID");

            //var homestays = await _homestayService.GetHomestayByUserIdAsync(userIdClaim.Value);
            var homestays = await _homestayService.GetHomestayByUserIdAsync(userIdClaim);
            return Ok(homestays);
        }

        // PUT: odata/Homestays(5)
        [HttpPut("({key})")]
        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] HomestayUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _homestayService.UpdateHomestayAsync(key, dto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }
    }
}
