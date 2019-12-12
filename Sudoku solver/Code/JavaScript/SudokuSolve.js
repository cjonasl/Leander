'use strict';

var fs = require('fs');

var sudoku = {};

sudoku.target = {
    ROW: "row",
    COLUMN: "column",
    SQUARE: "square"
};

sudoku.run = function (sudokuArray) {
    var row, column, number, i;
    var certaintySudokuBoard = null;
    var bestSoFarSudokuBoard = null, workingSudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);
    var candidates, squareCellToRowColumnMapper, candidatesAfterAddedNumbersWithCertainty = null;
    var maxNumberOfAttemptsToSolveSudoku = 100, numberOfAttemptsToSolveSudoku = 0;
    var numberOfCellsSetInInputSudokuBoard = 0, numberOfCellsSetInBestSoFar = 0;
    var numberOfCandidates = [0];
    var numberOfCandidatesAfterAddedNumbersWithCertainty = [0];
    var sudokuSolved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
    var cellsRemainToSet = [], cellsRemainToSetAfterAddedNumbersWithCertainty = null;
    var indexNumber = [0, 0];
    var result = "S";
    var saveStateDone = false;
    var currentSudokuToSolve = 1;
    var numberOfSudokusToSolve = sudokuArray.length;

    squareCellToRowColumnMapper = sudoku.returnSquareCellToRowColumnMapper();
    candidates = sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
    certaintySudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);
    cellsRemainToSetAfterAddedNumbersWithCertainty = [];
    candidatesAfterAddedNumbersWithCertainty = sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
    bestSoFarSudokuBoard = sudoku.returnTwoDimensionalDataStructure(9, 9);

    while (currentSudokuToSolve <= numberOfSudokusToSolve && result === "S") {

        cellsRemainToSet.splice(0, cellsRemainToSet.length);
        cellsRemainToSetAfterAddedNumbersWithCertainty.splice(0, cellsRemainToSetAfterAddedNumbersWithCertainty.length);
        numberOfAttemptsToSolveSudoku = 0;
        numberOfCellsSetInBestSoFar = 0;
        sudokuSolved = false;
        numbersAddedWithCertaintyAndThenNoCandidates = false;
        saveStateDone = false;

        sudoku.getInputSudokuBoard(workingSudokuBoard, cellsRemainToSet, currentSudokuToSolve - 1, sudokuArray);
        numberOfCandidates[0] = sudoku.initCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);
        numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.length;

        while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates) {
            if (numberOfAttemptsToSolveSudoku > 0) {
                sudoku.restoreState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidates);
            }

            while (numberOfCandidates[0] > 0) {
                number = 0;
                i = 0;

                while (i < cellsRemainToSet.length && number === 0) {
                    row = cellsRemainToSet[i][0];
                    column = cellsRemainToSet[i][1];
                    number = sudoku.tryFindNumberToSetInCellWithCertainty(row, column, candidates, squareCellToRowColumnMapper);
                    i = (number === 0) ? i + 1 : i;
                }

                if (number === 0) {
                    sudoku.simulateOneNumber(candidates, cellsRemainToSet, indexNumber);
                    i = indexNumber[0];
                    number = indexNumber[1];
                    row = cellsRemainToSet[i][0];
                    column = cellsRemainToSet[i][1];

                    if (!saveStateDone) {
                        sudoku.saveState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty);
                        saveStateDone = true;
                    }
                }

                workingSudokuBoard[row - 1][column - 1] = number;
                cellsRemainToSet.splice(i, 1);
                numberOfCandidates[0] -= sudoku.updateCandidates(candidates, squareCellToRowColumnMapper, row, column, number);
            }

            if (cellsRemainToSet.length === 0) {
                sudokuSolved = true;
            }
            else if (!saveStateDone) {
                numbersAddedWithCertaintyAndThenNoCandidates = true;
                numberOfCellsSetInBestSoFar = 81 - cellsRemainToSet.Count;
            }
            else {
                numberOfCellsSetInBestSoFar = sudoku.checkIfCanUpdateBestSoFarSudokuBoard(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard);
                numberOfAttemptsToSolveSudoku++;
            }
        }

        result = sudoku.ProcessResult(currentSudokuToSolve - 1, sudokuArray, sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);

        if ((currentSudokuToSolve % 500) == 0) {
            console.log(currentSudokuToSolve);
        }

        currentSudokuToSolve++;
    }

    return result;
};

