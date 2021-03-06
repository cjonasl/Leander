﻿import sys
import os.path
import datetime
from enum import Enum
from random import randrange

class Target(Enum):
    ROW = 1
    COLUMN = 2
    SQUARE = 3

def run(sudokuArray):
    certainty_sudoku_board = None
    working_sudoku_board = return_two_dimensional_data_structure(9, 9)
    best_so_far_sudoku_board = None
    candidates_after_added_numbers_with_certainty = None
    max_number_of_attempts_to_solve_sudoku = 100
    number_of_attempts_to_solve_sudoku = 0
    number_of_cells_set_in_best_so_far = 0
    number_of_cells_set_in_input_sudoku_board = 0
    sudoku_solved = False
    numbers_added_with_certainty_and_then_no_candidates = False
    cells_remain_to_set = []
    cells_remain_to_set_after_added_numbers_with_certainty = None
    number_of_candidates = [0]
    number_of_candidates_after_added_numbers_with_certainty = [0]
    index_number = [0, 0]
    result = "S"
    saveStateDone = False
    currentSudokuToSolve = 1
    numberOfSudokusToSolve = len(sudokuArray)

    square_cell_to_row_column_mapper = return_square_cell_to_row_column_mapper()
    candidates = return_three_dimensional_data_structure(9, 9, 10)
    certainty_sudoku_board = return_two_dimensional_data_structure(9, 9)
    cells_remain_to_set_after_added_numbers_with_certainty = []
    candidates_after_added_numbers_with_certainty = return_three_dimensional_data_structure(9, 9, 10)
    best_so_far_sudoku_board = return_two_dimensional_data_structure(9, 9)

    while currentSudokuToSolve <= numberOfSudokusToSolve and result == "S":
        cells_remain_to_set.clear()
        cells_remain_to_set_after_added_numbers_with_certainty.clear()
        number_of_attempts_to_solve_sudoku = 0
        number_of_cells_set_in_best_so_far = 0
        sudoku_solved = False
        numbers_added_with_certainty_and_then_no_candidates = False
        saveStateDone = False

        get_input_sudoku_board(working_sudoku_board, cells_remain_to_set, currentSudokuToSolve - 1, sudokuArray)
        number_of_candidates[0] = init_candidates(working_sudoku_board, square_cell_to_row_column_mapper, candidates)
        number_of_cells_set_in_input_sudoku_board = 81 - len(cells_remain_to_set)

        while number_of_attempts_to_solve_sudoku < max_number_of_attempts_to_solve_sudoku and not sudoku_solved and not numbers_added_with_certainty_and_then_no_candidates:
            if number_of_attempts_to_solve_sudoku > 0:
                restore_state(cells_remain_to_set, cells_remain_to_set_after_added_numbers_with_certainty, number_of_candidates_after_added_numbers_with_certainty, working_sudoku_board, certainty_sudoku_board, candidates, candidates_after_added_numbers_with_certainty, number_of_candidates)

            while number_of_candidates[0] > 0:
                number = 0
                i = 0

                while i < len(cells_remain_to_set) and number == 0:
                    row = cells_remain_to_set[i][0]
                    column = cells_remain_to_set[i][1]
                    number = try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper)
                    if number == 0:
                        i += 1

                if number == 0:
                    simulate_one_number(candidates, cells_remain_to_set, index_number)
                    i = index_number[0]
                    number = index_number[1]
                    row = cells_remain_to_set[i][0]
                    column = cells_remain_to_set[i][1]

                    if not saveStateDone:
                        save_state(cells_remain_to_set, cells_remain_to_set_after_added_numbers_with_certainty, number_of_candidates, working_sudoku_board, certainty_sudoku_board, candidates, candidates_after_added_numbers_with_certainty, number_of_candidates_after_added_numbers_with_certainty)
                        saveStateDone = True

                working_sudoku_board[row - 1][column - 1] = number
                del cells_remain_to_set[i]
                number_of_candidates[0] -= update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number)

            if len(cells_remain_to_set) == 0:
                sudoku_solved = True
            elif not saveStateDone:
                numbers_added_with_certainty_and_then_no_candidates = True
                number_of_cells_set_in_best_so_far = 81 - len(cells_remain_to_set)
            else:
                number_of_cells_set_in_best_so_far = check_if_can_update_best_so_far_sudoku_board(number_of_cells_set_in_best_so_far, cells_remain_to_set, working_sudoku_board, best_so_far_sudoku_board)
                number_of_attempts_to_solve_sudoku += 1

        result = process_result(currentSudokuToSolve - 1, sudokuArray, sudoku_solved, number_of_cells_set_in_input_sudoku_board, number_of_cells_set_in_best_so_far, working_sudoku_board, best_so_far_sudoku_board)

        if (currentSudokuToSolve % 500) == 0:
            print(currentSudokuToSolve)

        currentSudokuToSolve += 1

    return result

def copy_list(list_from, list_to):
    list_to.clear()

    for x in list_from:
        list_to.append(x)

def copy_sudoku_board(sudoku_board_from, sudoku_board_to):
    for row in range(1, 10):
        for column in range(1, 10):
            sudoku_board_to[row - 1][column - 1] = sudoku_board_from[row - 1][column - 1]

