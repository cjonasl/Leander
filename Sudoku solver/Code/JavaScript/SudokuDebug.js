var fs = require('fs');
sudoku = {};

sudoku.target = {
    ROW: "row",
    COLUMN: "column",
    SQUARE: "square"
}

sudoku.run = function(args) {
    var row = 0, column = 0, number, i;
    var certaintySudokuBoard = null;
    var bestSoFarSudokuBoard = null, workingSudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);
    var candidates, squareCellToRowColumnMapper, candidatesAfterAddedNumbersWithCertainty = null;
    var maxNumberOfAttemptsToSolveSudoku = 100, numberOfAttemptsToSolveSudoku = 0;
    var numberOfCellsSetInInputSudokuBoard = 0, numberOfCellsSetInBestSoFar = 0;
    var numberOfCandidates = [0];
    var numberOfCandidatesAfterAddedNumbersWithCertainty = [0];
    var sudokuSolved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
    var msg;
    var cellsRemainToSet = [], cellsRemainToSetAfterAddedNumbersWithCertainty = null;
    var indexNumber = [0, 0];
    var debugTotalCellsAdded = [];
    var debugDirectory, debugTry, debugAddNumber, debugSquare, debugString;
    var debugCategory = ["0"];
    var debugInfo = ["0"];

    msg = sudoku.getInputSudokuBoard(args, workingSudokuBoard, cellsRemainToSet);

    if (msg != null) {
        sudoku.printResult(false, args, msg, false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
        return;
    }

    squareCellToRowColumnMapper = sudoku.returnSquareCellToRowColumnMapper();
    msg = sudoku.validateSudokuBoard(workingSudokuBoard, squareCellToRowColumnMapper);

    if (msg != null) {
        sudoku.printResult(false, args, msg, false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
        return;
    }

    if (cellsRemainToSet.length == 0) {
        PrintResult(false, args, "A complete sudoku was given as input. There is nothing to solve.", true, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
        return;
    }

    candidates = sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
    numberOfCandidates[0] = sudoku.initCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);

    if (numberOfCandidates[0] == 0) {
        PrintResult(false, args, "It is not possible to add any number to the sudoku.", false, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
        return;
    }

    numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.length;

    debugDirectory = sudoku.debugCreateAndReturnDebugDirectory();
    debugTry = 0;

    while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates) {
        debugTry += 1;
        debugAddNumber = 0;
        debugTotalCellsAdded.splice(0, debugTotalCellsAdded.length);

        if (numberOfAttemptsToSolveSudoku > 0) {
            sudoku.restoreState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidates);
        }

        while (numberOfCandidates[0] > 0) {
            number = 0;
            i = 0;

            while (i < cellsRemainToSet.length && number == 0) {
                row = cellsRemainToSet[i][0];
                column = cellsRemainToSet[i][1];
                number = sudoku.tryFindNumberToSetInCellWithCertainty(row, column, candidates, squareCellToRowColumnMapper, debugCategory);
                i = (number == 0) ? i + 1 : i;
            }

            if (number == 0) {
                sudoku.simulateOneNumber(candidates, cellsRemainToSet, indexNumber, debugInfo);
                i = indexNumber[0];
                number = indexNumber[1];
                row = cellsRemainToSet[i][0];
                column = cellsRemainToSet[i][1];

                if (certaintySudokuBoard == null) {
                    certaintySudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);
                    cellsRemainToSetAfterAddedNumbersWithCertainty = [];
                    candidatesAfterAddedNumbersWithCertainty = sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
                    sudoku.saveState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty);
                }

                debugCategory[0] = "Simulated";
            }

            debugTotalCellsAdded.push([row, column]);

            debugSquare = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
            debugString = "(row, column, square, number, category) = (" + row + ", " + column + ", " + debugSquare + ", " + number + ", " + debugCategory[0] + ")\r\n\r\n";
            debugString += "Total cells added (" + debugTotalCellsAdded.length + " cells): " + sudoku.debugReturnCells(debugTotalCellsAdded) + "\r\n\r\n";

            if (debugCategory[0] == "Simulated") {
                debugString += debugInfo[0] + "\r\n\r\n";
            }

            debugString += "Data before update:\r\n\r\n" + sudoku.debugReturnInfo(workingSudokuBoard, cellsRemainToSet, numberOfCandidates[0], candidates, squareCellToRowColumnMapper);

            workingSudokuBoard[row - 1][column - 1] = number;
            cellsRemainToSet.splice(i, 1);
            numberOfCandidates[0] -= sudoku.updateCandidates(candidates, squareCellToRowColumnMapper, row, column, number);

            debugString += "\r\nData after update:\r\n\r\n" + sudoku.debugReturnInfo(workingSudokuBoard, cellsRemainToSet, numberOfCandidates[0], candidates, squareCellToRowColumnMapper);

            debugAddNumber += 1;
            debugFileNameFullPath = debugDirectory + "\\" + sudoku.debugReturnFileName(debugTry, debugAddNumber);
            fs.writeFileSync(debugFileNameFullPath, debugString, { encoding: 'ascii' });
        }

        if (cellsRemainToSet.length == 0) {
            sudokuSolved = true;
        }
        else if (certaintySudokuBoard == null) {
            numbersAddedWithCertaintyAndThenNoCandidates = true;
            numberOfCellsSetInBestSoFar = 81 - cellsRemainToSet.Count;
        }
        else {
            if (bestSoFarSudokuBoard == null)
                bestSoFarSudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);

            numberOfCellsSetInBestSoFar = sudoku.checkIfCanUpdateBestSoFarSudokuBoard(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard);
            numberOfAttemptsToSolveSudoku++;
        }
    }

    sudoku.printResult(true, args, null, sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
}