sudoku.sourceExists = function (source, isFile) {
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
};

sudoku.copyList = function(from, to) {
    to.splice(0, to.length);

    for (var i = 0; i < from.length; i++) {
        to.push(from[i]);
    }
};

sudoku.copySudokuBoard = function(sudokuBoardFrom, sudokuBoardTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
        }
    }
};

sudoku.copyCandidates = function(candidatesFrom, candidatesTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            for (var i = 0; i < 10; i++) {
                candidatesTo[row - 1][column - 1][i] = candidatesFrom[row - 1][column - 1][i];
            }
        }
    }
};

sudoku.saveState = function(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numOfCandidatesAfterAddedNumbersWithCert) {
    sudoku.copyList(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty);
    sudoku.copySudokuBoard(workingSudokuBoard, certaintySudokuBoard);
    sudoku.copyCandidates(candidates, candidatesAfterAddedNumbersWithCertainty);
    numOfCandidatesAfterAddedNumbersWithCert[0] = numberOfCandidates[0];
};

sudoku.restoreState = function(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numOfCandidatesAfterAddedNumWithCert, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numOfCandidates) {
    sudoku.copyList(cellsRemainToSetAfterAddedNumbersWithCertainty, cellsRemainToSet);
    sudoku.copySudokuBoard(certaintySudokuBoard, workingSudokuBoard);
    sudoku.copyCandidates(candidatesAfterAddedNumbersWithCertainty, candidates);
    numOfCandidates[0] = numOfCandidatesAfterAddedNumWithCert[0];
};

sudoku.getInputSudokuBoard = function(sudokuBoard, cellsRemainToSet, index, sudokuArray) {
    var rows, columns, sudokuBoardString;
    var row, column, n;

    sudokuBoardString = sudokuArray[index].replace("\r\n", "\n");

    rows = sudokuBoardString.split("\n");

    if (rows.length !== 9) {
        return "Number of rows in input file are not 9 as expected!";
    }

    for (row = 1; row <= 9; row++) {
        columns = rows[row - 1].split(' ');

        if (columns.length !== 9) {
            return "Number of columns in input file in row " + row + " are not 9 as expected!";
        }

        for (column = 1; column <= 9; column++) {
            n = Number(columns[column - 1]);

            if (isNaN(n) || Math.trunc(n) !== n) {
                return "The value \"" + columns[column - 1] + "\" in row " + row + " and column " + column + " in input file is not a valid integer!";
            }

            if (n < 0 || n > 9) {
                return "The value \"" + columns[column - 1] + "\" in row " + row + " and column " + column + " in input file is not an integer in the interval [0, 9] as expected!";
            }

            sudokuBoard[row - 1][column - 1] = n;

            if (n === 0) {
                cellsRemainToSet.push([row, column]);
            }
        }
    }

    return null;
};

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
                if (candidates[row - 1][column - 1][1 + j] === number) {
                    numberOfOccurenciesOfNumber++;

                    if (numberOfOccurenciesOfNumber > 1)
                        return false;
                }
            }
        }
    }

    return true;
};

sudoku.removeNumberIfItExists = function(v, number) {
    var i, n, index = -1, returnValue = 0;

    n = v[0];
    i = 1;
    while (i <= n && index === -1) {
        if (v[i] === number) {
            index = i;
            returnValue = 1;
        }
        else
            i++;
    }

    if (index !== -1) {
        while (index + 1 <= n) {
            v[index] = v[index + 1];
            index++;
        }

        v[0]--;
    }

    return returnValue;
};

