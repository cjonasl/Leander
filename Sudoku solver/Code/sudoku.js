window.sudoku = {};

window.sudoku.target = {
    ROW: "row",
    COLUMN: "column",
    SQUARE: "square"
};

window.sudoku.run = function (inputSudokuBoard) {
    var number, i;
    var certaintySudokuBoard = null;
    var bestSoFarSudokuBoard = null, workingSudokuBoard = window.sudoku.returnTwoDimensionalDataStructure(9, 9);
    var candidates, squareCellToRowColumnMapper, candidatesAfterAddedNumbersWithCertainty = null;
    var maxNumberOfAttemptsToSolveSudoku = 100, numberOfAttemptsToSolveSudoku = 0;
    var numberOfCellsSetInInputSudokuBoard = 0, numberOfCellsSetInBestSoFar = 0;
    var numberOfCandidates = [0];
    var numberOfCandidatesAfterAddedNumbersWithCertainty = [0];
    var sudokuSolved = false, numbersAddedWithCertaintyAndThenNoCandidates = false;
    var msg;
    var cellsRemainToSet = [], cellsRemainToSetAfterAddedNumbersWithCertainty = null;
    var indexNumber = [0, 0];

    msg = window.sudoku.getInputSudokuBoard(inputSudokuBoard, workingSudokuBoard, cellsRemainToSet);

    if (msg !== null) {
        return msg;
    }

    squareCellToRowColumnMapper = window.sudoku.returnSquareCellToRowColumnMapper();
    msg = window.sudoku.validateSudokuBoard(workingSudokuBoard, squareCellToRowColumnMapper);

    if (msg !== null) {
        return msg;
    }

    if (cellsRemainToSet.length === 0) {
        return "A complete sudoku was given as input. There is nothing to solve.";
    }

    candidates = window.sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
    numberOfCandidates[0] = window.sudoku.initCandidates(workingSudokuBoard, squareCellToRowColumnMapper, candidates);

    if (numberOfCandidates[0] === 0) {
        return "It is not possible to add any number to the sudoku.";
    }

    numberOfCellsSetInInputSudokuBoard = 81 - cellsRemainToSet.length;

    while (numberOfAttemptsToSolveSudoku < maxNumberOfAttemptsToSolveSudoku && !sudokuSolved && !numbersAddedWithCertaintyAndThenNoCandidates) {
        if (numberOfAttemptsToSolveSudoku > 0) {
            window.sudoku.restoreState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidates);
        }

        while (numberOfCandidates[0] > 0) {
            number = 0;
            i = 0;

            while (i < cellsRemainToSet.length && number === 0) {
                row = cellsRemainToSet[i][0];
                column = cellsRemainToSet[i][1];
                number = window.sudoku.tryFindNumberToSetInCellWithCertainty(row, column, candidates, squareCellToRowColumnMapper);
                i = (number === 0) ? i + 1 : i;
            }

            if (number === 0) {
                window.sudoku.simulateOneNumber(candidates, cellsRemainToSet, indexNumber);
                i = indexNumber[0];
                number = indexNumber[1];
                row = cellsRemainToSet[i][0];
                column = cellsRemainToSet[i][1];

                if (certaintySudokuBoard === null) {
                    certaintySudokuBoard = window.sudoku.returnTwoDimensionalDataStructure(9, 9);
                    cellsRemainToSetAfterAddedNumbersWithCertainty = [];
                    candidatesAfterAddedNumbersWithCertainty = window.sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
                    window.sudoku.saveState(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numberOfCandidatesAfterAddedNumbersWithCertainty);
                }
            }

            workingSudokuBoard[row - 1][column - 1] = number;
            cellsRemainToSet.splice(i, 1);
            numberOfCandidates[0] -= window.sudoku.updateCandidates(candidates, squareCellToRowColumnMapper, row, column, number);
        }

        if (cellsRemainToSet.length === 0) {
            sudokuSolved = true;
        }
        else if (certaintySudokuBoard === null) {
            numbersAddedWithCertaintyAndThenNoCandidates = true;
            numberOfCellsSetInBestSoFar = 81 - cellsRemainToSet.Count;
        }
        else {
            if (bestSoFarSudokuBoard === null)
                bestSoFarSudokuBoard = window.sudoku.returnTwoDimensionalDataStructure(9, 9);

            numberOfCellsSetInBestSoFar = window.sudoku.checkIfCanUpdateBestSoFarSudokuBoard(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard);
            numberOfAttemptsToSolveSudoku++;
        }
    }

    return window.sudoku.returnResult(sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard);
};

