import sys
import os.path
import datetime
from enum import Enum
from random import randrange

class Target(Enum):
    ROW = 1
    COLUMN = 2
    SQUARE = 3

def run(args):
    row = 0
    column = 0
    certainty_sudoku_board = None
    working_sudoku_board = return_two_dimensional_data_structure(9, 9)
    best_so_far_sudoku_board = return_two_dimensional_data_structure(9, 9)
    max_number_of_attempts_to_solve_sudoku = 100
    number_of_attempts_to_solve_sudoku = 0
    number_of_cells_set_in_best_so_far = 0
    sudoku_solved = False
    numbers_added_with_certainty_and_then_no_candidates = False
    cells_remain_to_set = []
    cells_remain_to_set_after_added_numbers_with_certainty = None
    index_number = [0, 0]
    debugCategory = ["0"]

    msg = get_input_sudoku_board(args, working_sudoku_board, cells_remain_to_set)

    if msg != None:
        print(msg)
        return

    square_cell_to_row_column_mapper = return_square_cell_to_row_column_mapper()
    msg = validate_sudoku_board(working_sudoku_board, square_cell_to_row_column_mapper)

    if msg != None:
        print(msg)
        return

    if len(cells_remain_to_set) == 0:
        print("A complete sudoku was given as input. There is nothing to solve.")
        return

    candidates = return_three_dimensional_data_structure(9, 9, 10)
    number_of_candidates = init_candidates(working_sudoku_board, square_cell_to_row_column_mapper, candidates)

    if number_of_candidates == 0:
        print("It is not possible to add any number to the sudoku.")
        return

    number_of_cells_set_in_input_sudoku_board = 81 - len(cells_remain_to_set)

    debugDirectory = DebugCreateAndReturnDebugDirectory()
    debugTry = 0

    while number_of_attempts_to_solve_sudoku < max_number_of_attempts_to_solve_sudoku and not sudoku_solved and not numbers_added_with_certainty_and_then_no_candidates:
        debugTry += 1
        debugAddNumber = 0

        if number_of_attempts_to_solve_sudoku > 0:
            copy_sudoku_board(certainty_sudoku_board, working_sudoku_board)
            copy_list(cells_remain_to_set_after_added_numbers_with_certainty, cells_remain_to_set)
            number_of_candidates = init_candidates(working_sudoku_board, square_cell_to_row_column_mapper, candidates)

        while number_of_candidates > 0:
            number = 0
            i = 0

            while i < len(cells_remain_to_set) and number == 0:
                row = cells_remain_to_set[i][0]
                column = cells_remain_to_set[i][1]
                number = try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper, debugCategory)
                if number == 0:
                    i += 1

            if number == 0:
                simulate_one_number(candidates, cells_remain_to_set, index_number)
                i = index_number[0]
                number = index_number[1]
                row = cells_remain_to_set[i][0]
                column = cells_remain_to_set[i][1]

                if certainty_sudoku_board == None:
                    certainty_sudoku_board = return_two_dimensional_data_structure(9, 9)
                    cells_remain_to_set_after_added_numbers_with_certainty = []
                    copy_sudoku_board(working_sudoku_board, certainty_sudoku_board)
                    copy_list(cells_remain_to_set, cells_remain_to_set_after_added_numbers_with_certainty)

                debugCategory[0] = "Simulated"

            debugString = "(row, column, number, category) = (" + str(row) + ", " + str(column) + ", " + str(number) + ", " + debugCategory[0] + ")\n\nData before update:\n\nSudoku board:\n" + return_sudoku_board_as_string(working_sudoku_board)

            working_sudoku_board[row - 1][column - 1] = number
            del cells_remain_to_set[i]
            number_of_candidates -= update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number)

            debugAddNumber += 1
            debugFileNameFullPath = debugDirectory + "\\" + DebugReturnFileName(debugTry, debugAddNumber)
            f = open(debugFileNameFullPath, "w")
            f.write(debugString)
            f.close()

        if number_of_cells_set_in_best_so_far < (81 - len(cells_remain_to_set)):
            number_of_cells_set_in_best_so_far = 81 - len(cells_remain_to_set)
            copy_sudoku_board(working_sudoku_board, best_so_far_sudoku_board)

        if len(cells_remain_to_set) == 0:
            sudoku_solved = True
        elif certainty_sudoku_board == None:
            numbers_added_with_certainty_and_then_no_candidates = True
        else:
            number_of_attempts_to_solve_sudoku += 1

    tmp1 = 81 - number_of_cells_set_in_input_sudoku_board
    if sudoku_solved:
        msg = "The sudoku was solved. " + str(tmp1) + " number(s) added to the original " +  str(number_of_cells_set_in_input_sudoku_board) + "."
    else:
        tmp1 = number_of_cells_set_in_best_so_far - number_of_cells_set_in_input_sudoku_board
        tmp2 = 81 - number_of_cells_set_in_best_so_far
        msg = "The sudoku was partially solved. " + str(tmp1) + " number(s) added to the original " + str(number_of_cells_set_in_input_sudoku_board) + ". Unable to set " + str(tmp2) + " number(s)."

    print_sudoku_board(sudoku_solved, args, msg, best_so_far_sudoku_board)
    print(msg)

