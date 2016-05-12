// <copyright file="DirectoryHelper.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>
namespace ScrapProject
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper that will try find the file in the current directory tree, if it doesn't find the file it will perform a full disk scan!
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// searches up the directory tree to find the file
        /// </summary>
        /// <param name="filename">the filename to search for</param>
        /// <param name="checkedPaths">a list of all the previous paths searched</param>
        /// <param name="current">the current directory to search</param>
        /// <returns>the path of the file if it was found or string.Empty if not</returns>
        public static string AscendFileSystem(string filename, ConcurrentBag<string> checkedPaths, string current)
        {
            var parent = Directory.GetParent(current);
            if (parent == null)
            {
                return string.Empty;
            }

            var file = Path.Combine(parent.FullName, filename);
            if (File.Exists(file))
            {
                return file;
            }

            checkedPaths.Add(parent.ToString());

            foreach (var path in parent.GetDirectories())
            {
                var result = DescendFileSystem(filename, checkedPaths, path.FullName);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }

            // Didn't find anything at this level, lets go up again
            return AscendFileSystem(filename, checkedPaths, parent.FullName);
        }

        /// <summary>
        /// Walks the directory tree looking for the filename
        /// </summary>
        /// <param name="filename">the file to search for</param>
        /// <param name="checkedPaths">the list of paths that have already been checked</param>
        /// <param name="current">the current path that we are on</param>
        /// <returns>the path to the filename if it is found</returns>
        public static string DescendFileSystem(string filename, ConcurrentBag<string> checkedPaths, string current)
        {
            if (checkedPaths.Contains(current) || current.Contains("$RECYCLE.BIN"))
            {
                return string.Empty;
            }

            var file = Path.Combine(current, filename);
            if (File.Exists(file))
            {
                return file;
            }

            checkedPaths.Add(current);

            try
            {
                var paths = Directory.GetDirectories(current);
                foreach (string path in paths)
                {
                    var result = DescendFileSystem(filename, checkedPaths, path);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// Finds the file in the directory tree starting with the current directory
        /// Will search the directories under the current first then start walking back to the root to find the file.
        /// </summary>
        /// <param name="filename">the file to find</param>
        /// <returns>the path to the filename requested or string.empty if not</returns>
        /// <remarks>Make sure the file you are looking for is close to the application, and that you are searching for a specific name</remarks>
        public static string FindFile(string filename)
        {
            var current = Directory.GetCurrentDirectory();
            var checkedPaths = new ConcurrentBag<string>();

            var file = Path.Combine(current, filename);
            if (File.Exists(file))
            {
                // Found it!
                return file;
            }

            // descencd all the child paths.
            var result = DescendFileSystem(filename, checkedPaths, current);
            if (string.IsNullOrEmpty(result))
            {
                result = AscendFileSystem(filename, checkedPaths, current);
            }

            return result;
        }
    }
}
