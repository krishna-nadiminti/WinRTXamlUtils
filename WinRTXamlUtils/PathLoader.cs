using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace WinRTXamlUtils
{
    internal static class PathLoader
    {
        public const string NoPathName = "none";

        private const string NoPathData = "M9.0767605,6.6701169C9.6621458,6.6952524 10.237936,6.9441347 10.666298,7.4110131 11.520303,8.3435421 11.460409,9.7930079 10.526914,10.648583 9.5947487,11.504229 8.1456911,11.440418 7.2903168,10.507959 6.4336626,9.5728807 6.4974453,8.1259756 7.429651,7.2717004 7.896394,6.8435926 8.4913742,6.6449814 9.0767605,6.6701169z M8.9594071,4.9494162C7.9979265,4.9537315 7.0348551,5.3026032 6.2694819,6.0050964 4.6379645,7.5011778 4.526025,10.036343 6.0233934,11.669134 7.5194719,13.300725 10.057222,13.411324 11.688709,11.914632 13.322797,10.415941 13.428296,7.8814254 11.932207,6.2492347 11.13672,5.3821969 10.049085,4.9445252 8.9594071,4.9494162z M9.5676157,0L21.314934,1.6601506 21.390434,1.5924013 51.329003,34.235405 30.504963,53.333 0.5676838,20.689983 0.64190225,20.620983 0,8.7746105z";

        private const string PathXamlFormat = @"
        <Path xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
              Stretch=""Uniform"" Data=""{0}"" Fill=""#80FFFFFF""  />
";

        private static readonly Dictionary<string, Dictionary<string, string>> IconSets = 
            new Dictionary<string, Dictionary<string, string>>();

        private static Dictionary<string, string> _globalIconCache;
 
        public static void PreloadIconsFrom(string resourceDictionaryName)
        {
            if (resourceDictionaryName == null) throw new ArgumentNullException("resourceDictionaryName");
            if (string.IsNullOrWhiteSpace(resourceDictionaryName))
                throw new ArgumentException("Resource dictionary name cannot be empty", "resourceDictionaryName");

            if (IconSets.ContainsKey(resourceDictionaryName))
                return;

            //pre-load all icons
            var iconDictionary =
                Application.Current
                    .Resources
                    .MergedDictionaries
                    .FirstOrDefault(d => d.Source.ToString().Contains(resourceDictionaryName));
            
            if (iconDictionary != null)
            {
                var keys = iconDictionary.Keys.Cast<string>();
                var iconSet = new Dictionary<string, string>();
                foreach (string key in keys)
                {
                    var globalKey = GetGlobalPathName(resourceDictionaryName, key);
                    var pathDataValue = iconDictionary[key] as string;

                    iconSet.Add(globalKey, pathDataValue);
                }
                IconSets.Add(resourceDictionaryName, iconSet);

                _globalIconCache = IconSets.SelectMany(kv => kv.Value).ToDictionary(k => k.Key, v => v.Value);
            }
         }

        public static IEnumerable<string> GetPathNamesForIconSet(string resourceDictionaryName)
        {
            if (IconSets.ContainsKey(resourceDictionaryName))
            {
                return IconSets[resourceDictionaryName].Keys;
            }
            return Enumerable.Empty<string>();
        }

        public static Windows.UI.Xaml.Shapes.Path LoadFrom(string pathName, Color color)
        {
            try
            {
                string pathData = NoPathData;
                if (!string.IsNullOrWhiteSpace(pathName))
                {
                    pathData = GetPathData(pathName);
                }
                var path = (Windows.UI.Xaml.Shapes.Path)XamlReader.Load(string.Format(PathXamlFormat, pathData));
                if (path != null)
                {
                    if (color == default(Color))
                    {
                        color = Color.FromArgb(255, 255, 255, 255);
                    }
                    path.Fill = new SolidColorBrush(color);
                }
                return path;
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception ex)
// ReSharper restore EmptyGeneralCatchClause
            {
                Debug.WriteLine("Error loading path: " + pathName + ", Message: " + ex);
            }

            return null;
        }

        private static string GetPathData(string pathName)
        {           
            string pathData;

            //try get it from the cache first, otherwise get it from resources
            if (_globalIconCache.ContainsKey(pathName))
            {
                pathData = _globalIconCache[pathName];
            }
            else if (Application.Current.Resources.ContainsKey(pathName))
            {
                object pathDataObject = Application.Current.Resources[pathName];
                pathData = pathDataObject as string; //we know this won't be null or empty - because we created the resources.

                //cache it
                _globalIconCache[pathName] = pathData;
            } 
            else
            {
                pathData = NoPathData;

                //cache it
                _globalIconCache[pathName] = pathData;
            }

            return pathData;
        }

        public static string GetGlobalPathName(string resourceDictionaryName, string pathResourcePattern)
        {
            return string.Format("{0}+{1}", resourceDictionaryName, pathResourcePattern);
        }
    }
}