sudoku.sourceExists = function(source, isFile) {
  try {
     fs.lstatSync(source);

     try {
       fs.readFileSync(source);

       if (isFile)
           return true;
       else
           return false;
     }
      catch (err) {
       if (isFile)
           return false;
       else
           return true;
    }
  }
  catch (err) {
    return false;
  }
}

sudoku.copyList = function (from, to)  {
    to.splice(0, to.length);

    for (var i = 0; i < from.length; i++) {
        to.push(from[i]);
    }
}

sudoku.copySudokuBoard = function (sudokuBoardFrom, sudokuBoardTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
        }
    }
}

sudoku.copyCandidates = function (candidatesFrom, candidatesTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            for (var i = 0; i < 10; i++) {
                candidatesTo[row - 1][column - 1][i] = candidatesFrom[row - 1][column - 1][i];
            }
        }
    }
}

sudoku.saveState = function (cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty) {
    sudoku.copyList(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty);
    sudoku.copySudokuBoard(workingSudokuBoard, certaintySudokuBoard);
    sudoku.copyCandidates(candidates, candidatesAfterAddedNumbersWithCertainty);
    numberOfCandidatesAfterAddedNumbersWithCertainty[0] = numberOfCandidates[0];
}

sudoku.restoreState = function (cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidates) {
    sudoku.copyList(cellsRemainToSetAfterAddedNumbersWithCertainty, cellsRemainToSet);
    sudoku.copySudokuBoard(certaintySudokuBoard, workingSudokuBoard);
    sudoku.copyCandidates(candidatesAfterAddedNumbersWithCertainty, candidates);
    numberOfCandidates[0] = numberOfCandidatesAfterAddedNumbersWithCertainty[0];
}

