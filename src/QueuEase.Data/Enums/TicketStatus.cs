namespace QueuEase.Data.Enums;

// Called and InService are used in a filtered database index; do not change their values.
public enum TicketStatus
{
    Waiting = 0,
    Called = 1,
    InService = 2,
    Completed = 3,
    NoShow = 4,
    Expired = 5
}