window.sudoku.copyList = function(from, to) {
    to.splice(0, to.length);

    for (var i = 0; i < from.length; i++) {
        to.push(from[i]);
    }
};

window.window.sudoku.copySudokuBoard = function(sudokuBoardFrom, sudokuBoardTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
        }
    }
};

window.sudoku.copyCandidates = function(candidatesFrom, candidatesTo) {
    for (var row = 1; row <= 9; row++) {
        for (var column = 1; column <= 9; column++) {
            for (var i = 0; i < 10; i++) {
                candidatesTo[row - 1][column - 1][i] = candidatesFrom[row - 1][column - 1][i];
            }
        }
    }
};

window.sudoku.saveState = function(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numberOfCandidates, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numOfCandidatesAfterAddedNumbersWithCert) {
    window.sudoku.copyList(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty);
    window.sudoku.copySudokuBoard(workingSudokuBoard, certaintySudokuBoard);
    window.sudoku.copyCandidates(candidates, candidatesAfterAddedNumbersWithCertainty);
    numOfCandidatesAfterAddedNumbersWithCert[0] = numberOfCandidates[0];
};

window.sudoku.restoreState = function(cellsRemainToSet, cellsRemainToSetAfterAddedNumbersWithCertainty, numOfCandidatesAfterAddedNumWithCert, workingSudokuBoard, certaintySudokuBoard, candidates, candidatesAfterAddedNumbersWithCertainty, numOfCandidates) {
    window.sudoku.copyList(cellsRemainToSetAfterAddedNumbersWithCertainty, cellsRemainToSet);
    window.sudoku.copySudokuBoard(certaintySudokuBoard, workingSudokuBoard);
    window.sudoku.copyCandidates(candidatesAfterAddedNumbersWithCertainty, candidates);
    numOfCandidates[0] = numOfCandidatesAfterAddedNumWithCert[0];
};