sudoku.getInputSudokuBoard = function(args, sudokuBoard, cellsRemainToSet) {
    var rows, columns, sudokuBoardString;
    var row, column, n;

    if (args.length == 0) {
        return "An input file is not given to the program (first parameter)!";
    }
    else if (args.length > 2) {
        return "At most two parameters may be given to the program!";
    }
    else if (!sudoku.sourceExists(args[0], true)) {      
        return "The given input file in first parameter does not exist!";
    }
    else if (args.length == 2 && !sudoku.sourceExists(args[1], false)) {
        return "The directory given in second parameter does not exist!";
    }

    sudokuBoardString = fs.readFileSync(args[0], { encoding: 'ascii' }).trim().replace("\r\n", "\n");

    rows = sudokuBoardString.split("\n");

    if (rows.length != 9) {
        return "Number of rows in input file are not 9 as expected!";
    }

    for (row = 1; row <= 9; row++) {
        columns = rows[row - 1].split(' ');

        if (columns.length != 9) {
            return "Number of columns in input file in row " + row + " are not 9 as expected!";
        }

        for (column = 1; column <= 9; column++) {
            n = new Number(columns[column - 1]);

            if (isNaN(n) || (Math.trunc(n) != n)) {
                return "The value \"" + columns[column - 1] + "\" in row " + row + " and column " +  column + " in input file is not a valid integer!";
            }

            if (n < 0 || n > 9) {
                return "The value \"" + columns[column - 1] + "\" in row " + row + " and column " + column + " in input file is not an integer in the interval [0, 9] as expected!";
            }

            sudokuBoard[row - 1][column - 1] = n;

            if (n == 0) {
                cellsRemainToSet.push([row, column]);
            }
        }
    }

    return null;
}

sudoku.candidateIsAlonePossible = function(number, candidates, squareCellToRowColumnMapper, t, target) {
    var row = 0, column = 0, n, i, j, numberOfOccurenciesOfNumber = 0;

    for (i = 0; i < 9; i++) {
        switch (target) {
            case sudoku.target.ROW:
                row = t;
                column = i + 1;
                break;
            case sudoku.target.COLUMN:
                row = i + 1;
                column = t;
                break;
            case sudoku.target.SQUARE:
                row = squareCellToRowColumnMapper[t - 1][i][0];
                column = squareCellToRowColumnMapper[t - 1][i][1];
                break;
        }

        n = candidates[row - 1][column - 1][0];

        if (n > 0) {
            for (j = 0; j < n; j++) {
                if (candidates[row - 1][column - 1][1 + j] == number) {
                    numberOfOccurenciesOfNumber++;

                    if (numberOfOccurenciesOfNumber > 1)
                        return false;
                }
            }
        }
    }

    return true;
}

sudoku.removeNumberIfItExists = function(v, number) {
    var i, n, index = -1, returnValue = 0;

    n = v[0];
    i = 1;
    while (i <= n && index == -1) {
        if (v[i] == number) {
            index = i;
            returnValue = 1;
        }
        else
            i++;
    }

    if (index != -1) {
        while (index + 1 <= n) {
            v[index] = v[index + 1];
            index++;
        }

        v[0]--;
    }

    return returnValue;
}

sudoku.returnNumberOfOccurenciesOfNumber = function(sudokuBoard, squareCellToRowColumnMapper, number, t, target) { //t refers to a row, column or square
    var row = 0, column = 0, n = 0;

    for (var i = 0; i < 9; i++)
    {
        switch (target) {
            case sudoku.target.ROW:
                row = t;
                column = i + 1;
                break;
            case sudoku.target.COLUMN:
                row = i + 1;
                column = t;
                break;
            case sudoku.target.SQUARE:
                row = squareCellToRowColumnMapper[t - 1][i][0];
                column = squareCellToRowColumnMapper[t - 1][i][1];
                break;
        }

        if (sudokuBoard[row - 1][column - 1] == number)
            n++;
    }

    return n;
}

sudoku.returnTwoDimensionalDataStructure = function(m, n) {
    var i, j, v = [];

    for (i = 0; i < m; i++)
        v.push([]);

    for (i = 0; i < m; i++)
        for (j = 0; j < n; j++)
            v[i].push(0);
 
    return v;
}

