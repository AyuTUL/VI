all:
	bison -d tac.y
	flex tac.l
	gcc tac.tab.c lex.yy.c -o tac -lfl

clean:
	rm -f tac.tab.c tac.tab.h lex.yy.c tac
