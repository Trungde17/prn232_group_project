namespace DTOs.HomestayDtos
{
    public class GetHomestayDetailDTO
    {
        public int HomestayId { get; set; }
        public string Name { get; set; }
        public string HomestayTypeName { get; set; }
        public string Description { get; set; }
        public string StreetAddress { get; set; }
        public string WardName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DistrictName { get; set; }
        public List<HomestayPolicyDTO> Policies { get; set; }
        public List<string> Amenities { get; set; }
        public List<string> Neighbourhoods { get; set; }
        public List<string> ImageUrls { get; set; }
    }
    public class HomestayPolicyDTO
    {
        public string PolicyName { get; set; }
        public bool IsAllowed { get; set; }
    }
}
