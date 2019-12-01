
all:
	@msbuild
	@cp LLVMCompiler/bin/Debug/* .
	@mv LLVMCompiler.exe latc_llvm
clean:
	@rm -f latc_*
	@rm -f *.dll
	@rm -f *.xml
	@rm -f *.exe
	@rm -f *.pdb
	@rm -f *.mdb
