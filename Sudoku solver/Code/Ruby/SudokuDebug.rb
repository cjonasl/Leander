module Target
  ROW = Class.new
  COLUMN = Class.new
  SQUARE = Class.new
end

class Sudoku

def Sudoku.run(args)
    row = 0
    column = 0
    certainty_sudoku_board = nil
    working_sudoku_board = return_two_dimensional_data_structure(9, 9)
    best_so_far_sudoku_board = return_two_dimensional_data_structure(9, 9)
    max_number_of_attempts_to_solve_sudoku = 100
    number_of_attempts_to_solve_sudoku = 0
    number_of_cells_set_in_best_so_far = 0
    sudoku_solved = false
    numbers_added_with_certainty_and_then_no_candidates = false
    cells_remain_to_set = []
    cells_remain_to_set_after_added_numbers_with_certainty = nil
    index_number = [0, 0]
    debugCategory = ["0"]
    debugInfo = ["0"]
    debugTotalCellsAdded = []

    msg = get_input_sudoku_board(args, working_sudoku_board, cells_remain_to_set)

    if msg != nil
        print(msg)
        return
    end

    square_cell_to_row_column_mapper = return_square_cell_to_row_column_mapper()
    msg = validate_sudoku_board(working_sudoku_board, square_cell_to_row_column_mapper)

    if msg != nil
        print(msg)
        return
    end

    if cells_remain_to_set.size == 0
        print("A complete sudoku was given as input. There is nothing to solve.")
        return
    end

    candidates = return_three_dimensional_data_structure(9, 9, 10)
    number_of_candidates = init_candidates(working_sudoku_board, square_cell_to_row_column_mapper, candidates)

    if number_of_candidates == 0
        print("It is not possible to add any number to the sudoku.")
        return
    end

    number_of_cells_set_in_input_sudoku_board = 81 - cells_remain_to_set.size

    debugDirectory = Sudoku.DebugCreateAndReturnDebugDirectory()
    debugTry = 0

    while number_of_attempts_to_solve_sudoku < max_number_of_attempts_to_solve_sudoku and not sudoku_solved and not numbers_added_with_certainty_and_then_no_candidates
        debugTry += 1
        debugAddNumber = 0
        debugTotalCellsAdded.clear()

        if number_of_attempts_to_solve_sudoku > 0
            copy_sudoku_board(certainty_sudoku_board, working_sudoku_board)
            copy_list(cells_remain_to_set_after_added_numbers_with_certainty, cells_remain_to_set)
            number_of_candidates = init_candidates(working_sudoku_board, square_cell_to_row_column_mapper, candidates)
        end

        while number_of_candidates > 0
            number = 0
            i = 0

            while i < cells_remain_to_set.size and number == 0
                row = cells_remain_to_set[i][0]
                column = cells_remain_to_set[i][1]
                number = try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper, debugCategory)
                i = (number == 0) ? i + 1 : i
            end

            if number == 0
                simulate_one_number(candidates, cells_remain_to_set, index_number, debugInfo)
                i = index_number[0]
                number = index_number[1]
                row = cells_remain_to_set[i][0]
                column = cells_remain_to_set[i][1]

                if certainty_sudoku_board == nil
                    certainty_sudoku_board = return_two_dimensional_data_structure(9, 9)
                    cells_remain_to_set_after_added_numbers_with_certainty = []
                    copy_sudoku_board(working_sudoku_board, certainty_sudoku_board)
                    copy_list(cells_remain_to_set, cells_remain_to_set_after_added_numbers_with_certainty)
                end

                debugCategory[0] = "Simulated"
            end

            debugTotalCellsAdded.append([row, column])

            debugSquare = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3
            debugString = "(row, column, square, number, category) = (" + row.to_s + ", " + column.to_s + ", " + debugSquare.to_s + ", " + number.to_s + ", " + debugCategory[0] + ")\n\n"
            debugString += "Total cells added (" + debugTotalCellsAdded.size.to_s + " cells): " + Sudoku.DebugReturnCells(debugTotalCellsAdded) + "\n\n"

            if debugCategory[0] == "Simulated"
                debugString += debugInfo[0] + "\n\n"
            end

            debugString += "Data before update:\n\n" + Sudoku.DebugReturnInfo(working_sudoku_board, cells_remain_to_set, number_of_candidates, candidates, square_cell_to_row_column_mapper)

            working_sudoku_board[row - 1][column - 1] = number
            cells_remain_to_set.delete_at(i)
            number_of_candidates -= update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number)

            debugString += "\nData after update:\n\n" + Sudoku.DebugReturnInfo(working_sudoku_board, cells_remain_to_set, number_of_candidates, candidates, square_cell_to_row_column_mapper)

            debugAddNumber += 1
            debugFileNameFullPath = debugDirectory + "\\" + Sudoku.DebugReturnFileName(debugTry, debugAddNumber)
            f = File.new(debugFileNameFullPath, "w")
            f.write(debugString)
            f.close()
        end

        if number_of_cells_set_in_best_so_far < (81 - cells_remain_to_set.size)
            number_of_cells_set_in_best_so_far = 81 - cells_remain_to_set.size
            copy_sudoku_board(working_sudoku_board, best_so_far_sudoku_board)
        end

        if cells_remain_to_set.size == 0
            sudoku_solved = true
        elsif certainty_sudoku_board == nil
            numbers_added_with_certainty_and_then_no_candidates = true
        else
            number_of_attempts_to_solve_sudoku += 1
        end
    end

    tmp1 = 81 - number_of_cells_set_in_input_sudoku_board
    if sudoku_solved
        msg = "The sudoku was solved. " + tmp1.to_s + " number(s) added to the original " +  number_of_cells_set_in_input_sudoku_board.to_s + "."
    else
        tmp1 = number_of_cells_set_in_best_so_far - number_of_cells_set_in_input_sudoku_board
        tmp2 = 81 - number_of_cells_set_in_best_so_far
        msg = "The sudoku was partially solved. " + tmp1.to_s + " number(s) added to the original " + number_of_cells_set_in_input_sudoku_board.to_s + ". Unable to set " + tmp2.to_s + " number(s)."
    end

    print_sudoku_board(sudoku_solved, args, msg, best_so_far_sudoku_board)
    print(msg)