sudoku.returnThreeDimensionalDataStructure = function(l, m, n) {
    var i, j, k, v = [];

    for (i = 0; i < l; i++)
        v.push([]);

    for (i = 0; i < l; i++)
        for (j = 0; j < m; j++)
            v[i].push([]);

    for (i = 0; i < l; i++)
        for (j = 0; j < m; j++)
            for (k = 0; k < n; k++)
                v[i][j].push(0);

    return v;
}

sudoku.returnSquareCellToRowColumnMapper = function() {
    var v, index, i, row, column, square;

    v = sudoku.returnThreeDimensionalDataStructure(9, 9, 2);

    index = [0, 0, 0, 0, 0, 0, 0, 0, 0];

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
            v[square - 1][index[square - 1]][0] = row;
            v[square - 1][index[square - 1]][1] = column;
            index[square - 1]++;
        }
    }

    return v;
}

sudoku.returnSudokuBoardAsString = function(sudokuBoard) {
    var row, column, sb = "";

    for (row = 1; row <= 9; row++) {
        if (row > 1)
            sb += "\r\n";

        for (column = 1; column <= 9; column++) {
            if (column == 1)
                sb += sudokuBoard[row - 1][column - 1];
            else
                sb += (" " + sudokuBoard[row - 1][column - 1]);
        }
    }

    return sb;
}

sudoku.returnIntegerRandomNumber = function (minIncluded, maxExcluded) {
    var n = minIncluded + Math.round(Math.random() * (maxExcluded - minIncluded));

    if (n === maxExcluded) 
        n = minIncluded;

    return n;
}

sudoku.simulateOneNumber = function (candidates, cellsRemainToSet, indexNumber, debugInfo) {
    var v = [], tmp, row, column, i, numberOfCandidates, minNumberOfCandidates = 9;

    for (i = 0; i < cellsRemainToSet.length; i++) {
        row = cellsRemainToSet[i][0];
        column = cellsRemainToSet[i][1];
        numberOfCandidates = candidates[row - 1][column - 1][0];

        if (numberOfCandidates > 0 && numberOfCandidates < minNumberOfCandidates)
            minNumberOfCandidates = numberOfCandidates;
    }

    str = "minNumberOfCandidates: " + minNumberOfCandidates + "\r\n";

    debugCellsWithMinNumberOfCandidates = [];

    for (i = 0; i < cellsRemainToSet.length; i++) {
        row = cellsRemainToSet[i][0];
        column = cellsRemainToSet[i][1];

        if (candidates[row - 1][column - 1][0] == minNumberOfCandidates) {
            debugCellsWithMinNumberOfCandidates.push([row, column]);
            v.push(i);
        }
    }

    str += "Cells with minNumberOfCandidates (" + v.length + " cells): " + sudoku.debugReturnCells(debugCellsWithMinNumberOfCandidates);
    debugInfo[0] = str;

    tmp = sudoku.returnIntegerRandomNumber(0, v.length);
    indexNumber[0] = v[tmp];
    row = cellsRemainToSet[indexNumber[0]][0];
    column = cellsRemainToSet[indexNumber[0]][1];
    indexNumber[1] = candidates[row - 1][column - 1][1 + sudoku.returnIntegerRandomNumber(0, minNumberOfCandidates)];
}

sudoku.checkIfCanUpdateBestSoFarSudokuBoard = function (numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard) {
    var retVal = numberOfCellsSetInBestSoFar; //Default

    if (numberOfCellsSetInBestSoFar < (81 - cellsRemainToSet.Count)) {
        retVal = 81 - cellsRemainToSet.Count;
        sudoku.copySudokuBoard(workingSudokuBoard, bestSoFarSudokuBoard);
    }

    return retVal;
}

sudoku.initCandidates = function(sudokuBoard, squareCellToRowColumnMapper, candidates) {
    var row, column, square, number, numberOfCandidates = 0, n;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);

            if (sudokuBoard[row - 1][column - 1] != 0) {
                candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
            }
            else {
                n = 0;
                candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                for (number = 1; number <= 9; number++) {
                    if (
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, "row") == 0) &&
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, "column") == 0) &&
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, "square") == 0)
                    ) {
                        n++;
                        candidates[row - 1][column - 1][0] = n;
                        candidates[row - 1][column - 1][n] = number;
                        numberOfCandidates++;
                    }
                }
            }
        }
    }

    return numberOfCandidates;
}

