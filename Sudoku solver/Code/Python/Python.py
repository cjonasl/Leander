﻿from enum import Enum
from random import randrange
import os.path

class Target(Enum):
    ROW = 1
    COLUMN = 2
    SQUARE = 3

def run(args):
    return "ToDo"
    
def copy_list(list_from, list_to):
    list_to.clear()

    for x in list_from:
        list_to.append(x)

def copy_sudoku_board(sudoku_board_from, sudoku_board_to):
    for row in range(1, 9):
        for column in range(1, 9):
            sudoku_board_to[row - 1][column - 1] = sudoku_board_from[row - 1][column - 1]
            
def get_input_sudoku_board(args, sudoku_board, cells_remain_to_set):
    if len(args) == 0:
        return "An input file is not given to the program (first parameter)!"
    elif len(args) > 2:
        return "At most two parameters may be given to the program!" 
    elif not os.path.isfile(args[0]):
        return "The given input file in first parameter does not exist!"
    elif len(args) == 2 and (not os.path.exists(args[1]) or os.path.isfile(args[1])):
        return "The directory given in second parameter does not exist!"
    
    fs = open(args[0], "rt")
    sudoku_board_string = fs.read().strip().replace("\r\n", "\n")

    rows = sudoku_board_string.split("\n")

    if len(rows) != 9:
        return "Number of rows in input file are not 9 as expected!";
    
    for row in range(1, 9):
        columns = rows[row - 1].split(" ")

        if len(columns) != 9:
            return "Number of columns in input file in row " + str(row) + " are not 9 as expected!";
        
        for column in range(1, 9):
            if not columns[column - 1].isdigit():
                return "The value \"" + columns[column - 1] + "\" in row " + str(row) + " and column " +  str(column) + " in input file is not a valid integer!";
            
            n = int(columns[column - 1], 10)

            if n < 0 or n > 9:
                return "The value \"" + columns[column - 1] + "\" in row " + str(row) + " and column " + str(column) + " in input file is not an integer in the interval [0, 9] as expected!";
            
            sudoku_board[row - 1][column - 1] = n

            if n == 0:
                cells_remain_to_set.append([row, column])
            
    return None

def number_is_alone_candidate(number, candidates, squareCellToRowColumnMapper, t, target):
    for i in range(9):
        if target == Target.ROW:
            row = t
            column = i + 1
        elif target == Target.COLUMN:
            row = i + 1
            column = t
        else:
            row = squareCellToRowColumnMapper[t - 1][i][0]
            column = squareCellToRowColumnMapper[t - 1][i][1]
        
        n = candidates[row - 1][column - 1][0]
        numberOfOccurenciesOfNumber = 0
        
        if n != -1:
            for j in range(9):
                if candidates[row - 1][column - 1][1 + j] == number:
                    numberOfOccurenciesOfNumber += 1
                    
                    if numberOfOccurenciesOfNumber > 1:
                        return False
    return True

def remove_number_if_it_exists(v, number):
    index = -1
    returnValue = 0
    n = v[0]
    i = 1
    
    while i <= n and index == -1:
        if v[i] == number:
            index = i
            returnValue = 1
        else:
            i += 1
            
    if index != -1:
        while index + 1 <= n:
            v[index] = v[index + 1]
            index += 1
        
        v[0] -= 1
    
    return returnValue
    
def return_number_of_occurencies_of_number(sudokuBoard, squareCellToRowColumnMapper, number, t, target):
    n = 0
    
    for i in range(9):
        if target == Target.ROW:
            row = t
            column = i + 1
        elif target == Target.COLUMN:
            row = i + 1
            column = t
        else:
            row = squareCellToRowColumnMapper[t - 1][i][0]
            column = squareCellToRowColumnMapper[t - 1][i][1]
            
        if sudokuBoard[row - 1][column - 1] == number:
            n += 1
            
    return n

def return_two_dimensional_data_structure(m, n):
    v = []

    for i in range(m):
        v.append([])

    for i in range(m):
        for j in range(n):
            v[i].append(0)

    return v

def return_three_dimensional_data_structure(l, m, n):
    v = []

    for i in range(l):
        v.append([])

    for i in range(l):
        for j in range(m):
            v[i].append([])

    for i in range(l):
        for j in range(m):
            for k in range(n):
                v[i][j].append(0)

    return v

def return_square_cell_to_row_column_mapper():
    v = return_three_dimensional_data_structure(9, 9, 2)

    index = [0, 0, 0, 0, 0, 0, 0, 0, 0]

    for row in range(1, 9):
        for column in range(1, 9):
            square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
            v[square - 1][index[square - 1]][0] = row
            v[square - 1][index[square - 1]][1] = column
            index[square - 1] += 1

    return v;

def return_sudoku_board_as_string(sudoku_board):
    sb = ""

    for row in range(1, 9):
        if row > 1:
            sb += "\r\n"

        for column in range(1, 9):
            if (column == 1):
                sb += sudoku_board[row - 1][column - 1]
            else:
                sb += (" " + sudoku_board[row - 1][column - 1])
        
    return sb

