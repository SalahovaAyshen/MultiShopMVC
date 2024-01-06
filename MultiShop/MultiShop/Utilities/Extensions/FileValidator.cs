﻿namespace MultiShop.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type)) return true;
            return false;
        }
        public static bool ValidateSize(this IFormFile file, int kb)
        {
            if (file.Length < kb * 1024) return true;
            return false;
        }
        public static string Root(string filename, string root, params string[] folders)
        {
            string path = root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, filename);
            return path;
        }
        public static async Task<string> CreateFileAsync(this IFormFile file, string root, params string[] folders)
        {
            string filename=Guid.NewGuid().ToString()+file.FileName;
            string path = Root(filename, root, folders);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filename;
        }
       
    }
}
