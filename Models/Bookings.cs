namespace Seat_Reservation_System.Models
{
    public class Bookings
    {
        public int BookId { get; set; }
        public string? TraineeID { get; set; }
        public string? TraineeName { get; set; }

        public DateTime? BookDateTime { get; set;}
        public string? TraineeNIC {  get; set; }
        public string? SeatNumber { get; set; }
        public int isExpire { get; set; }   

    }
}
