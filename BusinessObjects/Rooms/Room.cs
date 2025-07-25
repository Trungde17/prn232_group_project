﻿using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects.Rooms
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int HomestayId { get; set; }
        public Homestay Homestay { get; set; }
        public string Description { get; set; }
        public string ImgUrl { get; set; }
        public int Capacity { get; set; }
        public double Size { get; set; }  // m²

        // Navigation
        public ICollection<RoomBed> RoomBeds { get; set; } = new List<RoomBed>();
        public ICollection<RoomPrice> RoomPrices { get; set; } = new List<RoomPrice>();
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
        public ICollection<RoomSchedule> RoomSchedules { get; set; } =  new List<RoomSchedule>();
        public ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
