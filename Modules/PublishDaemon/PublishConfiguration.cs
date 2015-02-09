using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedDaemonLib;

namespace PublishDaemon
{
    public static class PublishConfiguration
    {
        private static AS_ConfigFile _configFile = new AS_ConfigFile();
        private const string Name = "PublishDaemon";

        public static void DefineConfigFile(string filePath)
        {
            _configFile.Dispose();
            _configFile = new AS_ConfigFile(filePath);
        }        

        public static string Collection
        {
            get { return GetValueWithDefault("Collection", "http://srvtfs2012:8080/tfs/Collection_v7"); }
            set { _configFile[Name, "Collection"] = value; }
        }

        public static string ProjectName
        {
            get { return GetValueWithDefault("ProjectName", "Actibase Radiologie 7.1"); }
            set { _configFile[Name, "ProjectName"] = value; }
        }
        public static DateTime LastBuildTime
        {
            get { return GetValueWithDefault("LastBuildTime", DateTime.MinValue); }
            set { _configFile[Name, "LastBuildTime"] = value; }
        }
        public static string LastBuildVersion
        {
            get { return GetValueWithDefault("LastBuildVersion", ""); }
            set { _configFile[Name, "LastBuildVersion"] = value; }
        }        

        public static int RefreshTime
        {
            get { return GetValueWithDefault("RefreshTime", 30000); }
            set { _configFile[Name, "RefreshTime"] = value; }
        }

        public static string MsBuildPath
        {
            get { return GetValueWithDefault("MSBuildPath", @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"); }
            set { _configFile[Name, "MSBuildPath"] = value; }
        }

        public static string SolutionPath
        {
            get { return GetValueWithDefault("SolutionPath", ""); }
            set { _configFile[Name, "SolutionPath"] = value; }
        }

        public static string PublishDirectory
        {
            get { return GetValueWithDefault("PublishDirectory", @"\\192.168.69.100\wwwroot\ARV71"); }
            set { _configFile[Name, "PublishDirectory"] = value; }
        }
        public static string UserName
        {
            get { return GetValueWithDefault("UserName", "Administrateur"); }
            set { _configFile[Name, "UserName"] = value; }
        }
        public static string Password
        {
            get { return GetValueWithDefault("Password", "1234"); }
            set { _configFile[Name, "Password"] = value; }
        }

        public static Version PublishVersion
        {
            get {
                string result = GetValueWithDefault("PublishVersion", "2.0.0.0");
                return new Version(result);
            }
            set { _configFile[Name, "PublishVersion"] = value.ToString(); }
        }

                
        //-------------------------------------------------

        private static T GetValueWithDefault<T>(string valueName, T defaultValue) where T : struct 
        {
            var result = _configFile[Name, valueName] as T?;
            if (result == null) {
                result = defaultValue;
                _configFile[Name, valueName] = result;
            }
            return result.Value;
        }
       
        private static string GetValueWithDefault(string valueName, string defaultValue)
        {
            var result = _configFile[Name, valueName] as string;
            if (result == null) {
                result = defaultValue;
                _configFile[Name, valueName] = result;
            }
            return result;
        }
    }
}
