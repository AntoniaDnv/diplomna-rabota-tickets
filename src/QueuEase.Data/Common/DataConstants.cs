namespace QueuEase.Data.Common;

public static class DataConstants
{
    public const int NameMaxLength = 100;
    public const int CounterNameMaxLength = 50;
    public const int AddressMaxLength = 200;

    public const int PrefixMaxLength = 3;
    public const string PrefixPattern = "^[A-ZА-Я]{1,3}$";
    public const int DisplayNumberMaxLength = 10;

    public const int TokenMaxLength = 64;
    public const int JoinCodeLength = 8;

    public const int TicketExpiryHoursMin = 1;
    public const int TicketExpiryHoursMax = 48;
    public const int TicketExpiryHoursDefault = 12;
}
