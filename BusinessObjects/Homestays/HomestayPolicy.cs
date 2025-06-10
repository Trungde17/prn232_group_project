namespace BusinessObjects.Homestays
{
    public class HomestayPolicy
    {
        public int HomestayId { get; set; }
        public int PolicyId { get; set; }

        public bool IsAllowed { get; set; }  // TRUE = cho phép, FALSE = không cho phép

        public Homestay Homestay { get; set; }
        public Policy Policy { get; set; }
    }

}
