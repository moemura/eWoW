using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace eWoW.Common
{
    public static class Config
    {
        public const string LogonServerConfig = "LogonServerConfig.xml";
        public const string RealmServerConfig = "RealmServerConfig.xml";
        public const string SqlConfigFile = "SqlConfig.xml";
        public const string WorldServerConfig = "WorldServerConfig.xml";

        /// <summary>
        /// This will hold *any* loaded config files, so we don't need to load them over and over.
        /// </summary>
        private static readonly Dictionary<string, XElement> ConfigFiles = new Dictionary<string, XElement>();

        private static string AppPath { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }

        /// <summary>
        /// Gets a value from the specified config file. Note: this does not search attributes!!
        /// </summary>
        /// <typeparam name="T">A base ValueType. (int, string, short, uint, etc)</typeparam>
        /// <param name="configFile">The NAME of the config file (including extension) to load.</param>
        /// <param name="elementName">The element containing the value to retrieve.</param>
        /// <returns></returns>
        public static T GetValue<T>(string configFile, string elementName)
        {
            XElement xe = GetXml(configFile);

            XElement ele = (from element in xe.Descendants(elementName)
                            select element).FirstOrDefault();

            if (ele == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(ele.Value, typeof(T));
        }

        public static T GetValue<T>(string configFile, params string[] elementHeirarchy)
        {
            XElement xe = GetXml(configFile);

            foreach (string e in elementHeirarchy)
            {
                xe = xe.Descendants(e).FirstOrDefault();
                if (xe == null)
                {
                    return default(T);
                }
            }

            if (xe == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(xe.Value, typeof(T));
        }

        public static T GetAttributeValue<T>(string configFile, string attributeName, params string[] elementHeirarchy)
        {
            XElement xe = GetXml(configFile);

            foreach (string e in elementHeirarchy)
            {
                xe = xe.Descendants(e).FirstOrDefault();
                if (xe == null)
                {
                    return default(T);
                }
            }

            if (xe == null || xe.Attribute(attributeName) == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(xe.Attribute(attributeName), typeof(T));
        }

        /// <summary>
        /// Gets a value from the specified config file, using an attribute. 
        /// </summary>
        /// <typeparam name="T">A base ValueType. (int, string, short, uint, etc)</typeparam>
        /// <param name="configFile">The NAME of the config file (including extension) to load.</param>
        /// <param name="elementName">The element containing the attribute to retrieve.</param>
        /// <param name="attributeName">The attribute containing the value to retrieve.</param>
        /// <returns></returns>
        public static T GetAttributeValue<T>(string configFile, string elementName, string attributeName)
            where T : struct
        {
            XElement xe = GetXml(configFile);

            XElement ele = (from element in xe.Descendants(elementName)
                            select element).FirstOrDefault();

            if (ele == null || ele.Attribute(attributeName) == null)
            {
                return default(T);
            }

            return (T) Convert.ChangeType(ele.Attribute(attributeName).Value, typeof(T));
        }

        private static XElement GetXml(string file)
        {
            if (!ConfigFiles.ContainsKey(file))
            {
                string filePath = AppPath + "\\Config\\" + file;
                Logging.Write(filePath);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Could not find the specified config file.", file);
                }
                ConfigFiles.Add(file, XElement.Load(filePath));
            }
            return ConfigFiles[file];
        }
    }
}