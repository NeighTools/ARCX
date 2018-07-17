#include <zstd.h>

ZSTD_DCtx *zstd_context = NULL;

size_t decompress_data_zstd(void *input_buffer, size_t input_size, void *output_buffer,
                       size_t output_size)
{
	if (zstd_context == NULL)
		zstd_context = ZSTD_createDCtx();
	return ZSTD_decompressDCtx(zstd_context, output_buffer, output_size, input_buffer, input_size);
}