sudoku.tryFindNumberToSetInCellWithCertainty = function (row, column, candidates, squareCellToRowColumnMapper, debugCategory) {
    var i, square, numberOfCandidatesInCell, candidate, number = 0;

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
    numberOfCandidatesInCell = candidates[row - 1][column - 1][0];

    if (numberOfCandidatesInCell == 1) {
        number = candidates[row - 1][column - 1][1];
        debugCategory[0] = "Alone in cell";
    }
    else {
        i = 1;
        while (i <= numberOfCandidatesInCell && number == 0) {
            candidate = candidates[row - 1][column - 1][i];

            if (sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, row, "row")) {
                number = candidate;
                debugCategory[0] = "Alone in row";
            }
            else if (sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, column, "column")) {
                number = candidate;
                debugCategory[0] = "Alone in column";
            }
            else if (sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, square, "square")) {
                number = candidate;
                debugCategory[0] = "Alone in square";
            }
            else
                i++;
        }
    }

    return number;
}

sudoku.updateCandidates = function(candidates, squareCellToRowColumnMapper, row, column, number) {
    var i, r, c, square, totalNumberOfCandidatesRemoved;

    totalNumberOfCandidatesRemoved = candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);

    for (c = 1; c <= 9; c++) {
        if (c != column && candidates[row - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[row - 1][c - 1], number);
        }
    }

    for (r = 1; r <= 9; r++) {
        if (r != row && candidates[r - 1][column - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[r - 1][column - 1], number);
        }
    }

    for (i = 0; i < 9; i++) {
        r = squareCellToRowColumnMapper[square - 1][i][0];
        c = squareCellToRowColumnMapper[square - 1][i][1];

        if (r != row && c != column && candidates[r - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[r - 1][c - 1], number);
        }
    }

    return totalNumberOfCandidatesRemoved;
}

sudoku.validateSudokuBoard = function(sudokuBoard, squareCellToRowColumnMapper) {
    var row, column, square, number;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
            number = sudokuBoard[row - 1][column - 1];

            if (number != 0) {
                if (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, "row") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in row " + row;
                }
                else if (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, "column") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in column " + column;
                }
                else if (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, "square") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in square " + square;
                }
            }
        }
    }

    return null;
}       

sudoku.printSudokuBoard =  function(solved, args, message, sudokuBoard) {
    var file, fileName, fileNameFullPath, d, yearStr, month, date, hour, minute, second, millisecond, str, c;
    var suffix, monthStr, dateStr, hourStr, minuteStr, secondStr, millisecondStr, fileContent;
    var index1, index2;

    d = new Date();

    yearStr = d.getFullYear().toString();
    month = 1 + d.getMonth();
    date = d.getDate();
    hour = d.getHours();
    minute = d.getMinutes();
    second = d.getSeconds();
    millisecond = d.getMilliseconds();

    monthStr = (month < 10 ? "0" : "") + month.toString();
    dateStr = (date < 10 ? "0" : "") + date.toString();
    hourStr = (hour < 10 ? "0" : "") + hour.toString();
    minuteStr = (minute < 10 ? "0" : "") + minute.toString();
    secondStr = (second < 10 ? "0" : "") + second.toString();
    millisecondStr = (millisecond < 10 ? "00" : (millisecond < 100 ? "0" : "")) + millisecond.toString();

    str = args[0].trim();
    index1 = 1 + str.lastIndexOf("\\");
    index2 = str.length;
    fileName = str.substring(index1, index2);

    if (solved)
        suffix = "__Solved_" + yearStr + "." + monthStr + "." + dateStr + "." + hourStr + "." + minuteStr + "." + secondStr + "." + millisecondStr + ".txt";
    else
        suffix = "__Partially_solved_" + yearStr + "." + monthStr + "." + dateStr + "." + hourStr + "." + minuteStr + "." + secondStr + "." + millisecondStr + ".txt";

    if (args.length == 2) {
        str = args[1].trim();
        c = str[str.length - 1];
        fileNameFullPath = args[1].trim() + ((c == '\\') ? "" : "\\") + fileName + suffix;
    }
    else
        fileNameFullPath = args[0].trim() + suffix;

    fileContent = message + "\r\n\r\n" + sudoku.returnSudokuBoardAsString(sudokuBoard);

    file = fs.openSync(fileNameFullPath, "w");
    fs.writeFileSync(file, fileContent, { encoding: 'ascii' });
    fs.closeSync(file);
}

