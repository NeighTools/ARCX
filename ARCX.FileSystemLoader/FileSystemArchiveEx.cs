using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ARCX.Core.Archive;

namespace ARCX.FileSystemLoader
{
    /// <summary>
    ///     File system using ARCX + ARC formats
    /// </summary>
    public class FileSystemArchiveEx : FileSystemArchive
    {
        private List<ArcXContainer> loadedArchives = new List<ArcXContainer>();

        public override bool AddArchive(string path)
        {
            Trace.TraceInformation($"Trying to open {path}");
            //try
            //{
            //    ArcXContainer archive = new ArcXContainer(File.OpenRead(path));
            //    loadedArchives.Add(archive);
            //}
            //catch (Exception e)
            //{
            //    Trace.TraceWarning($"Failed to read {path} as ARCX. Reason: {e.Message}");
            //    Trace.TraceWarning("Falling back to normal ARC file loader...");
            //}
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
            Trace.TraceInformation($"Trying to open {file_name}");
            return base.FileOpen(file_name);
        }

        public override string[] GetFileListAtExtension(string extension)
        {
            Trace.TraceInformation($"Trying to get file list at extension: {extension}");
            return base.GetFileListAtExtension(extension);
        }

        public override string[] GetList(string f_str_path, ListType type)
        {
            Trace.TraceInformation($"Trying to get list from {f_str_path} of type: {type}");
            return base.GetList(f_str_path, type);
        }

        public override bool IsExistentFile(string file_name)
        {
            return base.IsExistentFile(file_name);
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
        }
    }
}