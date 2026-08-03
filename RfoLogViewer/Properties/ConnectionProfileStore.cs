using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RfoLogViewer.Properties
{
    [Serializable]
    public sealed class ConnectionProfile
    {
        public string DataSource { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public long ContextId { get; set; }

        public bool SavePassword { get; set; }
    }

    internal static class ConnectionProfileStore
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(List<ConnectionProfile>));

        public static List<ConnectionProfile> Load(string serializedProfiles)
        {
            if (string.IsNullOrWhiteSpace(serializedProfiles))
            {
                return new List<ConnectionProfile>();
            }

            try
            {
                using (var reader = new StringReader(serializedProfiles))
                {
                    var profiles = Serializer.Deserialize(reader) as List<ConnectionProfile>;
                    return Normalize(profiles);
                }
            }
            catch
            {
                return new List<ConnectionProfile>();
            }
        }

        public static string Save(IEnumerable<ConnectionProfile> profiles)
        {
            var normalized = Normalize(profiles);
            if (normalized.Count == 0)
            {
                return string.Empty;
            }

            using (var writer = new StringWriter())
            {
                Serializer.Serialize(writer, normalized);
                return writer.ToString();
            }
        }

        public static List<ConnectionProfile> Upsert(IEnumerable<ConnectionProfile> profiles, ConnectionProfile currentProfile)
        {
            var result = new List<ConnectionProfile>();
            var normalizedCurrent = NormalizeProfile(currentProfile);
            if (normalizedCurrent != null)
            {
                result.Add(normalizedCurrent);
            }

            if (profiles != null)
            {
                foreach (var profile in profiles)
                {
                    var normalized = NormalizeProfile(profile);
                    if (normalized == null)
                    {
                        continue;
                    }

                    if ((normalizedCurrent != null) && string.Equals(normalized.DataSource, normalizedCurrent.DataSource, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (!ContainsDataSource(result, normalized.DataSource))
                    {
                        result.Add(normalized);
                    }
                }
            }

            return result;
        }

        public static ConnectionProfile CreateProfile(string dataSource, string login, string password, long contextId, bool savePassword)
        {
            return NormalizeProfile(new ConnectionProfile
            {
                DataSource = dataSource,
                Login = login,
                Password = savePassword ? password : string.Empty,
                ContextId = contextId,
                SavePassword = savePassword
            });
        }

        private static List<ConnectionProfile> Normalize(IEnumerable<ConnectionProfile> profiles)
        {
            var result = new List<ConnectionProfile>();
            if (profiles == null)
            {
                return result;
            }

            foreach (var profile in profiles)
            {
                var normalized = NormalizeProfile(profile);
                if (normalized == null)
                {
                    continue;
                }

                if (!ContainsDataSource(result, normalized.DataSource))
                {
                    result.Add(normalized);
                }
            }

            return result;
        }

        private static ConnectionProfile NormalizeProfile(ConnectionProfile profile)
        {
            if (profile == null)
            {
                return null;
            }

            var dataSource = NormalizeText(profile.DataSource);
            if (string.IsNullOrWhiteSpace(dataSource))
            {
                return null;
            }

            return new ConnectionProfile
            {
                DataSource = dataSource,
                Login = NormalizeText(profile.Login),
                Password = profile.SavePassword ? (profile.Password ?? string.Empty) : string.Empty,
                ContextId = profile.ContextId,
                SavePassword = profile.SavePassword
            };
        }

        private static bool ContainsDataSource(IEnumerable<ConnectionProfile> profiles, string dataSource)
        {
            if ((profiles == null) || string.IsNullOrWhiteSpace(dataSource))
            {
                return false;
            }

            foreach (var profile in profiles)
            {
                if ((profile != null) && string.Equals(profile.DataSource, dataSource, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
