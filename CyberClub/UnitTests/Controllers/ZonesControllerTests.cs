using Moq;
using Xunit;
using CyberClub.ViewModels;
using CyberClub.Domain.Services;
using CyberClub.Domain.Models;

namespace UnitTests.Controllers
{
    public class ZonesControllerTests
    {
        [Fact]
        public async Task ZoneSelection_ReturnsNonNullViewModel()
        {
            //var mockZoneService = new Mock<ZoneService>();
            //mockZoneService.Setup(service => service.GetAllZonesAsync())
            //               .ReturnsAsync(new List<Zone>() { new Zone { ZoneID = 1, Name = "Test Zone", Capacity = 100 } });

            //var controller = new ZonesController(mockZoneService.Object);

            //// Act
            //var result = await controller.ZoneSelection();

            //// Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsType<ZoneSelectionViewModel>(viewResult.Model);
            //Assert.NotNull(model);
            //Assert.Single(model.Zones);
            //Assert.False(model.NoZonesAvailable);
        }
    }
}
