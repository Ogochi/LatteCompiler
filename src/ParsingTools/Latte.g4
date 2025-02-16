grammar Latte;

program
    : topDef+
    ;

topDef
    : type ID '(' arg? ')' block                           # functionDef
    | 'class' ID ('extends' ID)? '{' fieldOrMethodDef* '}' # classDef
    ;

fieldOrMethodDef
    : fieldDef ';'                                         # classFieldDef
    | methodDef                                            # classMethodDef
    ;

fieldDef
    : type ID ( ',' ID )*
    ;

methodDef
    : type ID '(' arg? ')' block
    ;

arg
    : type ID ( ',' type ID )*
    ;

block
    : '{' stmt* '}'
    ;

stmt
    : ';'                                 # Empty
    | block                               # BlockStmt
    | type item ( ',' item )* ';'         # Decl
    | ID '=' expr ';'                     # Ass
    | expr '.' ID '=' expr ';'            # StructAss
    | ID '++' ';'                         # Incr
    | ID '--' ';'                         # Decr
    | expr '.' ID '++' ';'                # StructIncr
    | expr '.' ID '--' ';'                # StructDecr
    | 'return' expr ';'                   # Ret
    | 'return' ';'                        # VRet
    | 'if' '(' expr ')' stmt              # Cond
    | 'if' '(' expr ')' stmt 'else' stmt  # CondElse
    | 'while' '(' expr ')' stmt           # While
    | expr ';'                            # SExp
    ;

type
    : ID                                  # TTypeName
    | 'int'                               # TInt
    | 'string'                            # TString
    | 'boolean'                           # TBool
    | 'void'                              # TVoid
    ;

item
    : ID
    | ID '=' expr
    ;

expr
    : expr '.' ID '(' ( expr ( ',' expr )* )? ')' # EMethodCall
    | '(' type ')' 'null'                         # ENullCast
    | expr '.' ID                                 # EObjectField
    | unOp expr                                   # EUnOp
    | expr mulOp expr                             # EMulOp
    | expr addOp expr                             # EAddOp
    | expr relOp expr                             # ERelOp
    | <assoc=right> expr '&&' expr                # EAnd
    | <assoc=right> expr '||' expr                # EOr
    | ID                                          # EId
    | INT                                         # EInt
    | 'true'                                      # ETrue
    | 'false'                                     # EFalse
    | 'null'                                      # ENull
    | 'new' type                                  # ENewObject
    | ID '(' ( expr ( ',' expr )* )? ')'          # EFunCall
    | STR                                         # EStr
    | '(' expr ')'                                # EParen
    ;

unOp
    : '-' # UnaryMinus
    | '!' # UnaryNeg
    ;

addOp
    : '+' # Plus
    | '-' # Minus
    ;

mulOp
    : '*' # Multiply
    | '/' # Divide
    | '%' # Modulo
    ;

relOp
    : '<'  # LessThan
    | '<=' # LessEquals
    | '>'  # GreaterThan
    | '>=' # GreaterEquals
    | '==' # Equals
    | '!=' # NotEquals
    ;

COMMENT : ('#' ~[\r\n]* | '//' ~[\r\n]*) -> channel(HIDDEN);
MULTICOMMENT : '/*' .*? '*/' -> channel(HIDDEN);

fragment Letter  : Capital | Small ;
fragment Capital : [A-Z\u00C0-\u00D6\u00D8-\u00DE] ;
fragment Small   : [a-z\u00DF-\u00F6\u00F8-\u00FF] ;
fragment Digit : [0-9] ;

INT : Digit+ ;
fragment ID_First : Letter | '_';
ID : ID_First (ID_First | Digit)* ;

WS : (' ' | '\r' | '\t' | '\n')+ ->  skip;

STR
    :   '"' StringCharacters? '"'
    ;
fragment StringCharacters
    :   StringCharacter+
    ;
fragment
StringCharacter
    :   ~["\\]
    |   '\\' [tnr"\\]
    ;

ErrorCharacter : . ;