end

def Sudoku.copy_list(list_from, list_to)
    list_to.clear()

    for i in 0..list_from.size - 1
        list_to.append(list_from[i])
    end
end

def Sudoku.copy_sudoku_board(sudoku_board_from, sudoku_board_to)
    for row in 1..9
        for column in 1..9
            sudoku_board_to[row - 1][column - 1] = sudoku_board_from[row - 1][column - 1]
        end
    end
end

def Sudoku.get_input_sudoku_board(args, sudoku_board, cells_remain_to_set)
    if args.size == 0
        return "An input file is not given to the program (first parameter)!"
    elsif args.size > 2
        return "At most two parameters may be given to the program!" 
    elsif not File.file?(args[0])
        return "The given input file in first parameter does not exist!"
    elsif args.size == 2 and not Dir.exist?(args[1])
        return "The directory given in second parameter does not exist!"
    end

    fs = open(args[0], "r")
    sudoku_board_string = fs.read().strip().gsub("\r\n", "\n")
    fs.close()

    rows = sudoku_board_string.split("\n")

    if rows.size != 9
        return "Number of rows in input file are not 9 as expected!"
    end

    for row in 1..9
        columns = rows[row - 1].split(" ")

        if columns.size != 9
            return "Number of columns in input file in row " + row.to_s + " are not 9 as expected!"
        end

        for column in 1..9
            n = Integer(columns[column - 1]) rescue nil
            if n == nil
                return "The value \"" + columns[column - 1] + "\" in row " + row.to_s + " and column " +  column.to_s + " in input file is not a valid integer!"
            end

            if n < 0 or n > 9
                return "The value \"" + columns[column - 1] + "\" in row " + row.to_s + " and column " + column.to_s + " in input file is not an integer in the interval [0, 9] as expected!"
            end

            sudoku_board[row - 1][column - 1] = n

            if n == 0
                cells_remain_to_set.append([row, column])
            end
        end
    end

    return nil
end

def Sudoku.candidate_is_alone_possible(number, candidates, square_cell_to_row_column_mapper, t, target)
    number_of_occurencies_of_number = 0

    for i in 0..8
        if target == Target::ROW
            row = t
            column = i + 1
        elsif target == Target::COLUMN
            row = i + 1
            column = t
        else
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]
        end

        n = candidates[row - 1][column - 1][0]

        if n > 0
            for j in 0..n - 1
                if candidates[row - 1][column - 1][1 + j] == number
                    number_of_occurencies_of_number += 1

                    if number_of_occurencies_of_number > 1
                        return false
                    end
                end
            end
        end
    end

    return true
