using CyberClub.Domain.Models.Enum;
using CyberClub.Domain.Models;
using CyberClub.Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http.Extensions;

namespace UnitTests.Repositories
{
    public class BookingRepositoryTests
    {
        //[Fact]
        //public async Task AddBookingAsync_ReturnsTrue_WhenInsertedSuccessfully()
        //{
        //    var queryBuilder = new Mock<QueryBuilder>();
        //    var repo = new BookingRepository(queryBuilder.Object);

        //    var booking = new Booking
        //    {
        //        UserId = 1,
        //        SeatId = 2,
        //        StartTime = DateTime.Now,
        //        Duration = 60,
        //        Status = Status.Confirmed
        //    };

        //    queryBuilder.Setup(q => q.ExecuteScalarAsync<decimal>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
        //                .ReturnsAsync(101m);

        //    var result = await repo.AddBookingAsync(booking);

        //    Assert.True(result);
        //}

        //[Fact]
        //public async Task DeleteExpiredBookingsAsync_ReturnsNumberOfRowsDeleted()
        //{
        //    var queryBuilder = new Mock<QueryBuilder>();
        //    var repo = new BookingRepository(queryBuilder.Object);

        //    queryBuilder.Setup(q => q.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
        //                .ReturnsAsync(3);

        //    var deleted = await repo.DeleteExpiredBookingsAsync();

        //    Assert.Equal(3, deleted);
        //}

    }
}
