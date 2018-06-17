using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARCX.Core.Archive;

namespace ARCX.FileSystemLoader
{
    /// <summary>
    ///     File system using ARCX + ARC formats
    /// </summary>
    public class FileSystemArchiveEx : FileSystemArchive
    {
        private readonly List<ArcXContainer> loadedArchives = new List<ArcXContainer>();

        private readonly Dictionary<string, ArcXFile> nameToFileDict =
                new Dictionary<string, ArcXFile>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, ArcXFile> pathNameToDict =
                new Dictionary<string, ArcXFile>(StringComparer.InvariantCultureIgnoreCase);

        public override bool AddArchive(string path)
        {
            Console.WriteLine($"Trying to open archive {path}");
            try
            {
                ArcXContainer archive = new ArcXContainer(File.OpenRead(path));
                loadedArchives.Add(archive);

                foreach (ArcXFile arcXFile in archive.Files)
                {
                    pathNameToDict[arcXFile.Filename] = arcXFile;
                    nameToFileDict[Path.GetFileName(arcXFile.Filename)] = arcXFile;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read {path} as ARCX. Reason: {e.Message}");
                Console.WriteLine("Falling back to normal ARC file loader...");
            }

            return base.AddArchive(path);
        }

        public override bool AddAutoPath(string path)
        {
            return base.AddAutoPath(path);
        }

        public override void AddAutoPathForAllFolder()
        {
            base.AddAutoPathForAllFolder();
        }

        public override void AddPatchDecryptPreferredSearchDirectory(string path)
        {
            base.AddPatchDecryptPreferredSearchDirectory(path);
        }

        public override void ClearPatchDecryptPreferredSearchDirectory()
        {
            base.ClearPatchDecryptPreferredSearchDirectory();
        }

        public override AFileBase FileOpen(string file_name)
        {
            Console.WriteLine($"Trying to open file {file_name}");

            if (nameToFileDict.TryGetValue(file_name, out ArcXFile file) || pathNameToDict.TryGetValue(file_name, out file))
            {
                Console.WriteLine("Found file in the ARCX archive!");
                return new ArcXMemoryFile(file);
            }

            return base.FileOpen(file_name);
        }

        public override string[] GetFileListAtExtension(string extension)
        {
            Console.WriteLine($"Trying to get file list at extension: {extension}");

            List<string> result = new List<string>(base.GetFileListAtExtension(extension));
            result.AddRange(nameToFileDict.Keys.Where(k => k.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)));
            return result.ToArray();
        }

        public override string[] GetList(string f_str_path, ListType type)
        {
            Console.WriteLine($"Trying to get list from {f_str_path} of type: {type}");

            // TODO: Implement other types as well
            // AllFile is enough for base game

            if (type != ListType.AllFile)
                base.GetList(f_str_path, type);

            List<string> result = new List<string>(base.GetList(f_str_path, type));
            result.AddRange(nameToFileDict.Keys);
            return result.ToArray();
        }

        public override bool IsExistentFile(string file_name)
        {
            return nameToFileDict.ContainsKey(file_name) || pathNameToDict.ContainsKey(file_name) || base.IsExistentFile(file_name);
        }

        public override bool IsValid()
        {
            return base.IsValid();
        }

        public override bool SetBaseDirectory(string path)
        {
            return base.SetBaseDirectory(path);
        }

        protected override void Dispose(bool is_release_managed_code)
        {
            base.Dispose(is_release_managed_code);

            if (is_release_managed_code)
            {
                // TODO: Dispose of ARCX containers!
            }
        }
    }
}