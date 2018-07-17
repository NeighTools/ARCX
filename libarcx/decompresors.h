#ifndef DECOMPRESSORS_H
#define DECOMPRESSORS_H

#include "arcx.h"
#include <stdlib.h>

typedef size_t (*Decompressor)(void *input_buffer, size_t input_size, void *output_buffer, size_t output_size);

#define DEF_DECOMPRESSOR(name) \
	size_t decompress_data_##name(void *input_buffer, size_t input_size, void *output_buffer, size_t output_size);

#define INIT_DECOMPRESSOR(name) &decompress_data_##name
#define NULL_DECOMPRESSOR NULL

DEF_DECOMPRESSOR(uncompressed);
DEF_DECOMPRESSOR(zstd);

Decompressor DECOMPRESSORS[MAX_COMPRESSION_TYPE] = {
	INIT_DECOMPRESSOR(uncompressed),
	NULL_DECOMPRESSOR,
	INIT_DECOMPRESSOR(zstd),
	NULL_DECOMPRESSOR,
	NULL_DECOMPRESSOR,
	NULL_DECOMPRESSOR
};

inline Decompressor get_decompressor(ArcXChunk *chunk)
{
	return DECOMPRESSORS[chunk->compression_type];
}

#endif
