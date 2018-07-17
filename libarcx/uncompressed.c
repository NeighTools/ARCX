#include <stdlib.h>
#include <string.h>

size_t decompress_data_uncompressed(void *input_buffer, size_t input_size, void *output_buffer, size_t output_size)
{
	size_t final_size = input_size < output_size ? input_size : output_size;
	memcpy_s(output_buffer, final_size, input_buffer, final_size);
	return final_size;
}