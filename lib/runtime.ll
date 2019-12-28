@dnl = internal constant [4 x i8] c"%d\0A\00"
@fnl = internal constant [6 x i8] c"%.1f\0A\00"	
@d   = internal constant [3 x i8] c"%d\00"
@lf  = internal constant [4 x i8] c"%lf\00"	

%str = type {
	i8*, ; Pointer to the string
	i32  ; String length
}

define i1 @strEq(%str %s1, %str %s2) {
entry:
	%str1 = extractvalue %str %s1, 0
	%str2 = extractvalue %str %s2, 0
	%len1 = extractvalue %str %s1, 1
	%len2 = extractvalue %str %s2, 1

	%isLenEq = icmp eq i32 %len1, %len2
	br i1 %isLenEq, label %startLoop, label %retFalse

retFalse:
	ret i1 0

startLoop:
	%counter = phi i32 [0, %entry], [%nextCounter, %checkLoop]
	%shouldStop = icmp eq i32 %counter, %len1
	br i1 %shouldStop, label %retTrue, label %checkLoop

checkLoop:
	%c1 = getelementptr i8, i8* %str1, i32 %counter
	%char1 = load i8, i8* %c1
	%c2 = getelementptr i8, i8* %str2, i32 %counter
        %char2 = load i8, i8* %c2

	%charEq = icmp eq i8 %char1, %char2
	%nextCounter = add i32 1, %counter
	br i1 %charEq, label %startLoop, label %retFalse

retTrue:
	ret i1 1
}


declare void @exit(i32)
declare i32 @printf(i8*, ...) 
declare i32 @scanf(i8*, ...)
declare i32 @puts(i8*)

define void @error() {
	call void  @exit(i32 1)
	ret void
}

define void @printInt(i32 %x) {
       %t0 = getelementptr [4 x i8], [4 x i8]* @dnl, i32 0, i32 0
       call i32 (i8*, ...) @printf(i8* %t0, i32 %x) 
       ret void
}

define void @printString(%str %s) {
entry:  %r1 = extractvalue %str %s, 0
	call i32 @puts(i8* %r1)
	ret void
}

define i32 @readInt() {
entry:	%res = alloca i32
        %t1 = getelementptr [3 x i8], [3 x i8]* @d, i32 0, i32 0
	call i32 (i8*, ...) @scanf(i8* %t1, i32* %res)
	%t2 = load i32, i32* %res
	ret i32 %t2
}