sudoku.printResult = function(initialSudokuBoardHasCandidates, args, msg, sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard) {
    var tmp1, tmp2;

    if (initialSudokuBoardHasCandidates) {
        if (sudokuSolved) {
            tmp1 = 81 - numberOfCellsSetInInputSudokuBoard;
            msg = "The sudoku was solved. " + tmp1 + " number(s) added to the original " + numberOfCellsSetInInputSudokuBoard + ".";
        }
        else {
            tmp1 = numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard;
            tmp2 = 81 - numberOfCellsSetInBestSoFar;
            msg = "The sudoku was partially solved. " + tmp1 + " number(s) added to the original " + numberOfCellsSetInInputSudokuBoard + ". Unable to set " + tmp2 + " number(s).";
        }

        if (sudokuSolved || bestSoFarSudokuBoard == null) {
            sudoku.printSudokuBoard(sudokuSolved, args, msg, workingSudokuBoard);
        }
        else {
            sudoku.printSudokuBoard(sudokuSolved, args, msg, bestSoFarSudokuBoard);
        }
    }

    console.log(msg);
}

sudoku.debugCreateAndReturnDebugDirectory = function () {
    var d, yearStr, month, date, hour, minute, second, millisecond, monthStr;
    var dateStr, hourStr, minuteStr, secondStr, millisecondStr, suffix, debugDir;

    d = new Date();

    yearStr = d.getFullYear().toString();
    month = 1 + d.getMonth();
    date = d.getDate();
    hour = d.getHours();
    minute = d.getMinutes();
    second = d.getSeconds();
    millisecond = d.getMilliseconds();

    monthStr = (month < 10 ? "0" : "") + month.toString();
    dateStr = (date < 10 ? "0" : "") + date.toString();
    hourStr = (hour < 10 ? "0" : "") + hour.toString();
    minuteStr = (minute < 10 ? "0" : "") + minute.toString();
    secondStr = (second < 10 ? "0" : "") + second.toString();
    millisecondStr = (millisecond < 10 ? "00" : (millisecond < 100 ? "0" : "")) + millisecond.toString();

    suffix = yearStr + "." + monthStr + "." + dateStr + "." + hourStr + "." + minuteStr + "." + secondStr + "." + millisecondStr;

    debugDir = "C:\\Sudoku\\Debug\\Run_" + suffix;

    fs.mkdirSync(debugDir);

    return debugDir;
}

sudoku.debugReturnFileName = function(debugTry, debugAddNumber) {
    var s1, s2;

    if (debugTry < 10)
        s1 = "00" + debugTry;
    else if (debugTry < 100)
        s1 = "0" + debugTry;
    else
        s1 = debugTry;

    if (debugAddNumber < 10)
        s2 = "0" + debugAddNumber;
    else
        s2 = debugAddNumber;

    return "Try" + s1 + "AddNumber" + s2 + ".txt";
}

sudoku.debugReturnCells = function(cellsRemainToSet) {
    var str = "";

    for (var i = 0; i < cellsRemainToSet.length; i++)
    {
        if (i > 0) {
            str += " ";
        }

        str += "(" + cellsRemainToSet[i][0] + ", " + cellsRemainToSet[i][1] + ")";
    }

    return str;
}

