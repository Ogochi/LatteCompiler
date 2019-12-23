BASE_PATH=tests

FRONT_TESTS=$BASE_PATH/bad
EXT_TESTS=$BASE_PATH/extensions
BASE_TESTS=$BASE_PATH/good

printf "\t---FRONTEND TESTS---\n"
for f in $FRONT_TESTS/*.lat
do
	./latc_llvm $f 2> /dev/null

	if [ $? == 1 ]; then
   		printf "Test $f failed.\n"
	fi
done
rm $FRONT_TESTS/*.ll
rm $FRONT_TESTS/*.bc

printf "\n\t---BASE TESTS---\n"
for f in $BASE_TESTS/*.lat
do
	./latc_llvm $f > out.tmp

	DIFF=$(diff out.tmp ${f: : -4}.output)
        if [ "$DIFF" != "" ]
        then
                 printf "Test $f failed.\n"
        fi
done
rm $BASE_TESTS/*.ll
rm $BASE_TESTS/*.bc
rm out.tmp

