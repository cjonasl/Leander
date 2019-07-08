Create an ordinary console project in Visual studio.
Delete Program.cs from the project and instead include
Sudoku.cs. Build the project. The program has one
mandatory argument, which is file name (full path)
to the sudoku to be solved. If the file is in same
directory as the exe-file then file name is enough
and the program may be called for example:
Sudoku.exe SudokuToSolve.txt

The input file with sudoku to solve must be an ansi-file
with exactly 9 rows and 9 columns with numbers 0-9, where
0 indicates that the cell is not set, for example:

0 0 3 5 0 0 0 1 0
0 9 7 1 0 0 0 3 0
0 4 0 0 7 0 0 0 0
0 0 0 0 3 0 6 0 2
0 0 0 4 0 2 0 0 0
5 0 8 0 6 0 0 0 0
0 0 0 0 4 0 0 6 0
0 5 0 0 0 8 9 4 0
0 6 0 0 0 9 1 0 0

The program puts the result in a file, "Result.txt",
in the same directory as the input file.