// Estevão Santos Ribeiro

using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services
{
    public static class Normalizer
    {
        public static string NormalizeString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = Regex.Replace(value.Trim(), @"\s+", " ");
            return normalized;
        }

        public static string NormalizeDigitsOnly(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return Regex.Replace(value, @"\D", "");
        }

        public static string NormalizeUpperCase(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Trim().ToUpper();
        }

        public static string NormalizeCep(string? value)
        {
            return NormalizeDigitsOnly(value);
        }

        public static string NormalizeTelefone(string? value)
        {
            return NormalizeDigitsOnly(value);
        }

        public static string NormalizeCpf(string? value)
        {
            return NormalizeDigitsOnly(value);
        }

        public static string NormalizeEmail(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Trim().ToLower();
        }
    }
}
