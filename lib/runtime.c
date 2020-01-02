#include <stdio.h>
#include <string.h>
#include <stdlib.h>

void printInt(int x) {
    printf("%d\n", x);
}

int readInt() {
    int x;
    scanf("%d", &x);

    return x;
}

void printString(char *s) {
    puts(s);
}

char* readString() {
    char* result = NULL;
    size_t size;

    getline(&result, &size, stdin);
    size_t lastCharPos = strlen(result) - 1;
    if (result[lastCharPos] == '\n') {
      result[lastCharPos] = 0;
    }

    return result;
}

void error() {
    exit(-1);
}

int strEq(char *s1, char *s2) {
    size_t s1Size = strlen(s1);
    if (s1Size != strlen(s2)) {
        return 0;
    }

    for (int i = 0; i < s1Size; i++) {
        if (s1[i] != s2[i]) {
            return 0;
        }
    }

    return 1;
}

char* strConcat(char *s1, char *s2) {
    size_t s1Size = strlen(s1), s2Size = strlen(s2), resultSize = s1Size + s2Size + 1;
    char* result = malloc(sizeof(char) * resultSize);

    memcpy(result, s1, s1Size);
	  memcpy(result + s1Size, s2, s2Size);
    result[resultSize - 1] = 0;

    return result;
}

char* mmalloc(int size) {
    return malloc(size);
}