end

def Sudoku.remove_number_if_it_exists(v, number)
    index = -1
    returnValue = 0
    n = v[0]
    i = 1

    while i <= n and index == -1
        if v[i] == number
            index = i
            returnValue = 1
        else
            i += 1
        end
    end

    if index != -1
        while index + 1 <= n
            v[index] = v[index + 1]
            index += 1
        end

        v[0] -= 1
    end

    return returnValue
end

def Sudoku.return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, t, target)
    n = 0

    for i in 0..8
        if target == Target::ROW
            row = t
            column = i + 1
        elsif target == Target::COLUMN
            row = i + 1
            column = t
        else
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]
        end

        if sudoku_board[row - 1][column - 1] == number
            n += 1
        end
    end

    return n
end

def Sudoku.return_two_dimensional_data_structure(m, n)
    v = []

    for i in 0..m - 1
        v.append([])
    end

    for i in 0..m - 1
        for j in 0..n - 1
            v[i].append(0)
        end
    end

    return v
end

def Sudoku.return_three_dimensional_data_structure(l, m, n)
    v = []

    for i in 0..l - 1
        v.append([])
    end

    for i in 0..l - 1
        for j in 0..m - 1
            v[i].append([])
        end
    end

    for i in 0..l - 1
        for j in 0..m - 1
            for k in 0..n - 1
                v[i][j].append(0)
            end
        end
    end

    return v
end

def Sudoku.return_square_cell_to_row_column_mapper()
    v = return_three_dimensional_data_structure(9, 9, 2)

    index = [0, 0, 0, 0, 0, 0, 0, 0, 0]

    for row in 1..9
        for column in 1..9
            square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3
            v[square - 1][index[square - 1]][0] = row
            v[square - 1][index[square - 1]][1] = column
            index[square - 1] += 1
        end
    end

    return v
end

def Sudoku.return_sudoku_board_as_string(sudoku_board)
    sb = ""

    for row in 1..9
        if row > 1
            sb += "\n"
        end

        for column in 1..9
            if column == 1
                sb += sudoku_board[row - 1][column - 1].to_s
            else
                sb += (" " + sudoku_board[row - 1][column - 1].to_s)
            end
        end
    end

    return sb
end

def Sudoku.simulate_one_number(candidates, cells_remain_to_set, index_number, debugInfo)
    v = []
    debugCellsWithMinNumberOfCandidates = []
    min_number_of_candidates = 9

    for i in 0..cells_remain_to_set.size - 1
        row = cells_remain_to_set[i][0]
        column = cells_remain_to_set[i][1]
        number_of_candidates = candidates[row - 1][column - 1][0]

        if number_of_candidates > 0 and number_of_candidates < min_number_of_candidates
            min_number_of_candidates = number_of_candidates
        end
    end

    s = "minNumberOfCandidates: " + min_number_of_candidates.to_s + "\n"

    for i in 0..cells_remain_to_set.size - 1
        row = cells_remain_to_set[i][0]
        column = cells_remain_to_set[i][1]

        if candidates[row - 1][column - 1][0] == min_number_of_candidates
            debugCellsWithMinNumberOfCandidates.append([row, column])
            v.append(i)
        end
    end

    s += "Cells with minNumberOfCandidates (" + v.size.to_s + " cells): " + Sudoku.DebugReturnCells(debugCellsWithMinNumberOfCandidates)
    debugInfo[0] = s

    tmp = rand(v.size)
    index_number[0] = v[tmp]
    row = cells_remain_to_set[index_number[0]][0]
    column = cells_remain_to_set[index_number[0]][1]
    index_number[1] = candidates[row - 1][column - 1][1 + rand(min_number_of_candidates)]
end

def Sudoku.init_candidates(sudoku_board, square_cell_to_row_column_mapper, candidates)
    number_of_candidates = 0

    for row in 1..9
        for column in 1..9
            square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3

            if sudoku_board[row - 1][column - 1] != 0
                candidates[row - 1][column - 1][0] = -1
            else
                n = 0
                candidates[row - 1][column - 1][0] = 0
                for number in 1..9
                    if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target::ROW)
                        if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, column, Target::COLUMN)
                            if 0 == return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, square, Target::SQUARE)
                                n += 1
                                candidates[row - 1][column - 1][0] = n
                                candidates[row - 1][column - 1][n] = number
                                number_of_candidates += 1
                            end
                        end
                    end
                end
            end
        end
    end

    return number_of_candidates
