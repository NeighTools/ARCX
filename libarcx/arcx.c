#include "arcx.h"
#include "dict.h"
#include "list.h"
#include "decompresors.h"
#include <stdlib.h>

uint32_t read_int(FILE *file)
{
	uint32_t count = 0, shift = 0;
	uint8_t b;
	do
	{
		if (shift == 5 * 7)
			return 0;

		fread(&b, 1, 1, file);
		count |= (b & 0x7F) << shift;
		shift += 7;
	}
	while ((b & 0x80) != 0);

	return count;
}

// Archive functions
ArcXContainer *ARCX_open(const char *file)
{
	FILE *stream = fopen(file, "rb");

	char magic[4];
	fread(&magic, 4, 1, stream);

	if (!strcmp(magic, "ARCX"))
	{
		fclose(stream);
		return NULL;
	}

	ArcXContainer *result = malloc(sizeof(ArcXContainer));

	fread(&result->version, 2, 1, stream);
	fread(&result->header_offset, 8, 1, stream);
	_fseeki64(stream, result->header_offset, SEEK_SET);
	//fseeko64(stream, result->header_offset, SEEK_SET);

	fread(&result->file_count, 8, 1, stream);
	fread(&result->target_chunk_size, 4, 1, stream);
	fread(&result->chunk_count, 8, 1, stream);

	result->files = calloc(result->file_count, sizeof(ArcXFile));

	Dict* dict = dict_init(result->chunk_count);

	for (uint64_t i = 0; i < result->file_count; i++)
	{
		ArcXFile *arcx_file = &result->files[i];
		arcx_file->file_name_len_bytes = read_int(stream);
		arcx_file->file_name = (wchar_t*)calloc(1, arcx_file->file_name_len_bytes + 2);
		fread(arcx_file->file_name, 1, arcx_file->file_name_len_bytes, stream);
		fread(&arcx_file->chunk_id, 4, 1, stream);
		fread(&arcx_file->content_type, 2, 1, stream);
		fread(&arcx_file->offset, 8, 1, stream);
		fread(&arcx_file->size, 8, 1, stream);
		arcx_file->container = result;
		arcx_file->chunk = NULL;

		List* list = dict_get(dict, arcx_file->chunk_id);
		if(list == NULL)
		{
			list = list_init();
			dict_add(dict, arcx_file->chunk_id, list);
		}
		list_append(list, arcx_file);
	}

	result->chunks = calloc(result->chunk_count, sizeof(ArcXChunk));

	for (uint64_t i = 0; i < result->chunk_count; i++)
	{
		ArcXChunk *chunk = &result->chunks[i];

		fread(&chunk->id, 4, 1, stream);
		fread(&chunk->compression_type, 1, 1, stream);
		fread(&chunk->compression_flags, 1, 1, stream);
		fread(&chunk->crc32, 4, 1, stream);
		fread(&chunk->offset, 8, 1, stream);
		fread(&chunk->compressed_length, 8, 1, stream);
		fread(&chunk->uncompressed_length, 8, 1, stream);
		chunk->container = result;

		List *list = dict_remove(dict, chunk->id);
		chunk->contained_files_count = list_count(list);
		chunk->contained_files = list_to_array(list);

		for (uint64_t j = 0; j < chunk->contained_files_count; j++)
			chunk->contained_files[j]->chunk = chunk;

		list_free(list);
	}

	dict_free(dict);

	result->file_handler = stream;

	return result;
}

void ARCX_close(ArcXContainer *container)
{
	fclose(container->file_handler);

	for (uint64_t i = 0; i < container->file_count; i++)
	{
		ArcXFile *file = &container->files[i];
		free(file->file_name);
	}

	free(container->files);

	for (uint64_t j = 0; j < container->chunk_count; j++)
	{
		ArcXChunk *chunk = &container->chunks[j];
		if (chunk->contained_files != NULL)
			free(chunk->contained_files);
	}

	free(container->chunks);

	free(container);
}

// Chunk functions
void *ARCX_read_chunk(ArcXChunk *chunk)
{
	void *raw_data = malloc(chunk->compressed_length);
	_fseeki64(chunk->container->file_handler, chunk->offset, SEEK_SET);
	fread(raw_data, 1, chunk->compressed_length, chunk->container->file_handler);

	void *output = malloc(chunk->uncompressed_length);
	get_decompressor(chunk)(raw_data, chunk->compressed_length, output, chunk->uncompressed_length);
	free(raw_data);

	return output;
}

// File functions
ArcXChunk *ARCX_get_chunk_with_file(const ArcXFile *file)
{
	for (uint64_t i = 0; i < file->container->file_count; i++)
	{
		if (file->container->chunks[i].id == file->chunk_id)
			return &file->container->chunks[i];
	}
	return NULL;
}

void *ARCX_read_file(ArcXFile *file)
{
	ArcXChunk *chunk = ARCX_get_chunk_with_file(file);
	void *chunk_data = ARCX_read_chunk(chunk);
	void *result = malloc(file->size);
	memcpy_s(result, file->size, (char*)chunk_data + file->offset, file->size);
	free(chunk_data);
	return result;
}
