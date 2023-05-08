# Word Counter App

This console application reads all text files in a specified directory, counts the occurrences of each word, and writes the word counts sorted by their starting letter to files named `{letter}_count.txt` in a sub directory of the given directory called `Results`. It also counts the number of excluded words and writes it to a file named `excluded_count.txt` in the before-mentioned directory.

## Classes

- `FileReader`: reads text files and counts the occurrences of each word
- `FileWriter`: writes word counts and excluded word count to files
- `Program`: the main entry point of the application

## Example Files
- Example files are provided in a Directory inside the project root called he main entry point of the application `TextFiles`.

## Usage

When prompted, enter the path to the directory containing the text files to count. The application will skip any files with `exclude.txt` in the file name.

After the program finishes running, the word counts will be written to a files named `FILE_{letter}.txt` in the a subdirectory of the provided directory, called Results. The excluded words count will be written to a file named `excluded_count.txt` in the same directory.

The total non-excluded word count and excluded word count will be printed to the console.