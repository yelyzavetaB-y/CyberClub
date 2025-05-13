namespace CyberClub.Api.Interfaces
{
    public interface IBookingService
    {
        Task UpdateExpiredBookingsSeatsAsync();
    }
}
