#pragma once
#include <stdint.h>

struct Dict_s;
typedef struct Dict_s Dict;

Dict* dict_init(uint64_t size);
void dict_free(Dict* dict);

uint64_t dict_size(Dict* dict);
uint64_t dict_item_count(Dict* dict);

uint8_t dict_add(Dict* dict, uint64_t key, void* item);
void* dict_get(Dict* dict, uint64_t key);
void* dict_remove(Dict* dict, uint64_t key);