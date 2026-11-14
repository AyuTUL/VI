all:
	bison -d mini.y
	flex mini.l
	gcc mini.tab.c lex.yy.c symbol_table.c -o minic -lfl

run: all
	./minic < sample.mc

clean:
	rm -f mini.tab.c mini.tab.h lex.yy.c minic
