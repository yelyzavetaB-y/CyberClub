﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Domain.Interfaces
{
    public interface IBookingService
    {
        Task UpdateExpiredBookingsSeatsAsync();
    }
}
