all:
	gcc symbol_table.c -o symbol_table

lex:
	flex symtab.l
	gcc lex.yy.c -o symlex -lfl

clean:
	rm -f symbol_table symlex lex.yy.c
