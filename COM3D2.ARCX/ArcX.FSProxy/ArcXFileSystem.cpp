#include <string>
#include "ArcXFileSystem.h"
#include "logging.h"

#define LOG_MSG(msg) *(self->logger) << msg << std::endl;

void SetBaseDirectory(FileSystemArchiveNative *fs, char *path)
{
	auto self = GET_ARCX_THIS(fs, 0);

	LOG_MSG("Setting base directory to " << path);

	self->original_funcs->SetBaseDirectory(fs, path);
}

void AddArchive(FileSystemArchiveNative *fs, char *path)
{
	auto self = GET_ARCX_THIS(fs, 0);
	LOG_MSG("Adding archive: " << path);

	self->original_funcs->AddArchive(fs, path);
}

void AddAutoPathForAllFolder(FileSystemArchiveNative *fs)
{
	auto self = GET_ARCX_THIS(fs, 0);
	LOG_MSG("Add autp path for all folder!");

	self->original_funcs->AddAutoPathForAllFolder(fs);
}

void AddAutoPath(FileSystemArchiveNative *fs, char *path)
{
	auto self = GET_ARCX_THIS(fs, 0);
	LOG_MSG("Adding auto path to " << path);


	self->original_funcs->AddAutoPath(fs, path);
}

std::vector<std::string*> *CreateList(FileSystemArchiveNative *fs, std::vector<std::string*> *list, char *path,
                                      ListType list_type)
{
	auto self = GET_ARCX_THIS(fs, 4);

	LOG_MSG("Trying to create file list for " << path);

	auto result = self->original_funcs->CreateList(fs, list, path, list_type);

	LOG_MSG("Got list with " << std::dec << result->size() << " files!");

	return result;
}

void *GetFile(FileSystemArchiveNative *fs, char *file_str)
{
	auto self = GET_ARCX_THIS(fs, 4);
	LOG_MSG("FILE: " << file_str);


	return self->original_funcs->GetFile(fs, file_str);
}


void *GetFileWide(FileSystemArchiveNative *fs, wchar_t *path)
{
	auto self = GET_ARCX_THIS(fs, 2);

	LOG_MSG("FILE (WIDE): " << narrow(path));

	return self->original_funcs->GetFileWide(fs, path);
}
