# LatteCompiler

Project done as a part of **Compiler construction** class at **University of Warsaw**. It is basically compiler from language similar to **Java** to **LLVM**. 

## Description

Project includes:
 - Frontend
 - LLVM backend
 - Using registers and phi instead of alloc
 - Optimisations:
    - Calculating constant expressions
    - Constants propagation in LLVM
    - Removing unreachable branches of conditional statements

## Requirements

In order to be able to build the project you will need:
- **Mono** - 6.4.0
- **MSBuild** - 16.3.0
- **NuGet** - 5.2.0

## Compilation

Building the project:
```
make
```
This command restores missing packages, compiles solution using **MSBuild**, copies results to the main directory and also creates executable
responsible for compilation to **LLVM** (`latc_llvm`).


## Execution

Created executable (`latc_llvm`) when used, execute under the hood corresponsing executables from C# solution
using **Mono**.

## Used tools

Despite C# standard library I used **ANTLR4** ([link](https://www.antlr.org/)) in the project. This library allowed me to generate
parser and helpful tools using grammar in specific format (`Latte.g4`).

For project compilation I used **MSBuild** and **Mono**.

Responsible for packages is **NuGet**.

## Project structure

```
README.md
Makefile
Zad1.g4                   - Grammar in format accepted by ANTLR4 equivalent to Instant.cf 
|
src
|   LatteCompiler.sln     - Whole solution config file
|
└───Common                - Common AST structures and error management
└───Frontend              - Frontend with static checker
└───LatteCompiler         - Main project with solution entry point
└───LlvmGenerator         - LLVM code generator from AST
└───ParsingTools          - Project generating parsing tools from defined grammar using ANTLR4
|
lib                       - Required LLVM runtime with predefined and helper functions
   
```