sudoku.returnNumberOfOccurenciesOfNumber = function(sudokuBoard, squareCellToRowColumnMapper, number, t, target) { //t refers to a row, column or square
    var row = 0, column = 0, n = 0;

    for (var i = 0; i < 9; i++) {
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

        if (sudokuBoard[row - 1][column - 1] === number)
            n++;
    }

    return n;
};

sudoku.returnTwoDimensionalDataStructure = function(m, n) {
    var i, j, v = [];

    for (i = 0; i < m; i++)
        v.push([]);

    for (i = 0; i < m; i++)
        for (j = 0; j < n; j++)
            v[i].push(0);

    return v;
};

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
};

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
};

sudoku.returnSudokuBoardAsString = function(sudokuBoard) {
    var row, column, sb = "";

    for (row = 1; row <= 9; row++) {
        if (row > 1)
            sb += "\r\n";

        for (column = 1; column <= 9; column++) {
            if (column === 1)
                sb += sudokuBoard[row - 1][column - 1];
            else
                sb += (" " + sudokuBoard[row - 1][column - 1]);
        }
    }

    return sb;
};

sudoku.returnIntegerRandomNumber = function (minIncluded, maxExcluded) {
    var n = minIncluded + Math.round(Math.random() * (maxExcluded - minIncluded));

    if (n === maxExcluded)
        n = minIncluded;

    return n;
};

sudoku.simulateOneNumber = function(candidates, cellsRemainToSet, indexNumber) {
    var v = [], tmp, row, column, i, numberOfCandidates, minNumberOfCandidates = 9;

    for (i = 0; i < cellsRemainToSet.length; i++) {
        row = cellsRemainToSet[i][0];
        column = cellsRemainToSet[i][1];
        numberOfCandidates = candidates[row - 1][column - 1][0];

        if (numberOfCandidates > 0 && numberOfCandidates < minNumberOfCandidates)
            minNumberOfCandidates = numberOfCandidates;
    }

    for (i = 0; i < cellsRemainToSet.length; i++) {
        row = cellsRemainToSet[i][0];
        column = cellsRemainToSet[i][1];

        if (candidates[row - 1][column - 1][0] === minNumberOfCandidates)
            v.push(i);
    }

    tmp = sudoku.returnIntegerRandomNumber(0, v.length);
    indexNumber[0] = v[tmp];
    row = cellsRemainToSet[indexNumber[0]][0];
    column = cellsRemainToSet[indexNumber[0]][1];
    indexNumber[1] = candidates[row - 1][column - 1][1 + sudoku.returnIntegerRandomNumber(0, minNumberOfCandidates)];
};

sudoku.checkIfCanUpdateBestSoFarSudokuBoard = function(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard) {
    var retVal = numberOfCellsSetInBestSoFar; //Default

    if (numberOfCellsSetInBestSoFar < (81 - cellsRemainToSet.Count)) {
        retVal = 81 - cellsRemainToSet.Count;
        sudoku.copySudokuBoard(workingSudokuBoard, bestSoFarSudokuBoard);
    }

    return retVal;
};

sudoku.initCandidates = function(sudokuBoard, squareCellToRowColumnMapper, candidates) {
    var row, column, square, number, numberOfCandidates = 0, n;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);

            if (sudokuBoard[row - 1][column - 1] !== 0) {
                candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
            }
            else {
                n = 0;
                candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                for (number = 1; number <= 9; number++) {
                    if (
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, "row") === 0) &&
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, "column") === 0) &&
                        (sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, "square") === 0)
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
};

sudoku.tryFindNumberToSetInCellWithCertainty = function(row, column, candidates, squareCellToRowColumnMapper) {
    var i, square, numberOfCandidatesInCell, candidate, number = 0;

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
    numberOfCandidatesInCell = candidates[row - 1][column - 1][0];

    if (numberOfCandidatesInCell == 1)
        number = candidates[row - 1][column - 1][1];
    else {
        i = 1;
        while (i <= numberOfCandidatesInCell && number === 0) {
            candidate = candidates[row - 1][column - 1][i];

            if (sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, row, "row") ||
                sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, column, "column") ||
                sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, square, "square"))
                number = candidate;
            else
                i++;
        }
    }

    return number;
};