window.sudoku.getInputSudokuBoard = function (inputSudokuBoard, sudokuBoard, cellsRemainToSet) {
    var rows, columns, row, column, n;

    rows = inputSudokuBoard.split("\r\n");

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

window.sudoku.candidateIsAlonePossible = function(number, candidates, squareCellToRowColumnMapper, t, target) {
    var row = 0, column = 0, n, i, j, numberOfOccurenciesOfNumber = 0;

    for (i = 0; i < 9; i++) {
        switch (target) {
            case window.sudoku.target.ROW:
                row = t;
                column = i + 1;
                break;
            case window.sudoku.target.COLUMN:
                row = i + 1;
                column = t;
                break;
            case window.sudoku.target.SQUARE:
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

window.sudoku.removeNumberIfItExists = function(v, number) {
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

window.sudoku.returnNumberOfOccurenciesOfNumber = function(sudokuBoard, squareCellToRowColumnMapper, number, t, target) { //t refers to a row, column or square
    var row = 0, column = 0, n = 0;

    for (var i = 0; i < 9; i++) {
        switch (target) {
            case window.sudoku.target.ROW:
                row = t;
                column = i + 1;
                break;
            case window.sudoku.target.COLUMN:
                row = i + 1;
                column = t;
                break;
            case window.sudoku.target.SQUARE:
                row = squareCellToRowColumnMapper[t - 1][i][0];
                column = squareCellToRowColumnMapper[t - 1][i][1];
                break;
        }

        if (sudokuBoard[row - 1][column - 1] === number)
            n++;
    }

    return n;
};

window.sudoku.returnTwoDimensionalDataStructure = function(m, n) {
    var i, j, v = [];

    for (i = 0; i < m; i++)
        v.push([]);

    for (i = 0; i < m; i++)
        for (j = 0; j < n; j++)
            v[i].push(0);

    return v;
};

window.sudoku.returnThreeDimensionalDataStructure = function(l, m, n) {
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

window.sudoku.returnSquareCellToRowColumnMapper = function() {
    var v, index, i, row, column, square;

    v = window.sudoku.returnThreeDimensionalDataStructure(9, 9, 2);

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

window.sudoku.returnSudokuBoardAsString = function(sudokuBoard) {
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

window.sudoku.returnIntegerRandomNumber = function (minIncluded, maxExcluded) {
    var n = minIncluded + Math.round(Math.random() * (maxExcluded - minIncluded));

    if (n === maxExcluded)
        n = minIncluded;

    return n;
};

window.sudoku.simulateOneNumber = function(candidates, cellsRemainToSet, indexNumber) {
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

    tmp = window.sudoku.returnIntegerRandomNumber(0, v.length);
    indexNumber[0] = v[tmp];
    row = cellsRemainToSet[indexNumber[0]][0];
    column = cellsRemainToSet[indexNumber[0]][1];
    indexNumber[1] = candidates[row - 1][column - 1][1 + window.sudoku.returnIntegerRandomNumber(0, minNumberOfCandidates)];
};

window.sudoku.checkIfCanUpdateBestSoFarSudokuBoard = function(numberOfCellsSetInBestSoFar, cellsRemainToSet, workingSudokuBoard, bestSoFarSudokuBoard) {
    var retVal = numberOfCellsSetInBestSoFar; //Default

    if (numberOfCellsSetInBestSoFar < (81 - cellsRemainToSet.Count)) {
        retVal = 81 - cellsRemainToSet.Count;
        window.sudoku.copySudokuBoard(workingSudokuBoard, bestSoFarSudokuBoard);
    }

    return retVal;
};

window.sudoku.initCandidates = function(sudokuBoard, squareCellToRowColumnMapper, candidates) {
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
                        (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, "row") === 0) &&
                        (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, "column") === 0) &&
                        (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, "square") === 0)
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

window.sudoku.tryFindNumberToSetInCellWithCertainty = function(row, column, candidates, squareCellToRowColumnMapper) {
    var i, square, numberOfCandidatesInCell, candidate, number = 0;

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
    numberOfCandidatesInCell = candidates[row - 1][column - 1][0];

    if (numberOfCandidatesInCell == 1)
        number = candidates[row - 1][column - 1][1];
    else {
        i = 1;
        while (i <= numberOfCandidatesInCell && number === 0) {
            candidate = candidates[row - 1][column - 1][i];

            if (window.sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, row, "row") ||
                window.sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, column, "column") ||
                window.sudoku.candidateIsAlonePossible(candidate, candidates, squareCellToRowColumnMapper, square, "square"))
                number = candidate;
            else
                i++;
        }
    }

    return number;
};

window.sudoku.updateCandidates = function(candidates, squareCellToRowColumnMapper, row, column, number) {
    var i, r, c, square, totalNumberOfCandidatesRemoved;

    totalNumberOfCandidatesRemoved = candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
    candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

    square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);

    for (c = 1; c <= 9; c++) {
        if (c !== column && candidates[row - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += window.sudoku.removeNumberIfItExists(candidates[row - 1][c - 1], number);
        }
    }

    for (r = 1; r <= 9; r++) {
        if (r !== row && candidates[r - 1][column - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += window.sudoku.removeNumberIfItExists(candidates[r - 1][column - 1], number);
        }
    }

    for (i = 0; i < 9; i++) {
        r = squareCellToRowColumnMapper[square - 1][i][0];
        c = squareCellToRowColumnMapper[square - 1][i][1];

        if (r !== row && c !== column && candidates[r - 1][c - 1][0] > 0) {
            totalNumberOfCandidatesRemoved += window.sudoku.removeNumberIfItExists(candidates[r - 1][c - 1], number);
        }
    }

    return totalNumberOfCandidatesRemoved;
};

window.sudoku.validateSudokuBoard = function(sudokuBoard, squareCellToRowColumnMapper) {
    var row, column, square, number;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = 1 + (3 * Math.trunc((row - 1) / 3)) + Math.trunc((column - 1) / 3);
            number = sudokuBoard[row - 1][column - 1];

            if (number !== 0) {
                if (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, row, "row") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in row " + row;
                }
                else if (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, column, "column") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in column " + column;
                }
                else if (window.sudoku.returnNumberOfOccurenciesOfNumber(sudokuBoard, squareCellToRowColumnMapper, number, square, "square") > 1) {
                    return "The input sudoku is incorrect! The number " + number + " occurs more than once in square " + square;
                }
            }
        }
    }

    return null;
};

window.sudoku.processResult = function (sudokuBoardFinalResult) {
    var row, column, element;

    $("input[data-startinteger='1']").addClass("startInteger");

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            element = $("#cell" + row.toString() + column.toString());
            if (sudokuBoardFinalResult[row - 1][column - 1] !== 0 && !element.hasClass("startInteger"))
                element.val(sudokuBoardFinalResult[row - 1][column - 1].toString());
        }
    }

    $("#divSudokuSolver input[type='text']").prop("readonly", true);
    $("#solveButton").prop("disabled", true);
    $("#solveButton").addClass("greyFontColor");
    $("#newSudokuButton").prop("disabled", false);
    $("#newSudokuButton").removeClass("greyFontColor");
};

