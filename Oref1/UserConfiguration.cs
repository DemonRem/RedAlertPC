using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Oref1
{
    public static class UserConfiguration
    {
        private static readonly string _configFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "UserConfig.json");

        public static event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        public static void RaiseConfigurationChangedEvent(UserConfigJson userConfig)
        {
            EventHandler<ConfigurationChangedEventArgs> configurationChanged = ConfigurationChanged;

            if (configurationChanged != null)
            {
                configurationChanged.Invoke(null, new ConfigurationChangedEventArgs(userConfig));
            }
        }

        public static UserConfigJson GetConfiguration()
        {
            try
            {
                string jsonConfig = File.ReadAllText(_configFilePath);

                if (!string.IsNullOrWhiteSpace(jsonConfig))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    try
                    {
                        UserConfigJson userConfigJson = serializer.Deserialize<UserConfigJson>(jsonConfig);

                        if (!userConfigJson.ShowConnectionNotifications.HasValue)
                        {
                            userConfigJson.ShowConnectionNotifications = true;
                        }

                        if (!userConfigJson.ShowAlertsFromUnknownAreas.HasValue)
                        {
                            userConfigJson.ShowAlertsFromUnknownAreas = true;
                        }

                        string[] newCities = Cities.ListOfCities.Select(city => city.City).Except(userConfigJson.CityConfigs.Select(cityConfig => cityConfig.City)).ToArray();

                        if (newCities.Length > 0)
                        {
                            bool allDisplayAlerts = userConfigJson.CityConfigs.All(userCityConfig => userCityConfig.DisplayAlerts);
                            bool allSoundAlerts = userConfigJson.CityConfigs.All(userCityConfig => userCityConfig.SoundAlerts);

                            IEnumerable<UserCityConfig> newCitiesConfigs = newCities.Select(city => new UserCityConfig(city, allDisplayAlerts, allSoundAlerts));

                            userConfigJson.CityConfigs = userConfigJson.CityConfigs.Concat(newCitiesConfigs).ToArray();
                        }
                        

                        return userConfigJson;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }

                    //version 1.0 config region
                    {
                        UserCityConfig[] version1Config = serializer.Deserialize<UserCityConfig[]>(jsonConfig);
                        Dictionary<string, UserCityConfig> version1ConfigDictionary = version1Config.ToDictionary(userCityConfig => userCityConfig.City);

                        bool allDisplayAlerts;
                        bool allSoundAlerts;

                        if (version1Config.Length == 1883)
                        {
                            allDisplayAlerts = version1Config.All(userCityConfig => userCityConfig.DisplayAlerts);
                            allSoundAlerts = version1Config.All(userCityConfig => userCityConfig.SoundAlerts);
                        }
                        else
                        {
                            allDisplayAlerts = false;
                            allSoundAlerts = false;
                        }

                        UserConfigJson convertedConfigJson = new UserConfigJson()
                        {
                            Version = "1.5",
                            ShowConnectionNotifications = true,
                            ShowAlertsFromUnknownAreas = true,
                            CityConfigs = Cities.ListOfCities
                            .Select(cityEntry =>
                                {
                                    if (version1ConfigDictionary.ContainsKey(cityEntry.City))
                                    {
                                        return version1ConfigDictionary[cityEntry.City];
                                    }
                                    else
                                    {
                                        return new UserCityConfig(cityEntry.City, allDisplayAlerts, allSoundAlerts);
                                    }
                                })
                            .ToArray()
                        };

                        try
                        {
                            SaveConfiguration(convertedConfigJson);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }

                        convertedConfigJson.Version = "1.0";
                        return convertedConfigJson;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            //return default configuration
            return new UserConfigJson() { 
                Version = "1.5",
                ShowConnectionNotifications = true,
                ShowAlertsFromUnknownAreas = true,
                CityConfigs = Cities.ListOfCities.Select(cityEntry => new UserCityConfig(cityEntry.City, true, false)).ToArray() 
            };
        }

        public static void SaveConfiguration(UserConfigJson userCityConfigs)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                string serializedJson = serializer.Serialize(userCityConfigs);

                File.WriteAllText(_configFilePath, serializedJson, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            RaiseConfigurationChangedEvent(userCityConfigs);
        }
    }
}