sudoku.debugReturnCandidates = function(row, column, candidates) {
    var str = "";
    var n = candidates[row - 1][column - 1][0];

    for (var i = 1; i <= n; i++) {
        if (i > 1) {
            str += ", ";
        }

        str += candidates[row - 1][column - 1][i];
    }

    return str;
}

sudoku.debugSort = function(n, v) {
    var tmp;

    for (var i = 0; i < n - 1; i++) {
        for (var j = i + 1; j < n; j++) {
            if (v[j] < v[i]) {
                tmp = v[j];
                v[j] = v[i];
                v[i] = tmp;
            }
        }
    }
}

sudoku.debugReturnAllCandidatesSorted = function(candidates, v, squareCellToRowColumnMapper, t, target) {
    var i, j, row = 0, column = 0, c, n = 0;
    var str = "";

    for (i = 0; i < 9; i++) {
        switch (target) {
            case sudoku.target.ROW:
                row = t;
                column = i + 1;
                break;
            case sudoku.target.COLUMN:
                row = i + 1;
                column = t;
                break;
            case sudoku.target.SQUARE:
                row = squareCellToRowColumnMapper[t - 1][i][0];
                column = squareCellToRowColumnMapper[t - 1][i][1];
                break;
        }

        if (candidates[row - 1][column - 1][0] > 0) {
            c = candidates[row - 1][column - 1][0];

            for (j = 0; j < c; j++) {
                v[n] = candidates[row - 1][column - 1][1 + j];
                n += 1;
            }
        }
    }

    sudoku.debugSort(n, v);

    for (i = 0; i < n; i++) {
        if (i > 0) {
            str += ", ";
        }

        str += v[i];
    }

    if (n > 0) {
        str += " (a total of " + n + " candidates)";
    }

    return str;
}

sudoku.debugReturnInfo = function(sudokuBoard, cellsRemainToSet, numberOfCandidates, candidates, squareCellToRowColumnMapper) {
    var str;
    var row, column, square;
    v = [];

    for (var i = 0; i < 81; i++) {
        v.push(0);
    }

    str = "Sudoku board:\r\n" + sudoku.returnSudokuBoardAsString(sudokuBoard) + "\r\n\r\nCells remain to set (" + cellsRemainToSet.length + " cells): ";
    str += sudoku.debugReturnCells(cellsRemainToSet) + "\r\n\r\n";
    str += "Number Of candidates: " + numberOfCandidates + "\r\n\r\n";
    str += "Candidates (row, column, square, numberOfCandidate):\r\n";

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
            if (candidates[row - 1][column - 1][0] == -1) {
                str += "(" + row + ", " + column + ", " + square + ", 0): Already set to " + sudokuBoard[row - 1][column - 1] + "\r\n";
            }
            else {
                str += "(" + row + ", " + column + ", " + square + ", " + candidates[row - 1][column - 1][0] + "): " + sudoku.debugReturnCandidates(row, column, candidates) + "\r\n";
            }
        }
    }

    str += "\r\nCandidates in the rows:\r\n";

    for (row = 1; row <= 9; row++) {
        str += row + ": " + sudoku.debugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, row, sudoku.target.ROW) + "\r\n";
    }

    str += "\r\nCandidates in the columns:\r\n";

    for (column = 1; column <= 9; column++) {
        str += column + ": " + sudoku.debugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, column, sudoku.target.COLUMN) + "\r\n";
    }

    str += "\r\nCandidates in the squares:\r\n";

    for (square = 1; square <= 9; square++) {
        str += square + ": " + sudoku.debugReturnAllCandidatesSorted(candidates, v, squareCellToRowColumnMapper, square, sudoku.target.SQUARE) + "\r\n";
    }

    return str;
}


var args = [];

for (var i = 2; i < process.argv.length; i++)
    args.push(process.argv[i]);

sudoku.run(args);
