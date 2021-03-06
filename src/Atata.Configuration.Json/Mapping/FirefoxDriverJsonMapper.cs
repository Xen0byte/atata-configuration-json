﻿using System;
using System.Linq;
using OpenQA.Selenium.Firefox;

namespace Atata.Configuration.Json
{
    public class FirefoxDriverJsonMapper : DriverJsonMapper<FirefoxAtataContextBuilder, FirefoxDriverService, FirefoxOptions>
    {
        protected override FirefoxAtataContextBuilder CreateDriverBuilder(AtataContextBuilder builder)
        {
            return builder.UseFirefox();
        }

        protected override void MapOptions(DriverOptionsJsonSection section, FirefoxOptions options)
        {
            base.MapOptions(section, options);

            if (section.GlobalAdditionalCapabilities != null)
            {
                foreach (var item in section.GlobalAdditionalCapabilities.ExtraPropertiesMap)
                    options.AddAdditionalCapability(item.Key, FillTemplateVariables(item.Value), true);
            }

            if (section.Proxy != null)
                options.Proxy = section.Proxy.ToProxy();

            if (section.Arguments?.Any() ?? false)
                options.AddArguments(section.Arguments);

            if (section.Profile != null)
            {
                if (options.Profile == null || !string.IsNullOrWhiteSpace(section.Profile.ProfileDirectory) || section.Profile.DeleteSourceOnClean != null)
                    options.Profile = CreateProfile(section.Profile);

                MapProfile(section.Profile, options.Profile);
            }

            if (section.Preferences != null)
            {
                foreach (var item in section.Preferences.ExtraPropertiesMap)
                    SetOptionsPreference(item.Key, item.Value, options);
            }
        }

        private void SetOptionsPreference(string name, object value, FirefoxOptions options)
        {
            switch (value)
            {
                case bool castedValue:
                    options.SetPreference(name, castedValue);
                    break;
                case int castedValue:
                    options.SetPreference(name, castedValue);
                    break;
                case long castedValue:
                    options.SetPreference(name, castedValue);
                    break;
                case double castedValue:
                    options.SetPreference(name, castedValue);
                    break;
                case string castedValue:
                    options.SetPreference(name, FillTemplateVariables(castedValue));
                    break;
                case null:
                    options.SetPreference(name, null as string);
                    break;
                default:
                    throw new ArgumentException($"Unsupported {nameof(FirefoxOptions)} preference value type: {value.GetType().FullName}. Supports: bool, int, long, double, string.", nameof(value));
            }
        }

        private FirefoxProfile CreateProfile(DriverProfileJsonSection section)
        {
            string profileDirectory = string.IsNullOrWhiteSpace(section.ProfileDirectory)
                ? null
                : section.ProfileDirectory;

            return new FirefoxProfile(profileDirectory, section.DeleteSourceOnClean ?? false);
        }

        private void MapProfile(DriverProfileJsonSection section, FirefoxProfile profile)
        {
            ObjectMapper.Map(section.ExtraPropertiesMap, profile);

            if (section.Extensions != null)
            {
                foreach (var item in section.Extensions)
                    profile.AddExtension(item);
            }

            if (section.Preferences != null)
            {
                foreach (var item in section.Preferences.ExtraPropertiesMap)
                    SetProfilePreference(item.Key, item.Value, profile);
            }
        }

        private void SetProfilePreference(string name, object value, FirefoxProfile profile)
        {
            switch (value)
            {
                case bool castedValue:
                    profile.SetPreference(name, castedValue);
                    break;
                case int castedValue:
                    profile.SetPreference(name, castedValue);
                    break;
                case string castedValue:
                    profile.SetPreference(name, FillTemplateVariables(castedValue));
                    break;
                case null:
                    throw new ArgumentNullException(nameof(value), $"Unsupported {nameof(FirefoxProfile)} preference value: null. Supports: string, int, bool.");
                default:
                    throw new ArgumentException($"Unsupported FirefoxProfile preference value type: {value.GetType().FullName}. Supports: bool, int, string.", nameof(value));
            }
        }
    }
}
