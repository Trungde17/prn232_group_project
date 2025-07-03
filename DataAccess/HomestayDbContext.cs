using BusinessObjects;
using BusinessObjects.Bookings;
using BusinessObjects.Homestays;
using BusinessObjects.Rooms;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class HomestayDbContext : IdentityDbContext<ApplicationUser>
    {

        public HomestayDbContext(DbContextOptions<HomestayDbContext> options)
        : base(options)
        {
        }
        //public HomestayDbContext() { }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=(local);Database=BookingHomestayDb;Uid=sa; Pwd=123; TrustServerCertificate=True;");
        //    }
        //}
        // DbSet cho các bảng chính

        public DbSet<Homestay> Homestays { get; set; }

        public DbSet<HomestayType> HomestayTypes { get; set; }

        public DbSet<HomestayAmenity> HomestayAmenities { get; set; }

        public DbSet<HomestayImage> HomestayImages { get; set; }

        public DbSet<HomestayNeighbourhood> HomestayNeighbourhoods { get; set; }

        public DbSet<HomestayPolicy> HomestayPolicies { get; set; }

        public DbSet<Neighbourhood> Neighbourhoods { get; set; }

        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<RoomPrice> RoomPrices { get; set; }
        public DbSet<PriceType> PriceTypes { get; set; }
        public DbSet<RoomBed> RoomBeds { get; set; }
        public DbSet<BedType> BedTypes { get; set; }
        public DbSet<RoomSchedule> RoomSchedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Policy> Policys { get; set; }
        public DbSet<Ward> Wards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // HomestayNeighbourhood: composite key
            modelBuilder.Entity<HomestayNeighbourhood>()
                .HasKey(hn => new { hn.NeighbourhoodId, hn.HomestayId });
            //HomestayPolicy : composite key
            modelBuilder.Entity<HomestayPolicy>()
                .HasKey(hp => new { hp.PolicyId, hp.HomestayId });
            // RoomBed: composite key
            modelBuilder.Entity<RoomBed>()
                .HasKey(rb => new { rb.RoomId, rb.BedTypeId });

            // HomestayAmenity: composite key
            modelBuilder.Entity<HomestayAmenity>()
                .HasKey(ha => new { ha.HomestayId, ha.AmenityId });

            // RoomPrice: composite key
            modelBuilder.Entity<RoomPrice>()
                .HasKey(rp => new { rp.RoomId, rp.PriceTypeId });
            //
            modelBuilder.Entity<RoomAmenity>()
                .HasKey(ra => new { ra.RoomId, ra.AmenityId });

            //Homestay vs Ward
            modelBuilder.Entity<Homestay>()
                .HasOne(h => h.Ward)
                .WithMany(w => w.Homestays)
                .HasForeignKey(h => h.WardId);

            // homestay vs homestay type
            modelBuilder.Entity<Homestay>()
                .HasOne(h => h.HomestayType)
                .WithMany(t => t.Homestays)
                .HasForeignKey(h => h.HomestayTypeId);
            // Homestay vs Owner
            modelBuilder.Entity<Homestay>()
                .HasOne(h => h.Owner)
                .WithMany(u => u.Homestays)
                .HasForeignKey(h => h.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);
            // Feedback vs homestay
            modelBuilder.Entity<Feedback>()
                .HasOne(fb => fb.Homestay)
                .WithMany(ht => ht.Feedbacks)
                .HasForeignKey(fb => fb.HomestayId)
                .OnDelete(DeleteBehavior.NoAction);
            // Feedback vs customer
            modelBuilder.Entity<Feedback>()
                .HasOne(fb => fb.Customer)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(fb => fb.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
            // homestay Amenities vs homestay
            modelBuilder.Entity<HomestayAmenity>()
                .HasOne(ha => ha.Homestay)
                .WithMany(ht => ht.HomestayAmenities)
                .HasForeignKey(ha => ha.HomestayId);
            // homestay Amenities vs amenity
            modelBuilder.Entity<HomestayAmenity>()
                .HasOne(ha => ha.Amenity)
                .WithMany(a => a.HomestayAmenities)
                .HasForeignKey(ha => ha.AmenityId);
            // homestay neighbourhood vs neighbourhood
            modelBuilder.Entity<HomestayNeighbourhood>()
                .HasOne(hn => hn.Neighbourhood)
                .WithMany(n => n.HomestayNeighbourhoods)
                .HasForeignKey(hn => hn.NeighbourhoodId);
            // homestay neighbourhood vs homestay
            modelBuilder.Entity<HomestayNeighbourhood>()
                .HasOne(hn => hn.Homestay)
                .WithMany(h => h.HomestayNeighbourhoods)
                .HasForeignKey(hn => hn.HomestayId);
            //homestay Policies vs policy
            modelBuilder.Entity<HomestayPolicy>()
                .HasOne(hp => hp.Policy)
                .WithMany(p => p.HomestayPolicies)
                .HasForeignKey(hp => hp.PolicyId);
            //homestay Policies vs Homestay
            modelBuilder.Entity<HomestayPolicy>()
                .HasOne(hp => hp.Homestay)
                .WithMany(h => h.HomestayPolicies)
                .HasForeignKey(hp => hp.HomestayId);
            // homestayimg vs homestay
            modelBuilder.Entity<HomestayImage>()
                .HasOne(hi => hi.Homestay)
                .WithMany(h => h.HomestayImages)
                .HasForeignKey(hi => hi.HomestayId);
            // room vs room schedule
            modelBuilder.Entity<RoomSchedule>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomSchedules)
                .HasForeignKey(rs => rs.RoomId);
            //Room bed vs bed type
            modelBuilder.Entity<RoomBed>()
                .HasOne(rb => rb.BedType)
                .WithMany(bt => bt.RoomBeds)
                .HasForeignKey(rb => rb.BedTypeId);
            //Room bed vs Room
            modelBuilder.Entity<RoomBed>()
                .HasOne(rb => rb.Room)
                .WithMany(r => r.RoomBeds)
                .HasForeignKey(rb => rb.RoomId);
            // roomAmenity vs room
            modelBuilder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Room)
                .WithMany(r => r.RoomAmenities)
                .HasForeignKey(ra => ra.RoomId);
            // roomAmenity vs amenity
            modelBuilder.Entity<RoomAmenity>()
                .HasOne(ra => ra.Amenity)
                .WithMany(a => a.RoomAmenities)
                .HasForeignKey(ra => ra.AmenityId);
            // Room Price vs priceType
            modelBuilder.Entity<RoomPrice>()
                .HasOne(rp => rp.PriceType)
                .WithMany(pt => pt.RoomPrices)
                .HasForeignKey(rp => rp.PriceTypeId);
            // Room Price vs room
            modelBuilder.Entity<RoomPrice>()
                .HasOne(rp => rp.Room)
                .WithMany(r => r.RoomPrices)
                .HasForeignKey(rp => rp.RoomId);
            // Booking vs homestay
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Homestay)
                .WithMany(h => h.Bookings)
                .HasForeignKey(b => b.HomestayId)
                .OnDelete(DeleteBehavior.NoAction);
            // Booking vs customer
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
            //Booking detail vs Booking
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId);
            //Booking detail vs Room
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Room)
                .WithMany(r => r.BookingDetails)
                .HasForeignKey(bd => bd.RoomId);
        }
    }

}
