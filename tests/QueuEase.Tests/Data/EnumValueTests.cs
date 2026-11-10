using QueuEase.Data.Enums;

namespace QueuEase.Tests.Data;

// The numeric values are stored in the database and used in index filters,
// so changing them would silently break existing data.
public class EnumValueTests
{
    [Theory]
    [InlineData(OrganizationStatus.Pending, 0)]
    [InlineData(OrganizationStatus.Active, 1)]
    [InlineData(OrganizationStatus.Suspended, 2)]
    public void OrganizationStatus_HasFixedValues(OrganizationStatus status, int expected)
    {
        Assert.Equal(expected, (int)status);
    }

    [Theory]
    [InlineData(TicketStatus.Waiting, 0)]
    [InlineData(TicketStatus.Called, 1)]
    [InlineData(TicketStatus.InService, 2)]
    [InlineData(TicketStatus.Completed, 3)]
    [InlineData(TicketStatus.NoShow, 4)]
    [InlineData(TicketStatus.Expired, 5)]
    public void TicketStatus_HasFixedValues(TicketStatus status, int expected)
    {
        Assert.Equal(expected, (int)status);
    }

    [Theory]
    [InlineData(TicketSource.Kiosk, 0)]
    [InlineData(TicketSource.Staff, 1)]
    public void TicketSource_HasFixedValues(TicketSource source, int expected)
    {
        Assert.Equal(expected, (int)source);
    }

    [Theory]
    [InlineData(CallingMode.ArrivalOrder, 0)]
    [InlineData(CallingMode.ServicePriority, 1)]
    public void CallingMode_HasFixedValues(CallingMode mode, int expected)
    {
        Assert.Equal(expected, (int)mode);
    }

    [Theory]
    [InlineData(TicketEventType.Issued, 0)]
    [InlineData(TicketEventType.Called, 1)]
    [InlineData(TicketEventType.ServiceStarted, 2)]
    [InlineData(TicketEventType.Completed, 3)]
    [InlineData(TicketEventType.NoShow, 4)]
    [InlineData(TicketEventType.Redirected, 5)]
    [InlineData(TicketEventType.ReturnedToQueue, 6)]
    [InlineData(TicketEventType.Expired, 7)]
    public void TicketEventType_HasFixedValues(TicketEventType type, int expected)
    {
        Assert.Equal(expected, (int)type);
    }
}