def copy_list(list_from, list_to):
    list_to.clear()

    for x in list_from:
        list_to.append(x)

def copy_sudoku_board(sudoku_board_from, sudoku_board_to):
    for row in range(1, 10):
        for column in range(1, 10):
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
    fs.close()

    rows = sudoku_board_string.split("\n")

    if len(rows) != 9:
        return "Number of rows in input file are not 9 as expected!"

    for row in range(1, 10):
        columns = rows[row - 1].split(" ")

        if len(columns) != 9:
            return "Number of columns in input file in row " + str(row) + " are not 9 as expected!"

        for column in range(1, 10):
            if not columns[column - 1].isdigit():
                return "The value \"" + columns[column - 1] + "\" in row " + str(row) + " and column " +  str(column) + " in input file is not a valid integer!"

            n = int(columns[column - 1], 10)

            if n < 0 or n > 9:
                return "The value \"" + columns[column - 1] + "\" in row " + str(row) + " and column " + str(column) + " in input file is not an integer in the interval [0, 9] as expected!"

            sudoku_board[row - 1][column - 1] = n

            if n == 0:
                cells_remain_to_set.append([row, column])

    return None

def candidate_is_alone_possible(number, candidates, square_cell_to_row_column_mapper, t, target):
    number_of_occurencies_of_number = 0

    for i in range(9):
        if target == Target.ROW:
            row = t
            column = i + 1
        elif target == Target.COLUMN:
            row = i + 1
            column = t
        else:
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]

        n = candidates[row - 1][column - 1][0]

        if n != -1:
            for j in range(9):
                if candidates[row - 1][column - 1][1 + j] == number:
                    number_of_occurencies_of_number += 1

                    if number_of_occurencies_of_number > 1:
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

def return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, t, target):
    n = 0

    for i in range(9):
        if target == Target.ROW:
            row = t
            column = i + 1
        elif target == Target.COLUMN:
            row = i + 1
            column = t
        else:
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]

        if sudoku_board[row - 1][column - 1] == number:
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

    for row in range(1, 10):
        for column in range(1, 10):
            square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
            v[square - 1][index[square - 1]][0] = row
            v[square - 1][index[square - 1]][1] = column
            index[square - 1] += 1

    return v

def return_sudoku_board_as_string(sudoku_board):
    sb = ""

    for row in range(1, 10):
        if row > 1:
            sb += "\n"

        for column in range(1, 10):
            if column == 1:
                sb += str(sudoku_board[row - 1][column - 1])
            else:
                sb += (" " + str(sudoku_board[row - 1][column - 1]))

    return sb

def simulate_one_number(candidates, cells_remain_to_set, index_number):
    v = []
    min_number_of_candidates = 9

    for i in range(len(cells_remain_to_set)):
        row = cells_remain_to_set[i][0]
        column = cells_remain_to_set[i][1]
        number_of_candidates = candidates[row - 1][column - 1][0]

        if number_of_candidates > 0 and number_of_candidates < min_number_of_candidates:
            min_number_of_candidates = number_of_candidates

    for i in range(len(cells_remain_to_set)):
        row = cells_remain_to_set[i][0]
        column = cells_remain_to_set[i][1]

        if candidates[row - 1][column - 1][0] == min_number_of_candidates:
            v.append(i)

    tmp = randrange(0, len(v))
    index_number[0] = v[tmp]
    row = cells_remain_to_set[index_number[0]][0]
    column = cells_remain_to_set[index_number[0]][1]
    index_number[1] = candidates[row - 1][column - 1][1 + randrange(0, min_number_of_candidates)]

def init_candidates(sudoku_board, square_cell_to_row_column_mapper, candidates):
    number_of_candidates = 0

    for row in range(1, 10):
        for column in range(1, 10):
            square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3

            if sudoku_board[row - 1][column - 1] != 0:
                candidates[row - 1][column - 1][0] = -1
            else:
                n = 0
                candidates[row - 1][column - 1][0] = 0
                for number in range(1, 10):
                    if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target.ROW):
                        if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, column, Target.COLUMN):
                            if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, square, Target.SQUARE):
                                n += 1
                                candidates[row - 1][column - 1][0] = n
                                candidates[row - 1][column - 1][n] = number
                                number_of_candidates += 1

    return number_of_candidates

