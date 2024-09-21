namespace Seat_Reservation_System.Models
{
    public class Inquiry
    {
        public int InquiryId { get; set; }

        public int UserId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public bool AdminReplied { get; set; }

        public string? AdminReply { get; set; }
        public DateOnly InquiryDate { get; set; }

        public virtual User User { get; set; }
    }

    public class InquiryViewModel
    {
        public string Title { get; set; }
        public DateOnly InquiryDate { get; set; }
        public bool AdminReplied { get; set; }
    }

}
