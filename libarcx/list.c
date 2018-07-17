#include "list.h"
#include <stdint.h>
#include <stdlib.h>

struct List_s
{
	uint64_t count;
	ListItem *first;
	ListItem *last;
};

struct ListItem_s
{
	ListItem *prev;
	ListItem *next;
	void *item;
};

List *list_init()
{
	List *res = malloc(sizeof(List));
	res->first = NULL;
	res->last = NULL;
	res->count = 0;
	return res;
}

void list_free(List *list)
{
	for (ListItem *item = list->first; item != NULL;)
	{
		ListItem *i = item;
		item = item->next;
		free(i);
	}
	free(list);
}

void list_append(List *list, void *item)
{
	ListItem *list_item = malloc(sizeof(ListItem));
	list_item->item = item;
	list_item->prev = NULL;
	list_item->next = NULL;

	ListItem *last = list->last;
	if (last == NULL)
	{
		list->first = list->last = list_item;
		list->count++;
		return;
	}
	list_item->prev = last;
	last->next = list_item;
	list->last = list_item;
	list->count++;
}

void list_prepend(List *list, void *item)
{
	ListItem *list_item = malloc(sizeof(ListItem));
	list_item->item = item;
	list_item->prev = NULL;
	list_item->next = NULL;

	ListItem *first = list->first;
	if (first == NULL)
	{
		list->first = list->last = list_item;
		return;
	}
	list_item->next = first;
	first->prev = list_item;
	list->first = list_item;
	list->count++;
}

void **list_to_array(List *list)
{
	if (list->count == 0)
		return NULL;

	void **res = calloc(list->count, sizeof(void*));

	ListItem *item = list->first;

	uint64_t i = 0;
	do
	{
		res[i] = item->item;
		i++;
	}
	while ((item = item->next) != NULL);

	return res;
}

uint64_t list_count(List *list)
{
	return list->count;
}

ListItem *list_first_item(List *list)
{
	return list->first;
}

ListItem *list_last_item(List *list)
{
	return list->last;
}

ListItem *list_next_item(ListItem *item)
{
	return item->next;
}

ListItem *list_prev_item(ListItem *item)
{
	return item->prev;
}

void *list_remove(List *list, ListItem *item)
{
	void *res = item->item;

	if (item->next == NULL)
		list->last = NULL;
	if (item->prev == NULL)
		list->first = NULL;
	if (item->next != NULL && item->prev != NULL)
	{
		item->next->prev = item->prev;
		item->prev->next = item->next;
	}

	free(item);
	list->count--;

	return res;
}