sudoku.updateCandidates = function(candidates, squareCellToRowColumnMapper, row, column, number) {
    var i, r, c, square, totalNumberOfCandidatesRemoved;

    totalNumberOfCandidatesRemoved = candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);

    for (c = 1; c <= 9; c++) {
        if (c !== column && candidates[row - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[row - 1][c - 1], number);
        }
    }

    for (r = 1; r <= 9; r++) {
        if (r !== row && candidates[r - 1][column - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[r - 1][column - 1], number);
        }
    }

    for (i = 0; i < 9; i++) {
        r = squareCellToRowColumnMapper[square - 1][i][0];
        c = squareCellToRowColumnMapper[square - 1][i][1];

        if (r !== row && c !== column && candidates[r - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += sudoku.removeNumberIfItExists(candidates[r - 1][c - 1], number);
        }
    }

    return totalNumberOfCandidatesRemoved;
};

sudoku.ProcessResult = function (index, sudokuArray, sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard) {
    var tmp1, tmp2, msg;

    if (sudokuSolved) {
        msg = "S";
    }
    else {
        tmp1 = numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard;
        tmp2 = 81 - numberOfCellsSetInBestSoFar;
        msg = "The sudoku was partially solved. " + tmp1 + " number(s) added to the original " + numberOfCellsSetInInputSudokuBoard + ". Unable to set " + tmp2 + " number(s).";
    }

    if (sudokuSolved || bestSoFarSudokuBoard === null) {
        sudokuArray[index] = sudoku.returnSudokuBoardAsString(workingSudokuBoard);
    }
    else {
        sudokuArray[index] = sudoku.returnSudokuBoardAsString(bestSoFarSudokuBoard);
    }

    return msg;
};


var d = new Date();

var yearStr = d.getFullYear().toString();
var month = 1 + d.getMonth();
var date = d.getDate();
var hour = d.getHours();
var minute = d.getMinutes();
var second = d.getSeconds();

var start = yearStr + "," + month + "," + date + "," + hour + "," + minute + "," + second + "\r\n";
var end, str, f, n, i;

var s = fs.readFileSync("C:\\Sudoku\\Solve\\StartSudokuBoards.txt", { encoding: 'ascii' });

var sudokuArray = s.split("\r\n-- New sudoku --\r\n");

var result = sudoku.run(sudokuArray);

if (result === "S") {
    d = new Date();
    yearStr = d.getFullYear().toString();
    month = 1 + d.getMonth();
    date = d.getDate();
    hour = d.getHours();
    minute = d.getMinutes();
    second = d.getSeconds();
    end = yearStr + "," + month + "," + date + "," + hour + "," + minute + "," + second;
    str = start + end;

    f = fs.openSync("C:\\Sudoku\\Solve\\StartEnd.txt", "w");
    fs.writeFileSync(f, str);
    fs.closeSync(f);

    //Clear contents in file
    f = fs.openSync("C:\\Sudoku\\Solve\\SolveResult.txt", "w");
    fs.writeFileSync(f, "");
    fs.closeSync(f);

    f = fs.openSync("C:\\Sudoku\\Solve\\SolveResult.txt", "a");
    n = sudokuArray.length;

    for (i = 0; i < n; i++) {
        fs.writeFileSync(f, sudokuArray[i]);

        if (i < (n - 1)) {
            fs.writeFileSync(f, "\r\n-- New sudoku --\r\n");
        }
    }

    fs.closeSync(f);
}
else {
    f = fs.openSync("C:\\Sudoku\\Solve\\Error.txt", "w");
    fs.writeFileSync(f, str);
    fs.closeSync(f);
}