def simulate_one_number(candidates, cells_remain_to_set, index_number):
    v = []
    min_number_of_candidates = 9

    for i in range(len(cells_remain_to_set)):
        row = cellsRemainToSet[i][0]
        column = cellsRemainToSet[i][1]
        number_of_candidates = candidates[row - 1][column - 1][0]

        if number_of_candidates > 0 and number_of_candidates < min_number_of_candidates:
            min_number_of_candidates = number_of_candidates

    for i in range(n):
        row = cellsRemainToSet[i][0]
        column = cellsRemainToSet[i][1]

        if candidates[row - 1][column - 1][0] == min_number_of_candidates:
            v.append(i)
    
    tmp = randrange(0, len(v))
    index_number[0] = v[tmp]
    row = cells_remain_to_set[index_number[0]][0]
    column = cells_remain_to_set[indexNumber[0]][1]
    index_number[1] = candidates[row - 1][column - 1][1 + randrange(0, min_number_of_candidates)];

def init_candidates(sudoku_board, square_cell_to_row_column_mapper, candidates):
    number_of_candidates = 0

    for row in range(1, 9):
        for column in range(1, 9):
            square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3

            if sudoku_board[row - 1][column - 1] != 0:
                candidates[row - 1][column - 1][0] = -1
            else:
                n = 0
                candidates[row - 1][column - 1][0] = 0
                for number in range(1, 9):
                    if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target.ROW):
                        if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target.COLUMN):
                            if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target.SQUARE):
                                n += 1
                                candidates[row - 1][column - 1][0] = n
                                candidates[row - 1][column - 1][n] = number
                                numberOfCandidates += 1

    return numberOfCandidates

def try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper):
    return_number = 0

    square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
    number_of_candidates_in_cell = candidates[row - 1][column - 1][0]

    if number_of_candidates_in_cell == 1:
        return_number = candidates[row - 1][column - 1][1]
    else:
        i = 1
        while i <= number_of_candidates_in_cell and return_number == 0:
            number = candidates[row - 1][column - 1][i]

            if number_is_alone_candidate(number, candidates, square_cell_to_row_column_mapper, row, Target.ROW):
                return_number = number
            else:
                if number_is_alone_candidate(number, candidates, square_cell_to_row_column_mapper, row, Target.COLUMN):
                    return_number = number
                else:
                    if number_is_alone_candidate(number, candidates, square_cell_to_row_column_mapper, row, Target.SQUARE):
                        return_number = number
                    else:
                        i += 1
    
    return return_number

def update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number):
    total_number_of_candidates_removed = candidates[row - 1][column - 1][0]; #Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1; #Indicates that the cell is set already

    square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3

    for i in range(1, 9):
        if i != column and candidates[row - 1][i - 1][0] > 0:
            total_number_of_candidates_removed += remove_numberIfItExists(candidates[row - 1][i - 1], number)
        
    for i in range(1, 9):
        if i != row and candidates[i - 1][column - 1][0] > 0:
            total_number_of_candidates_removed += remove_numberIfItExists(candidates[i - 1][column - 1], number)
        
    for i in range(1, 9):
        r = square_cell_to_row_column_mapper[square - 1][i][0]
        c = square_cell_to_row_column_mapper[square - 1][i][1]

        if r != row and c != column and candidates[r - 1][c - 1][0] > 0:
            total_number_of_candidates_removed += remove_numberIfItExists(candidates[r - 1][c - 1], number)
        
    return total_number_of_candidates_removed

def validate_sudoku_board():
    for row in range(1, 9):
        for column in range(1, 9):
            square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
            number = sudoku_board[row - 1][column - 1]

            if number != 0:
                if return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target.ROW) > 1:
                    return "The input sudoku is incorrect! The number " + str(number) + " occurs more than once in row " + str(row)      
                elif return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, column, Target.COLUMN) > 1:
                    return "The input sudoku is incorrect! The number " + str(number) + " occurs more than once in column " + str(column)        
                elif return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, square, Target.SQUARE) > 1:
                    return "The input sudoku is incorrect! The number " + str(number) + " occurs more than once in square " + str(square)

    return None

def print_sudoku_board(solved, args, message, sudoku_board):
    index = args[0].rfind("\\")
    fileName = args[0][1 + n :]
    tmp = datetime.datetime.now().strftime("%Y.%m.%d.%H.%M.%S.%f")

    if solved:
        suffix = "__Solved_" + tmp[0 : len(tmp) - 3] + ".txt"
    else:
        suffix = "__Partially_solved" + tmp[0 : len(tmp) - 3] + ".txt"

    if len(args) == 2:
        d = args[1].strip()
        c = d[len(d) - 1]
        if c == "\\":
            fileNamefullPath = d + fileName + suffix
        else:
            fileNamefullPath = d + "\\" + fileName + suffix
    else:
        fileNamefullPath = args[0] + suffix


    fileContent = message + "\r\n\r\n" + return_sudoku_board_as_string(sudoku_board)
    f = open(fileNamefullPath, "w")
    f.write(fileContent)
    f.close()