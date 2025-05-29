using CyberClub.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Services
{
    public class BookingServiceApi : IBookingService
    {
        private readonly IBookingApiRepository _bookingApiRepository;

        public BookingServiceApi(IBookingApiRepository bookingApiRepository)
        {
            _bookingApiRepository = bookingApiRepository;
        }

        public async Task UpdateExpiredBookingsSeatsAsync()
        {
            await _bookingApiRepository.UpdateExpiredBookingsSeatsAsync();
        }
    }
}
