namespace ARCX.FileSystemLoader
{
    /// <summary>
    ///     File system using ARCX + ARC formats
    /// </summary>
    public class FileSystemArchiveEx : FileSystemArchive
    {
        public override void AddAutoPathForAllFolder()
        {
            base.AddAutoPathForAllFolder();
        }

        public override AFileBase FileOpen(string file_name)
        {
            return base.FileOpen(file_name);
        }

        public override string[] GetFileListAtExtension(string extension)
        {
            return base.GetFileListAtExtension(extension);
        }

        public override string[] GetList(string f_str_path, ListType type)
        {
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

        protected override void Dispose(bool is_release_managed_code)
        {
            base.Dispose(is_release_managed_code);
        }
    }
}