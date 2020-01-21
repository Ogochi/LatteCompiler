BASE_PATH=tests

FRONT_TESTS=$BASE_PATH/bad
EXT_TESTS=$BASE_PATH/extensions
BASE_TESTS=$BASE_PATH/good

printf "\t---FRONTEND TESTS---\n"
for f in $FRONT_TESTS/*.lat
do
	./latc_llvm $f

	if [ $? == 0 ]; then
   		printf "Test $f failed.\n"
	else
		printf '\e[1;34m%-6s\e[m\n' "Test $f succeeded!"
	fi
done

printf "\n\t---BASE TESTS---\n"
for f in $BASE_TESTS/*.lat
do
	./latc_llvm $f

	if test -f "${f: : -4}.input"
	then
		lli ${f: : -4}.bc < ${f: : -4}.input > out.tmp 2> /dev/null
	else
		lli ${f: : -4}.bc > out.tmp 2> /dev/null
	fi
	LLIRES=$?

	DIFF=$(diff out.tmp ${f: : -4}.output)
        if [ "$DIFF" != "" ] || [ $LLIRES != 0 ]
        then
                 printf "Test $f failed.\n"
	else
		printf '\e[1;34m%-6s\e[m\n' "Test $f succeeded!"
		rm ${f: : -4}.ll
	fi
done
rm $BASE_TESTS/*.bc

printf "\n\t---EXTENSION TESTS---\n"

declare -a tests_arr=("struct" "objects1" "objects2")

for test_dir in "${tests_arr[@]}"
do
	for f in $EXT_TESTS/$test_dir/*.lat
	do
        	./latc_llvm $f

        	lli ${f: : -4}.bc > out.tmp 2> /dev/null
        	LLIRES=$?

        	DIFF=$(diff out.tmp ${f: : -4}.output)
        	if [ "$DIFF" != "" ] || [ $LLIRES != 0 ]
        	then
                	 printf "Test $f failed.\n"
        	else
                	printf '\e[1;34m%-6s\e[m\n' "Test $f succeeded!"
                	rm ${f: : -4}.ll
        	fi
	done
	rm $EXT_TESTS/$test_dir/*.bc
done

rm out.tmp