window.sudoku.returnResult = function(sudokuSolved, numberOfCellsSetInInputSudokuBoard, numberOfCellsSetInBestSoFar, workingSudokuBoard, bestSoFarSudokuBoard) {
    var msg, tmp1, tmp2;

    if (sudokuSolved) {
        tmp1 = 81 - numberOfCellsSetInInputSudokuBoard;
        msg = "The sudoku was solved. " + tmp1 + " number(s) added to the original " + numberOfCellsSetInInputSudokuBoard + ".";
    }
    else {
        tmp1 = numberOfCellsSetInBestSoFar - numberOfCellsSetInInputSudokuBoard;
        tmp2 = 81 - numberOfCellsSetInBestSoFar;
        msg = "The sudoku was partially solved. " + tmp1 + " number(s) added to the original " + numberOfCellsSetInInputSudokuBoard + ". Unable to set " + tmp2 + " number(s).";
    }

    if (sudokuSolved || bestSoFarSudokuBoard === null) {
        window.sudoku.processResult(workingSudokuBoard);
    }
    else {
        window.sudoku.processResult(bestSoFarSudokuBoard);
    }

    return msg;
};

window.sudoku.makeSudokuBoard = function () {
    var element, r, c, oc, or, l, t, str1, str2, sudokuSquareWidth = 42, offset = 5;

    element = $("#divSudokuSolver");

    for (r = 1; r <= 9; r++) {
        for (c = 1; c <= 9; c++) {
            if (c < 4) {
                oc = 0;
            }
            else if (c >= 4 && c < 7) {
                oc = offset;
            }
            else {
                oc = 2 * offset;
            }

            if (r < 4) {
                or = 0;
            }
            else if (r >= 4 && r < 7) {
                or = offset;
            }
            else {
                or = 2 * offset;
            }

            l = sudokuSquareWidth * (c - 1) + 120 + oc;
            t = sudokuSquareWidth * (r - 1) + 170 + or;
            str1 = l.toString();
            str2 = t.toString();
            str3 = "<input type='text' id='cell" + r.toString() + c.toString() + "' class='sudokuCells' style='left: " + str1 + "px; top: " + str2 + "px;' />";
            element.append(str3);
        }
    }
};

window.sudoku.readInputIntegers = function () {
    var r, c, n, str, element, inputSudokuBoard, errorMessage;

    errorMessage = null;
    inputSudokuBoard = "";
    r = 1;

    while (r <= 9 && !errorMessage) {
        c = 1;
        while (c <= 9 && !errorMessage) {
            element = $("#cell" + r.toString() + c.toString());
            str = element.val().trim();

            if (str === "") {
                if (r < 9 || c < 9) {
                    if (c === 9)
                        inputSudokuBoard += "0\r\n";
                    else
                        inputSudokuBoard += "0 ";
                }
                else {
                    inputSudokuBoard += "0";
                }
            }             
            else if (window.isNaN(str))
                errorMessage = "The value given in row " + r.toString() + " and column " + c.toString() + " is not an integer!";
            else {
                n = window.parseFloat(str);

                if (n < 1 || n > 9 || Math.ceil(n) !== n)
                    errorMessage = "The value given in row " + r.toString() + " and column " + c.toString() + " is not an integer between 1 and 9!";
                else {
                    element.attr("data-startinteger", "1");

                    if (r < 9 || c < 9) {
                        if (c === 9)
                            inputSudokuBoard += "0\r\n";
                        else
                            inputSudokuBoard += (str + " ");
                    }
                    else {
                        inputSudokuBoard += str;
                    }
                }
            }
            c++;
        }
        r++;
    }

    return { errorMsg: errorMessage, inputSudokuBrd: inputSudokuBoard };
};

window.sudoku.newSudoku = function () {
    $("input[data-startinteger='1']").removeClass("startInteger");
    $("input[data-startinteger='1']").removeAttr("data-startinteger");
    $("#divSudokuSolver input[type='text']").prop("readonly", false);
    $("#divSudokuSolver input[type='text']").val("");
    $("#solveButton").prop("disabled", false);
    $("#solveButton").removeClass("greyFontColor");
    $("#newSudokuButton").prop("disabled", true);
    $("#newSudokuButton").addClass("greyFontColor");
};

window.sudoku.solveSudoku = function () {
    var obj, result;

    $("input[data-startinteger='1']").removeAttr("data-startinteger");

    obj = window.sudoku.readInputIntegers();

    if (obj.errorMsg) {
        alert(obj.errorMsg);
    }
    else {
        result = window.sudoku.run(obj.inputSudokuBrd);
        alert(result);
    }
};
