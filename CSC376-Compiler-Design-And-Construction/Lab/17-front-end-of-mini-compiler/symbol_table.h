#ifndef SYMBOL_TABLE_H
#define SYMBOL_TABLE_H
int st_insert(const char *name, const char *type);
const char *st_type(const char *name);
void st_print(void);
#endif