end

def Sudoku.try_find_number_to_set_in_cell_with_certainty(row, column, candidates, square_cell_to_row_column_mapper, debugCategory)
    number = 0
    square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3
    number_of_candidates_in_cell = candidates[row - 1][column - 1][0]

    if number_of_candidates_in_cell == 1
        number = candidates[row - 1][column - 1][1]
        debugCategory[0] = "Alone in cell"
    else
        i = 1
        while i <= number_of_candidates_in_cell and number == 0
            candidate = candidates[row - 1][column - 1][i]

            if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, row, Target::ROW)
                number = candidate
                debugCategory[0] = "Alone in row"
            else
                if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, column, Target::COLUMN)
                    number = candidate
                    debugCategory[0] = "Alone in column"
                else
                    if candidate_is_alone_possible(candidate, candidates, square_cell_to_row_column_mapper, square, Target::SQUARE)
                        number = candidate
                        debugCategory[0] = "Alone in square"
                    else
                        i += 1
                    end
                end
            end
        end
    end

    return number
end

def Sudoku.update_candidates(candidates, square_cell_to_row_column_mapper, row, column, number)
    total_number_of_candidates_removed = candidates[row - 1][column - 1][0] #Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1 #Indicates that the cell is set already

    square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3

    for c in 1..9
        if c != column and candidates[row - 1][c - 1][0] > 0
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[row - 1][c - 1], number)
        end
    end

    for r in 1..9
        if r != row and candidates[r - 1][column - 1][0] > 0
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[r - 1][column - 1], number)
        end
    end

    for i in 0..8
        r = square_cell_to_row_column_mapper[square - 1][i][0]
        c = square_cell_to_row_column_mapper[square - 1][i][1]

        if r != row and c != column and candidates[r - 1][c - 1][0] > 0
            total_number_of_candidates_removed += remove_number_if_it_exists(candidates[r - 1][c - 1], number)
        end
    end

    return total_number_of_candidates_removed
end

def Sudoku.validate_sudoku_board(sudoku_board, square_cell_to_row_column_mapper)
    for row in 1..9
        for column in 1..9
            square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3
            number = sudoku_board[row - 1][column - 1]

            if number != 0
                if return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, row, Target::ROW) > 1
                    return "The input sudoku is incorrect! The number " + number.to_s + " occurs more than once in row " + row.to_s
                elsif return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, column, Target::COLUMN) > 1
                    return "The input sudoku is incorrect! The number " + number.to_s + " occurs more than once in column " + column.to_s
                elsif return_number_of_occurencies_of_number(sudoku_board, square_cell_to_row_column_mapper, number, square, Target::SQUARE) > 1
                    return "The input sudoku is incorrect! The number " + number.to_s + " occurs more than once in square " + square.to_s
                end
            end
        end
    end

    return nil
end

def Sudoku.print_sudoku_board(solved, args, message, sudoku_board)
    index = 1 + args[0].rindex("\\")
    fileName = args[0][index, args[0].size - index]
    current_date_time = Time.now
    tmp = current_date_time.strftime("%Y.%m.%d.%H.%M.%S.") + current_date_time.usec.to_s[0, 3]

    if solved
        suffix = "__Solved_" + tmp + ".txt"
    else
        suffix = "__Partially_solved_" + tmp + ".txt"
    end

    if args.size == 2
        d = args[1].strip()
        c = d[d.size - 1]
        if c == "\\"
            fileNamefullPath = d + fileName + suffix
        else
            fileNamefullPath = d + "\\" + fileName + suffix
        end
    else
        fileNamefullPath = args[0] + suffix
    end

    fileContent = message + "\n\n" + return_sudoku_board_as_string(sudoku_board)
    f = File.new(fileNamefullPath, "w")
    f.write(fileContent)
    f.close()
end

def Sudoku.DebugCreateAndReturnDebugDirectory()
    current_date_time = Time.now
    tmp = current_date_time.strftime("%Y.%m.%d.%H.%M.%S.") + current_date_time.usec.to_s[0, 3]
    debugDir = "C:\\Sudoku\\Debug\\Run_" + tmp
    Dir.mkdir(debugDir)
    return debugDir
