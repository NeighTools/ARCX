#include "dict.h"
#include <string.h>
#include <stdlib.h>

typedef struct DictItem_s
{
	uint8_t was_occupied;
	uint64_t key;
	void *item;
} DictItem;

struct Dict_s
{
	uint64_t size_lb;
	uint64_t count;
	DictItem *items;
};

uint64_t cmp_dict_size_log(uint64_t start, uint64_t min)
{
	uint64_t res = start;
	uint64_t t = 1 << start;

	while (t < min)
	{
		res++;
		t <<= 1;
	}

	return res;
}

uint64_t cmp_slot(uint64_t key, uint64_t shift)
{
	key ^= key >> shift;
	return (key * 11400714819323198485llu) >> (64 - shift);
}

void expand(Dict* dict)
{
	uint64_t new_size_log = cmp_dict_size_log(dict->size_lb, dict_size(dict) * 2);
	uint64_t new_size = 1 << new_size_log;

	DictItem* new_items = calloc(new_size, sizeof(DictItem));
	DictItem* items = dict->items;
	uint64_t size = 1 << dict->size_lb;
	
	dict->items = new_items;
	dict->size_lb = new_size_log;

	for(uint64_t i = 0; i < size; i++)
	{
		DictItem* item = &items[i];

		if (item->item == NULL)
			continue;

		uint64_t new_slot = cmp_slot(item->key, new_size_log);
		for(uint64_t j = 0;; j++)
		{
			uint64_t slot = (new_slot + j) % new_size;
			DictItem *new_item = &new_items[slot];

			if (new_item->item != NULL)
				continue;
			new_item->key = item->key;
			new_item->item = item->item;
			new_item->was_occupied = 1;
			break;
		}
	}

	free(items);
}

Dict *dict_init(uint64_t size)
{
	Dict *result = malloc(sizeof(Dict));
	result->size_lb = cmp_dict_size_log(1, size);
	result->count = 0;
	result->items = calloc(dict_size(result), sizeof(DictItem));
	return result;
}

void dict_free(Dict *dict)
{
	free(dict->items);
	free(dict);
}

uint64_t dict_size(Dict *dict)
{
	return 1 << dict->size_lb;
}

uint64_t dict_item_count(Dict *dict)
{
	return dict->count;
}

uint8_t dict_add(Dict *dict, uint64_t key, void *item)
{
	if (item == NULL)
		return 0;

	if (dict_size(dict) < 2 * (dict->count + 1))
		expand(dict);

	uint64_t slot = cmp_slot(key, dict->size_lb);
	uint64_t size = 1 << dict->size_lb;

	DictItem* dict_item;
	for(uint64_t i = 0;; i++)
	{
		uint64_t cur = (slot + i) % size;
		dict_item = &dict->items[cur];

		if (dict_item->item == NULL)
			break;
	}

	dict_item->key = key;
	dict_item->item = item;
	dict_item->was_occupied = 1;
	dict->count++;
	return 1;
}

DictItem *dict_item_get(Dict *dict, uint64_t key)
{
	if (dict->count == 0)
		return NULL;

	uint64_t slot = cmp_slot(key, dict->size_lb);
	uint64_t size = 1 << dict->size_lb;

	for (uint64_t i = 0;; i++)
	{
		uint64_t cur = (slot + i) % size;

		DictItem *item = &dict->items[cur];

		if (item->item == NULL && !item->was_occupied)
			return NULL;

		if (item->was_occupied && item->key == key)
			return item;
	}
}

void *dict_get(Dict *dict, uint64_t key)
{
	DictItem* item = dict_item_get(dict, key);
	return item != NULL ? item->item : NULL;
}

void *dict_remove(Dict *dict, uint64_t key)
{
	DictItem* item = dict_item_get(dict, key);

	if (item == NULL)
		return NULL;

	void* res = item->item;
	item->item = NULL;
	item->key = 0;
	return res;
}
