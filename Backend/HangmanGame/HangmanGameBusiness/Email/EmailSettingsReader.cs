using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HangmanGameBusiness.Email
{
    public static class EmailSettingsReader
    {
        private const string SettingsFileName = "EmailSettings.config";

        public static EmailSettings Read()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string settingsPath = Path.Combine(baseDirectory, SettingsFileName);

            if (!File.Exists(settingsPath))
            {
                throw new FileNotFoundException(
                    "Email settings file was not found.",
                    settingsPath
                );
            }

            XDocument document = XDocument.Load(settingsPath);
            XElement appSettingsSection = document.Root;

            if (appSettingsSection == null || appSettingsSection.Name != "appSettings")
            {
                throw new ConfigurationErrorsException(
                    "EmailSettings.config must contain an appSettings section."
                );
            }

            return new EmailSettings
            {
                SenderAddress = GetRequiredValue(appSettingsSection, "EmailSenderAddress"),
                SenderPassword = GetRequiredValue(appSettingsSection, "EmailSenderPassword"),
                SmtpHost = GetRequiredValue(appSettingsSection, "SmtpHost"),
                SmtpPort = int.Parse(GetRequiredValue(appSettingsSection, "SmtpPort")),
                SenderDisplayName = GetRequiredValue(appSettingsSection, "EmailSenderDisplayName")
            };
        }

        private static string GetRequiredValue(
            XElement appSettingsSection,
            string key)
        {
            XElement element = appSettingsSection
                .Elements("add")
                .FirstOrDefault(setting => (string)setting.Attribute("key") == key);

            string value = element == null ? null : (string)element.Attribute("value");

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Missing email setting: {0}", key)
                );
            }

            return value;
        }
    }
}
