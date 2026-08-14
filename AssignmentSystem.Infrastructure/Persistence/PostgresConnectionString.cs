using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace AssignmentSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Resolves the PostgreSQL connection string from configuration and normalizes it.
    /// Managed hosts (Render, Heroku, Railway, Fly) hand out a URL such as
    /// postgres://user:pass@host:5432/db, which Npgsql cannot parse — this converts it
    /// into the keyword format Npgsql expects. Values already in keyword format are
    /// returned unchanged.
    /// </summary>
    public static class PostgresConnectionString
    {
        public static string Resolve(IConfiguration configuration)
        {
            var raw = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (string.IsNullOrWhiteSpace(raw))
                raw = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException(
                    "No PostgreSQL connection string found. Set the DATABASE_URL environment variable " +
                    "or the ConnectionStrings:DefaultConnection configuration value.");

            return Normalize(raw);
        }

        public static string Normalize(string connectionString)
        {
            if (!LooksLikeUrl(connectionString))
                return connectionString;

            Uri uri;
            try
            {
                uri = new Uri(connectionString);
            }
            catch (UriFormatException ex)
            {
                throw new InvalidOperationException(
                    "The PostgreSQL connection string looks like a URL but could not be parsed.", ex);
            }

            var userInfo = uri.UserInfo.Split(':', 2);

            var b = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
                Username = Uri.UnescapeDataString(userInfo[0]),
                Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
                // Managed Postgres requires TLS but presents a certificate signed by an
                // internal CA. SslMode.Require encrypts without validating the chain,
                // which is what these hosts expect (VerifyCA/VerifyFull would fail).
                SslMode = SslMode.Require
            };

            // Honor an explicit ?sslmode= in the URL (e.g. sslmode=disable for a local
            // Postgres reached over a URL rather than a keyword string).
            var sslMode = ReadQueryValue(uri.Query, "sslmode");
            if (sslMode is not null && Enum.TryParse<SslMode>(sslMode, ignoreCase: true, out var parsed))
            {
                b.SslMode = parsed;
            }

            return b.ConnectionString;
        }

        private static bool LooksLikeUrl(string value) =>
            value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase);

        private static string? ReadQueryValue(string query, string key)
        {
            foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split('=', 2);
                if (parts.Length == 2 && parts[0].Equals(key, StringComparison.OrdinalIgnoreCase))
                    return Uri.UnescapeDataString(parts[1]);
            }

            return null;
        }
    }
}