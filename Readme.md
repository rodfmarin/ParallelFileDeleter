ParallelFileDeleter
==============

A command line utility that deletes a list of files using Parallel tasks
--------------

A simple tool that reads a .csv containing file paths and deletes them using parallel tasks.

The input csv is formatted as:

	C:\folder\sub-folder\file1.txt
	C:\folder\sub-folder\file2.txt
	C:\folder2\sub-folder2\file3.txt
	...

To execute the utility, from the command line:
	ParallelFileDeleter.exe "C:\Path\To\CSVDirectory\" "C:\Path\To\ErrorLogDirectory\"
