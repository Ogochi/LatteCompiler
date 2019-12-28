
all:
	@clang -S -emit-llvm lib/runtime.c -o lib/runtime.ll
	@llvm-as -o lib/runtime.bc lib/runtime.ll
	@msbuild
	@cp LatteCompiler/bin/Debug/* .
	@mv LatteCompiler.exe latc_llvm
	@chmod +x latc_llvm
clean:
	@rm -f latc_*
	@rm -f *.dll
	@rm -f *.xml
	@rm -f *.exe
	@rm -f *.pdb
	@rm -f *.mdb
	@rm -f *.ll
	@rm -f lib/*.bc
	@rm -f lib/*.ll
