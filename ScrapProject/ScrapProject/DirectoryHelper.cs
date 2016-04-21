using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapProject
{
    public static class DirectoryHelper
    {
        public static string FindFile(string filename)
        {
            var current = Directory.GetCurrentDirectory();
            ConcurrentBag<string> checkedPaths = new ConcurrentBag<string>();

            string file = Path.Combine(current, filename);
            if (File.Exists(file))
            {
                // Found it!
                return file;
            }

            // descencd all the child paths. 
            string result = DescendFileSystem(filename, checkedPaths, current);
            if (string.IsNullOrEmpty(result))
            {
                result = AscendFileSystem(filename, checkedPaths, current);
            }

            return result;
        }

        public static string DescendFileSystem(string filename, ConcurrentBag<string> checkedPaths, string current)
        {
            if (checkedPaths.Contains(current) || current.Contains("$RECYCLE.BIN"))
            {
                return string.Empty;
            }

            string file = Path.Combine(current, filename);
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
                    string result = DescendFileSystem(filename, checkedPaths, path);
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

        public static string AscendFileSystem(string filename, ConcurrentBag<string> checkedPaths, string current)
        {
            var parent = Directory.GetParent(current);
            if (parent == null)
            {
                return string.Empty;
            }

            string file = Path.Combine(parent.FullName, filename);
            if (File.Exists(file))
            {
                return file;
            }



            checkedPaths.Add(parent.ToString());

            foreach (var path in parent.GetDirectories())
            {
                string result = DescendFileSystem(filename, checkedPaths, path.FullName);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }

            // Didn't find anything at this level, lets go up again
            return AscendFileSystem(filename, checkedPaths, parent.FullName);
        }
    }
}
