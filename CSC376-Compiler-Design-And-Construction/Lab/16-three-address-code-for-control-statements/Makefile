all:
	bison -d backpatch.y
	flex backpatch.l
	gcc backpatch.tab.c lex.yy.c -o backpatch -lfl

clean:
	rm -f backpatch.tab.c backpatch.tab.h lex.yy.c backpatch