end

def Sudoku.DebugReturnFileName(debugTry, debugAddNumber)
    if debugTry < 10
        s1 = "00" + debugTry.to_s
    elsif debugTry < 100
        s1 = "0" + debugTry.to_s
    else
        s1 = debugTry.to_s
    end

    if debugAddNumber < 10
        s2 = "0" + debugAddNumber.to_s
    else
        s2 = debugAddNumber.to_s
    end

    return "Try" + s1 + "AddNumber" + s2 + ".txt"
end

def Sudoku.DebugReturnCells(cellsRemainToSet)
    s = ""

    for i in 0..cellsRemainToSet.size - 1
        if i > 0
            s += " "
        end
        s += "(" + cellsRemainToSet[i][0].to_s + ", " + cellsRemainToSet[i][1].to_s + ")"
    end
 
    return s
end

def Sudoku.DebugReturnCandidates(row, column, candidates)
    s = ""
    n = candidates[row - 1][column - 1][0]

    for i in 1..n
        if i > 1
            s += ", "
        end

        s += candidates[row - 1][column - 1][i].to_s
    end

    return s
end

def Sudoku.DebugSort(n, v)
    for i in 0...n - 1
        for j in i + 1...n
            if v[j] < v[i]
                tmp = v[j]
                v[j] = v[i]
                v[i] = tmp
            end
        end
    end
end

def Sudoku.DebugReturnAllCandidatesSorted(candidates, v, square_cell_to_row_column_mapper, t, target)
    row = 0
    column = 0
    n = 0
    s = ""

    for i in 0...9
        if target == Target::ROW
            row = t
            column = i + 1
        elsif target == Target::COLUMN
            row = i + 1
            column = t
        else
            row = square_cell_to_row_column_mapper[t - 1][i][0]
            column = square_cell_to_row_column_mapper[t - 1][i][1]
        end

        if candidates[row - 1][column - 1][0] > 0
            c = candidates[row - 1][column - 1][0]

            for j in 0...c
                v[n] = candidates[row - 1][column - 1][1 + j]
                n += 1
            end
        end
    end

    Sudoku.DebugSort(n, v)

    for i in 0...n
        if i > 0
            s += ", "
        end
        s += v[i].to_s
    end

    if n > 0
        s += " (a total of " + n.to_s + " candidates)"
    end

    return s
end

def Sudoku.DebugReturnInfo(sudokuBoard, cellsRemainToSet, numberOfCandidates, candidates, squareCellToRowColumnMapper)

    v = []

    for i in 0...81
        v.append(0)
    end

    s = "Sudoku board:\n" + return_sudoku_board_as_string(sudokuBoard) + "\n\nCells remain to set (" + cellsRemainToSet.size.to_s + " cells): "
    s += Sudoku.DebugReturnCells(cellsRemainToSet) + "\n\n"
    s += "Number Of candidates: " + numberOfCandidates.to_s + "\n\n"
    s += "Candidates (row, column, square, numberOfCandidate):\n"

    for row in 1...10
        for column in 1...10
            square = 1 + (3 * ((row - 1) / 3)) + (column - 1) / 3
            if candidates[row - 1][column - 1][0] == -1
                s += "(" + row.to_s + ", " + column.to_s + ", " + square.to_s + ", 0): Already set to " + sudokuBoard[row - 1][column - 1].to_s + "\n"
            else
                s += "(" + row.to_s + ", " + column.to_s + ", " + square.to_s + ", " + + candidates[row - 1][column - 1][0].to_s + "): " + Sudoku.DebugReturnCandidates(row, column, candidates) + "\n"
            end
        end
    end

    s += "\nCandidates in the rows:\n"

    for row in 1..9
        s += row.to_s + ": " + Sudoku.DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, row, Target::ROW) + "\n"
    end

    s += "\nCandidates in the columns:\n"

    for column in 1..9
        s += column.to_s + ": " + Sudoku.DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, column, Target::COLUMN) + "\n"
    end

    s += "\nCandidates in the squares:\n"

    for square in 1..9
        s += square.to_s + ": " + Sudoku.DebugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, square, Target::SQUARE) + "\n"
    end

    return s
end

end


Sudoku.run(ARGV)