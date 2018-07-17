#pragma once
#include <stdint.h>

struct List_s;
typedef struct List_s List;

struct ListItem_s;
typedef struct ListItem_s ListItem;

List* list_init();
void list_free(List* list);

void list_append(List* list, void* item);
void list_prepend(List* list, void* item);

void** list_to_array(List* list);

uint64_t list_count(List *list);

ListItem* list_first_item(List* list);
ListItem* list_last_item(List* list);

ListItem* list_next_item(ListItem* item);
ListItem* list_prev_item(ListItem* item);

void* list_remove(List* list, ListItem* item);

