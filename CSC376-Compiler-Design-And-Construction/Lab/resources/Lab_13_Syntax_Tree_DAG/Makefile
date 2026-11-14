all:
	bison -d ast.y
	flex ast.l
	gcc ast.tab.c lex.yy.c -o ast_bison -lfl

clean:
	rm -f ast.tab.c ast.tab.h lex.yy.c ast_bison