def copy_candidates(candidates_from, candidates_to):
    for row in range(1, 10):
        for column in range(1, 10):
            for i in range(0, 10):
                candidates_to[row - 1][column - 1][i] = candidates_from[row - 1][column - 1][i]

def save_state(cells_remainT_to_set, cells_remain_to_set_after_added_numbers_with_certainty, number_of_candidates, working_sudoku_board, certainty_sudoku_board, candidates, candidates_after_added_numbers_with_certainty, number_of_candidates_after_added_numbers_with_certainty):
    copy_list(cells_remainT_to_set, cells_remain_to_set_after_added_numbers_with_certainty)
    copy_sudoku_board(working_sudoku_board, certainty_sudoku_board)
    copy_candidates(candidates, candidates_after_added_numbers_with_certainty)
    number_of_candidates_after_added_numbers_with_certainty[0] = number_of_candidates[0]

def restore_state(cells_remainT_to_set, cells_remain_to_set_after_added_numbers_with_certainty, number_of_candidates_after_added_numbers_with_certainty, working_sudoku_board, certainty_sudoku_board, candidates, candidates_after_added_numbers_with_certainty, number_of_candidates):
    copy_list(cells_remain_to_set_after_added_numbers_with_certainty, cells_remainT_to_set)
    copy_sudoku_board(certainty_sudoku_board, working_sudoku_board)
    copy_candidates(candidates_after_added_numbers_with_certainty, candidates)
    number_of_candidates[0] = number_of_candidates_after_added_numbers_with_certainty[0]

def get_input_sudoku_board(sudoku_board, cells_remain_to_set, index, sudokuArray):

    sudoku_board_string = sudokuArray[index].replace("\r\n", "\n")

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

        if n  > 0:
            for j in range(n):
                if candidates[row - 1][column - 1][1 + j] == number:
                    number_of_occurencies_of_number += 1

                    if number_of_occurencies_of_number > 1:
                        return False
    return True

def remove_number_if_it_exists(v, number):
    index = -1
    returnValue = 0
    i = 1

    while i <= v[0] and number >= v[i] and index == -1: #The numbers in v are in increasing order
        if v[i] == number:
            index = i
            returnValue = 1
        else:
            i += 1

    if index != -1:
        while index + 1 <= v[0]:
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

def check_if_can_update_best_so_far_sudoku_board(number_of_cells_set_in_best_so_far, cells_remain_to_set, working_sudoku_board, best_so_far_sudoku_board):
    ret_val = number_of_cells_set_in_best_so_far #Default

    if number_of_cells_set_in_best_so_far < (81 - len(cells_remain_to_set)):
        ret_val = 81 - len(cells_remain_to_set)
        copy_sudoku_board(working_sudoku_board, best_so_far_sudoku_board)

    return ret_val

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

def try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper):
    number = 0
    square = 1 + (3 * ((row - 1) // 3)) + (column - 1) // 3
    number_of_candidates_in_cell = candidates[row - 1][column - 1][0]

    if number_of_candidates_in_cell == 1:
        number = candidates[row - 1][column - 1][1]
    else:
        i = 1
        while i <= number_of_candidates_in_cell and number == 0:
            candidate = candidates[row - 1][column - 1][i]

            if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, row, Target.ROW):
                number = candidate
            else:
                if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, column, Target.COLUMN):
                    number = candidate
                else:
                    if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, square, Target.SQUARE):
                        number = candidate
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

def process_result(index, sudokuArray, sudoku_solved, number_of_cells_set_in_input_sudoku_board, number_of_cells_set_in_best_so_far, working_sudoku_board, best_so_far_sudoku_board):

    if sudoku_solved:
        msg = "S"
    else:
        tmp1 = number_of_cells_set_in_best_so_far - number_of_cells_set_in_input_sudoku_board
        tmp2 = 81 - number_of_cells_set_in_best_so_far
        msg = "The sudoku was partially solved. " + str(tmp1) + " number(s) added to the original " + str(number_of_cells_set_in_input_sudoku_board) + ". Unable to set " + str(tmp2) + " number(s)."

    if sudoku_solved or best_so_far_sudoku_board == None:
        sudokuArray[index] = return_sudoku_board_as_string(working_sudoku_board)
    else:
        sudokuArray[index] = return_sudoku_board_as_string(best_so_far_sudoku_board)

    return msg


start = datetime.datetime.now().strftime("%Y,%m,%d,%H,%M,%S") + "\n"

fs = open("C:\\Sudoku\\Solve\\StartSudokuBoards.txt", "rt")
s = fs.read()
fs.close()

sudokuArray = s.split("\n-- New sudoku --\n")

result = run(sudokuArray)

if result == "S":
    end = datetime.datetime.now().strftime("%Y,%m,%d,%H,%M,%S")
    s = start + end
    fs = open("C:\\Sudoku\\Solve\\StartEnd.txt", "wt")
    fs.write(s)
    fs.close()

    fs = open("C:\\Sudoku\\Solve\\SolveResult.txt", "wt")
    n = len(sudokuArray)

    for i in range(0, n):
        fs.write(sudokuArray[i])

        if i < (n - 1):
            fs.write("\n-- New sudoku --\n")

    fs.close()
else:
    fs = open("C:\\Sudoku\\Solve\\Error.txt", "wt")
    fs.write(result)
    fs.close()
