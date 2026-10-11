using Moq;
using VyshKat.Core.Assets;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;

namespace VyshKat.Tests.Core;

public class FleetAcceptanceServiceTests
{
    private readonly InMemoryFleetRepository _repository = new();
    private readonly Mock<IServiceCenter> _serviceCenter = new();

    private FleetAcceptanceService CreateService() => new(_repository, _serviceCenter.Object);

    [Fact]
    public void Accept_WhenInspectionPasses_AddsAssetToFleet()
    {
        _serviceCenter.Setup(c => c.Inspect(It.IsAny<Asset>())).Returns(InspectionResult.Pass("ок"));
        var scooter = TestAssets.ElectricScooter();

        var result = CreateService().Accept(scooter);

        Assert.True(result.Accepted);
        Assert.Same(scooter, result.Asset);
        Assert.Contains(scooter, _repository.GetAll());
        _serviceCenter.Verify(c => c.Inspect(scooter), Times.Once);
    }

    [Fact]
    public void Accept_WhenInspectionFails_DoesNotAddAndReturnsReason()
    {
        _serviceCenter.Setup(c => c.Inspect(It.IsAny<Asset>())).Returns(InspectionResult.Fail("тормоза"));

        var result = CreateService().Accept(TestAssets.ElectricScooter(condition: 100));

        Assert.False(result.Accepted);
        Assert.Contains("тормоза", result.Reason);
        Assert.Empty(_repository.GetAll());
    }

    [Fact]
    public void Accept_DuplicateInventoryNumber_RejectedWithoutSecondInspection()
    {
        _serviceCenter.Setup(c => c.Inspect(It.IsAny<Asset>())).Returns(InspectionResult.Pass());
        var service = CreateService();
        service.Accept(TestAssets.ElectricScooter(number: "VK-ESC-0001"));

        var duplicate = TestAssets.Bicycle(number: "vk-esc-0001");
        var result = service.Accept(duplicate);

        Assert.False(result.Accepted);
        Assert.Contains("уже на балансе", result.Reason);
        Assert.Single(_repository.GetAll());
        _serviceCenter.Verify(c => c.Inspect(duplicate), Times.Never);
    }

    [Fact]
    public void Accept_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateService().Accept(null!));
    }

    [Fact]
    public void Constructor_NullDependencies_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => new FleetAcceptanceService(null!, _serviceCenter.Object));
        Assert.Throws<ArgumentNullException>(() => new FleetAcceptanceService(_repository, null!));
    }
}
