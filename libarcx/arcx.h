/*
 * ARCX -- ARC Extended
 */

#ifndef CARCX_LIBRARY_H
#define CARCX_LIBRARY_H

#include <stdint.h>
#include <stdio.h>
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif


	/**
	 * \brief Content type of a chunk
	 */
	typedef enum ArcXContentType_e
	{
		NoneSpecified,
		TextureTex,
		TextureDxt5,
		MAX_CONTENT_TYPE
	} ArcXContentType;


	/**
	 * \brief Compression algorithm used for the chunk
	 */
	typedef enum ArcXCompressionType_e
	{
		Uncompressed,
		LZ4,
		Zstd,
		Brotli,
		LZHAM,
		LZMA,
		MAX_COMPRESSION_TYPE
	} ArcXCompressionType;


	/**
	 * \brief Additional compression flags given to the chunk
	 */
	typedef enum ArcXCompressionFlags_e
	{
		None
	} ArcXCompressionFlags;

	struct ArcXContainer_s;
	struct ArcXChunk_s;

	/**
	 * \brief A file pointer in the ARCX archive.
	 */
	typedef struct ArcXFile_s
	{
		uint32_t file_name_len_bytes;
		wchar_t *file_name;
		enum ArcXContentType_e content_type;
		int32_t chunk_id;
		uint64_t size;
		uint64_t offset;
		struct ArcXChunk_s *chunk;
		struct ArcXContainer_s *container;
	} ArcXFile;


	/**
	 * \brief ARCX chunk that contains one or more files.
	 */
	typedef struct ArcXChunk_s
	{
		int32_t id;
		enum ArcXCompressionType_e compression_type;
		enum ArcXCompressionFlags_e compression_flags;
		uint32_t crc32;
		uint64_t offset;
		uint64_t compressed_length;
		uint64_t uncompressed_length;
		uint64_t contained_files_count;
		struct ArcXFile_s **contained_files;
		struct ArcXContainer_s *container;
	} ArcXChunk;


	/**
	 * \brief ARCX container
	 */
	typedef struct ArcXContainer_s
	{
		uint16_t version;
		uint64_t header_offset;
		uint64_t chunk_count;
		struct ArcXChunk_s *chunks;
		uint64_t file_count;
		struct ArcXFile_s *files;
		uint32_t target_chunk_size;
		FILE *file_handler;
	} ArcXContainer;


	/**
	 * \brief Opens an ARCX archive specified by the path
	 * \param file Path to the ARCX archive
	 * \return The ARCX container structure that represents the opened file
	 */
	ArcXContainer *ARCX_open(const char *file);


	/**
	 * \brief Closes the specified ARCX archive file handler and frees the memory associated with it
	 * \param container Container to close
	 */
	void ARCX_close(ArcXContainer *container);


	/**
	 * \brief Gets the chunk that contains the specified ARCX file data.
	 * \param file ARCX file pointer.
	 * \return ARCX chunk that contains the file data.
	 */
	ArcXChunk *ARCX_get_chunk_with_file(const ArcXFile *file);


	void* ARCX_read_file(ArcXFile *file);

	void* ARCX_read_chunk(ArcXChunk *chunk);

#ifdef __cplusplus
}
#endif

#endif