def try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper, debugCategory):
    number = 0
    square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
    number_of_candidates_in_cell = candidates[row - 1][column - 1][0]

    if number_of_candidates_in_cell == 1:
        number = candidates[row - 1][column - 1][1]
        debugCategory[0] = "Alone in cell"
    else:
        i = 1
        while i <= number_of_candidates_in_cell and number == 0:
            candidate = candidates[row - 1][column - 1][i]

            if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, row, Target.ROW):
                number = candidate
                debugCategory[0] = "Alone in row"
            else:
                if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, column, Target.COLUMN):
                    number = candidate
                    debugCategory[0] = "Alone in column"
                else:
                    if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, square, Target.SQUARE):
                        number = candidate
                        debugCategory[0] = "Alone in square"
                    else:
                        i += 1

    return number

def update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number):
    total_number_of_candidates_removed = candidates[row - 1][column - 1][0] #Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1 #Indicates that the cell is set already

    square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3

    for c in range(1, 10):
        if c != column and candidates[row - 1][c - 1][0] > 0:
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[row - 1][c - 1], number)

    for r in range(1, 10):
        if r != row and candidates[r - 1][column - 1][0] > 0:
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[r - 1][column - 1], number)

    for i in range(0, 9):
        r = square_cell_to_row_column_mapper[square - 1][i][0]
        c = square_cell_to_row_column_mapper[square - 1][i][1]

        if r != row and c != column and candidates[r - 1][c - 1][0] > 0:
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[r - 1][c - 1], number)

    return total_number_of_candidates_removed

def validate_sudoku_board(sudoku_board, square_cell_to_row_column_mapper):
    for row in range(1, 10):
        for column in range(1, 10):
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
    fileName = args[0][1 + index :]
    tmp = datetime.datetime.now().strftime("%Y.%m.%d.%H.%M.%S.%f")

    if solved:
        suffix = "__Solved_" + tmp[0 : len(tmp) - 3] + ".txt"
    else:
        suffix = "__Partially_solved_" + tmp[0 : len(tmp) - 3] + ".txt"

    if len(args) == 2:
        d = args[1].strip()
        c = d[len(d) - 1]
        if c == "\\":
            fileNamefullPath = d + fileName + suffix
        else:
            fileNamefullPath = d + "\\" + fileName + suffix
    else:
        fileNamefullPath = args[0] + suffix

    fileContent = message + "\n\n" + return_sudoku_board_as_string(sudoku_board)
    f = open(fileNamefullPath, "w")
    f.write(fileContent)
    f.close()

def DebugCreateAndReturnDebugDirectory():
    tmp = datetime.datetime.now().strftime("%Y.%m.%d.%H.%M.%S.%f")
    debugDir = "C:\\Sudoku\\Debug\\Run_" + tmp[0 : len(tmp) - 3]
    os.mkdir(debugDir)
    return debugDir

def DebugReturnFileName(debugTry, debugAddNumber):
    if debugTry < 10:
        s1 = "00" + str(debugTry)
    elif debugTry < 100:
        s1 = "0" + str(debugTry)
    else:
        s1 = str(debugTry)

    if debugAddNumber < 10:
        s2 = "0" + str(debugAddNumber)
    else:
        s2 = str(debugAddNumber)

    return "Try" + s1 + "AddNumber" + s2 + ".txt"

def DebugReturnCells(cellsRemainToSet):
    s = ""

    for i in range(1, len(cellsRemainToSet)):
        if i > 0:
            s += " "
        s += "(" + str(cellsRemainToSet[i][0]) + ", " + str(cellsRemainToSet[i][1]) + ")"
 
    return s;

def DebugReturnCandidates(int row, int column, candidates):
    s = ""
    n = candidates[row - 1][column - 1][0]

    for i in range(1, 1 + n):
        if i > 1:
            s += ", "

        s += str(candidates[row - 1][column - 1][i])

    return s

def DebugSort(n, v):
    for i in range(1, n - 1):
        for j in range(i + 1, n):
            if (v[j] < v[i]):
                tmp = v[j];
                v[j] = v[i];
                v[i] = tmp;

def ReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, t, Target target):
    row = 0
    column = 0
    n = 0
    s = ""

    for i in range(9):
        if target == Target.ROW:
            row = t
            column = i + 1
        elif target == Target.COLUMN:
            row = i + 1
            column = t
        else:
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]

        if candidates[row - 1][column - 1][0] > 0:
            c = candidates[row - 1][column - 1][0]

            for j in range(c):
                v[n] = candidates[row - 1][column - 1][1 + j]
                n += 1

    DebugSort(n, v)

    for i in range(9):
        if i > 0:
            s += ", "
        s += str(v[i])

    if n > 0:
        s += " (a total of " + str(n) + " candidates)"

    return s

args = []
n = len(sys.argv)

for i in range(1, n):
    args.append(sys.argv[i])

run(args)