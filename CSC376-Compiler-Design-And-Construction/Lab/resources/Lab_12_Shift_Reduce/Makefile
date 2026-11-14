all:
	bison -d calc.y
	flex calc.l
	gcc calc.tab.c lex.yy.c -o shift_reduce -lfl

clean:
	rm -f calc.tab.c calc.tab.h lex.yy.c shift_reduce
