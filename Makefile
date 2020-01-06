
all:
	@llvm-as -o lib/runtime.bc lib/runtime.ll
	@cd src; nuget restore; msbuild
	@cp src/LatteCompiler/bin/Debug/* .
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
