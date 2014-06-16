using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;

namespace WinRTXamlUtils
{
    internal static class ImageLoader
    {
        public static FrameworkElement LoadPathFrom(string pathResourcePattern, string resourceDictionaryName, Color fillColor = default(Color))
        {
            PathLoader.PreloadIconsFrom(resourceDictionaryName);

            var paths = PathLoader.GetPathNamesForIconSet(resourceDictionaryName);

            if (string.IsNullOrWhiteSpace(pathResourcePattern))
                pathResourcePattern = PathLoader.NoPathName;

            pathResourcePattern = PathLoader.GetGlobalPathName(resourceDictionaryName, pathResourcePattern);

            var matchingPaths = 
                (from p in paths
                 where string.Equals(p, pathResourcePattern, StringComparison.OrdinalIgnoreCase)
                 orderby GetPathPartSequence(p)
                 select PathLoader.LoadFrom(p, fillColor))
                .ToList();

            //just return the path if there is only one
            if (matchingPaths.Count() == 1)
            {
                return matchingPaths.First();
            }

            //if we have multiple paths, create a grid out of it and return that
            var grid = new Windows.UI.Xaml.Controls.Grid();
            foreach (var path in matchingPaths)
            {
                grid.Children.Add(path);
            }

            return grid;
        }

        private static short GetPathPartSequence(string pathName)
        {
            //a path name is of the form path_n, where n is a number
            //if there is only one path, it is denoted as just 'path' without an underscore

            if (!pathName.Contains("_") || pathName.EndsWith("_"))
                return 0;

            string indexPart = pathName.Substring(pathName.LastIndexOf('_') + 1);

            short index;
            short.TryParse(indexPart, out index);

            return index;
        }
    }
}
