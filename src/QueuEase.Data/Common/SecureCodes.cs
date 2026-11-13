using System.Security.Cryptography;

namespace QueuEase.Data.Common;

// Random secrets for locations. RandomNumberGenerator is cryptographically secure,
// unlike System.Random, so the values cannot be guessed.
public static class SecureCodes
{
    private const string TokenAlphabet =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    private const string JoinCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string NewToken() =>
        RandomNumberGenerator.GetString(TokenAlphabet, DataConstants.TokenLength);

    public static string NewJoinCode() =>
        RandomNumberGenerator.GetString(JoinCodeAlphabet, DataConstants.JoinCodeLength);
}
