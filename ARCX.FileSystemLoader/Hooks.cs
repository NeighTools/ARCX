using System;
using System.Reflection;

namespace ARCX.FileSystemLoader
{
    public static class Hooks
    {
        public static void InitARCX()
        {
            Type gameUty = typeof(GameUty);

            FieldInfo fileSystemField = gameUty.GetField("m_FileSystem", BindingFlags.Static | BindingFlags.NonPublic);
            FieldInfo fileSystemOldField = gameUty.GetField("m_FileSystemOld", BindingFlags.Static | BindingFlags.NonPublic);

            FileSystemArchive fileSystem = fileSystemField.GetValue(null) as FileSystemArchive;
            fileSystem.Dispose();
            fileSystemField.SetValue(null, new FileSystemArchiveEx());

            FileSystemArchive fileSystemOld = fileSystemOldField.GetValue(null) as FileSystemArchive;
            fileSystemOld.Dispose();
            fileSystemOldField.SetValue(null, new FileSystemArchiveEx());
        }
    }
}