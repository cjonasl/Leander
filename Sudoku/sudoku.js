window.sudoku = new window.Object();

window.sudoku.makeSudokuBoard = function () {
    var element, r, c, oc, or, l, t, str1, str2, sudokuSquareWidth = 40, offset = 3;

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

            l = sudokuSquareWidth * (c - 1) + 40 + oc;
            t = sudokuSquareWidth * (r - 1) + 170 + or;
            str1 = l.toString();
            str2 = t.toString();
            str3 = "<input type='text' id='cell" + r.toString() + c.toString() + "' class='sudokuCells' style='left: " + str1 + "px; top: " + str2 + "px;' />";
            element.append(str3);
        }
    }
};

window.sudoku.returnIntegerRandomNumber = function (minIncluded, maxExcluded) {
    var n = minIncluded + Math.round(Math.random() * (maxExcluded - minIncluded));

    if (n === maxExcluded) 
        n = minIncluded;

    return n;
};

window.sudoku.integerDivision = function (a, b) {
    var n = 1;

    while (n * b <= a) 
        n++;
 
    return n - 1;
};

window.sudoku.returnSquare = function (row, column) {
    return 1 + 3 * window.sudoku.integerDivision(row - 1, 3) + window.sudoku.integerDivision(column - 1, 3);
};

window.sudoku.returnTwoDimensionalDataStructure = function (m, n) {
    var i, j, v = [];

    for (i = 0; i < m; i++)
        v.push([]);

    for (i = 0; i < m; i++)
        for (j = 0; j < n; j++)
            v[i].push(0);
 
    return v;
};

window.sudoku.returnThreeDimensionalDataStructure = function (l, m, n) {
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

window.sudoku.returnSquareToCellMapper = function () {
    var v, index, i, row, column, square;

    v = window.sudoku.returnThreeDimensionalDataStructure(9, 9, 2);

    index = [0, 0, 0, 0, 0, 0, 0, 0, 0];

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = window.sudoku.returnSquare(row, column);
            v[square - 1][index[square - 1]][0] = row;
            v[square - 1][index[square - 1]][1] = column;
            index[square - 1]++;
        }
    }

    return v;
};

window.sudoku.returnNumberOfOccurenciesOfNumberInRow = function (sudokuBoard, row, number) {
    var column, n = 0;

    for (column = 1; column <= 9; column++) {
        if (sudokuBoard[row - 1][column - 1] === number)
            n++;
    }

    return n;
};

window.sudoku.returnNumberOfOccurenciesOfNumberInColumn = function(sudokuBoard, column, number) {
    var row, n = 0;

    for (row = 1; row <= 9; row++) {
        if (sudokuBoard[row - 1][column - 1] === number)
            n++;
    }

    return n;
};

window.sudoku.returnNumberOfOccurenciesOfNumberInSquare = function(sudokuBoard, square, number) {
    var row, column, i, n = 0;

    for (i = 0; i < 9; i++) {
        row = window.sudoku.squareToCellMapper[square - 1][i][0];
        column = window.sudoku.squareToCellMapper[square - 1][i][1];

        if (sudokuBoard[row - 1][column - 1] === number)
            n++;
    }

    return n;
};

window.sudoku.validateSudokuRule = function (sudokuBoard) {
    var row, column, square, number;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = window.sudoku.returnSquare(row, column);
            number = sudokuBoard[row - 1][column - 1];

            if (number) {
                if (window.sudoku.returnNumberOfOccurenciesOfNumberInRow(sudokuBoard, row, number) > 1)
                    return "The number " + number + " occurs more than once in row " + row;
                else if (window.sudoku.returnNumberOfOccurenciesOfNumberInColumn(sudokuBoard, column, number) > 1)
                    return "The number " + number + " occurs more than once in column " + column;
                else if (window.sudoku.returnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, square, number) > 1)
                    return "The number " + number + " occurs more than once in square " + square;
            }
        }
    }

    return null;
};

window.sudoku.calculateCandidateStructure = function (sudokuBoard) {
    var row, column, square, number, n;

    window.sudoku.cellsRemainToSet = [];
    window.sudoku.numberOfCandidates = 0;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            square = window.sudoku.returnSquare(row, column);

            if (sudokuBoard[row - 1][column - 1] !== 0) {
                window.sudoku.candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already
            }
            else {
                window.sudoku.cellsRemainToSet.push([row, column]);
                n = 0;
                window.sudoku.candidates[row - 1][column - 1][0] = 0; //Number of candidates is set in index 0

                for (number = 1; number <= 9; number++) {
                    if (
                        window.sudoku.returnNumberOfOccurenciesOfNumberInRow(sudokuBoard, row, number) === 0 &&
                        window.sudoku.returnNumberOfOccurenciesOfNumberInColumn(sudokuBoard, column, number) === 0 &&
                        window.sudoku.returnNumberOfOccurenciesOfNumberInSquare(sudokuBoard, square, number) === 0
                    ) {
                        n++;
                        window.sudoku.candidates[row - 1][column - 1][0] = n;
                        window.sudoku.candidates[row - 1][column - 1][n] = number;
                        window.sudoku.numberOfCandidates++;
                    }
                }
            }
        }
    }
};

window.sudoku.removeCandidateIfItExists = function (v, number) {
    var i, n, index = -1;

    n = v[0];
    i = 1;
    while (i <= n && index === -1) {
        if (v[i] === number)
            index = i;
        else
            i++;
    }

    if (index !== -1) {
        while (index + 1 <= n) {
            v[index] = v[index + 1];
            index++;
        }

        v[0]--;

        window.sudoku.numberOfCandidates--;
    }
};

window.sudoku.numberIsAloneCandidateInRow = function (row, number) {
    var column, n, i, numberOfOccurenciesOfNumber = 0;

    for (column = 1; column <= 9; column++) {
        n = window.sudoku.candidates[row - 1][column - 1][0];

        if (n > 0) {
            for (i = 1; i <= n; i++) {
                if (window.sudoku.candidates[row - 1][column - 1][i] === number) {
                    numberOfOccurenciesOfNumber++;

                    if (numberOfOccurenciesOfNumber > 1)
                        return false;
                }
            }
        }
    }

    return true;
};

window.sudoku.numberIsAloneCandidateInColumn = function (column, number) {
    var row, n, i, numberOfOccurenciesOfNumber = 0;

    for (row = 1; row <= 9; row++) {
        n = window.sudoku.candidates[row - 1][column - 1][0];

        if (n > 0) {
            for (i = 1; i <= n; i++) {
                if (window.sudoku.candidates[row - 1][column - 1][i] === number) {
                    numberOfOccurenciesOfNumber++;

                    if (numberOfOccurenciesOfNumber > 1)
                        return false;
                }
            }
        }
    }

    return true;
};

window.sudoku.numberIsAloneCandidateInSquare = function(square, number) {
    var row, column, n, i, j, numberOfOccurenciesOfNumber = 0;

    for (i = 0; i < 9; i++) {
        row = window.sudoku.squareToCellMapper[square - 1][i][0];
        column = window.sudoku.squareToCellMapper[square - 1][i][1];
        n = window.sudoku.candidates[row - 1][column - 1][0];

        if (n > 0) {
            for (j = 1; j <= n; j++) {
                if (window.sudoku.candidates[row - 1][column - 1][j] === number) {
                    numberOfOccurenciesOfNumber++;

                    if (numberOfOccurenciesOfNumber > 1)
                        return false;

                    break;
                }
            }
        }
    }

    return true;
};

window.sudoku.removeCandidateInCurrentRowColumnSquare = function (row, column, candidate) {
    var i, r, c, square;

    square = window.sudoku.returnSquare(row, column);

    for (i = 1; i <= 9; i++) {
        if (i !== column && window.sudoku.candidates[row - 1][i - 1][0] > 0) {
            window.sudoku.removeCandidateIfItExists(window.sudoku.candidates[row - 1][i - 1], candidate);
        }
    }

    for (i = 1; i <= 9; i++) {
        if (i !== row && window.sudoku.candidates[i - 1][column - 1][0] > 0) {
            window.sudoku.removeCandidateIfItExists(window.sudoku.candidates[i - 1][column - 1], candidate);
        }
    }

    for (i = 0; i < 9; i++) {
        r = window.sudoku.squareToCellMapper[square - 1][i][0];
        c = window.sudoku.squareToCellMapper[square - 1][i][1];

        if (r !== row && c !== column && window.sudoku.candidates[r - 1][c - 1][0] > 0) {
            window.sudoku.removeCandidateIfItExists(window.sudoku.candidates[r - 1][c - 1], candidate);
        }
    }
};

window.sudoku.setNewCellAndUpdateStructure = function (sudokuBoard, row, column, candidate, index) {
    var i, r, c;

    window.sudoku.removeCandidateInCurrentRowColumnSquare(row, column, candidate);
    sudokuBoard[row - 1][column - 1] = candidate;
    window.sudoku.numberOfCandidates -= window.sudoku.candidates[row - 1][column - 1][0]; //Remove all candidates in that cell
    window.sudoku.candidates[row - 1][column - 1][0] = -1; //Indicates that the cell is set already

    i = 0;
    while (index === -1) //index == -1 if candidate was simulated, otherwise not
    {
        r = window.sudoku.cellsRemainToSet[i][0];
        c = window.sudoku.cellsRemainToSet[i][1];

        if (r === row && c === column)
            index = i;

        i++;
    }

    window.sudoku.cellsRemainToSet.splice(index, 1);
};

window.sudoku.simulateOneCandidate = function () {
    var v, r, c, index, minNumberOfCandidates, row, column, candidate;

    v = [];
    minNumberOfCandidates = 9;

    for (r = 1; r <= 9; r++) {
        for (c = 1; c <= 9; c++) {
            if (window.sudoku.candidates[r - 1][c - 1][0] > 0 && window.sudoku.candidates[r - 1][c - 1][0] < minNumberOfCandidates)
                minNumberOfCandidates = window.sudoku.candidates[r - 1][c - 1][0];
        }
    }

    for (r = 1; r <= 9; r++) {
        for (c = 1; c <= 9; c++) {
            if (window.sudoku.candidates[r - 1][c - 1][0] === minNumberOfCandidates)
                v.push([r, c]);
        }
    }

    index = window.sudoku.returnIntegerRandomNumber(0, v.length);
    row = v[index][0];
    column = v[index][1];
    index = window.sudoku.returnIntegerRandomNumber(0, minNumberOfCandidates);
    candidate = window.sudoku.candidates[row - 1][column - 1][1 + index];

    return {
        row: row,
        column: column,
        candidate: candidate
    };
};

window.sudoku.copySudokuBoard = function (sudokuBoardFrom, sudokuBoardTo) {
    var row, column;

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            sudokuBoardTo[row - 1][column - 1] = sudokuBoardFrom[row - 1][column - 1];
        }
    }
};

window.sudoku.initSudoku = function () {
    window.sudoku.maxNumberOfSimulations = 10;
    window.sudoku.numberOfCandidates = 0;
    window.sudoku.squareToCellMapper = window.sudoku.returnSquareToCellMapper();
    window.sudoku.candidates = window.sudoku.returnThreeDimensionalDataStructure(9, 9, 10);
    window.sudoku.sudokuBoardCertainty = window.sudoku.returnTwoDimensionalDataStructure(9, 9);
    window.sudoku.sudokuBoardTmp = window.sudoku.returnTwoDimensionalDataStructure(9, 9);
    window.sudoku.sudokuBoardBestSoFar = window.sudoku.returnTwoDimensionalDataStructure(9, 9);
    window.sudoku.cellsRemainToSet = [];
};

window.sudoku.readInputIntegers = function () {
    var r, c, n, str, element, errorMessage;

    errorMessage = null;
    r = 1;
    while (r <= 9 && !errorMessage) {
        c = 1;
        while (c <= 9 && !errorMessage) {
            element = $("#cell" + r.toString() + c.toString());
            str = element.val().trim();

            if (str === "") 
                window.sudoku.sudokuBoardCertainty[r - 1][c - 1] = 0;   
            else if (str !== "" && window.isNaN(str))
                errorMessage = "The value given in row " + r.toString() + " and column " + c.toString() + " is not an integer!";     
            else {
                n = window.parseFloat(str);

                if (n < 1 || n > 9 || Math.ceil(n) !== n) 
                    errorMessage = "The value given in row " + r.toString() + " and column " + c.toString() + " is not an integer between 1 and 9!";              
                else {
                    window.sudoku.sudokuBoardCertainty[r - 1][c - 1] = n; 
                    element.attr("data-startinteger", "1");
                }
            }
            c++;
        }
        r++;
    }

    return errorMessage;
};

window.sudoku.showResult = function(sudokuBoardFinalResult, message) {
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

    alert(message);
};

window.sudoku.canSetCell = function (row, column) {
    var i, square, numberOfCandidates;

    candidate = 0;
    square = window.sudoku.returnSquare(row, column);
    numberOfCandidates = window.sudoku.candidates[row - 1][column - 1][0];

    if (numberOfCandidates === 1) {
        candidate = window.sudoku.candidates[row - 1][column - 1][1]; //Alone candidate in cell
    }
    else if (numberOfCandidates > 1) {
        i = 0;

        while (i < numberOfCandidates && candidate === 0) {
            if (window.sudoku.numberIsAloneCandidateInRow(row, window.sudoku.candidates[row - 1][column - 1][1 + i]))
                candidate = window.sudoku.candidates[row - 1][column - 1][1 + i];
            else if (window.sudoku.numberIsAloneCandidateInColumn(column, window.sudoku.candidates[row - 1][column - 1][1 + i]))
                candidate = window.sudoku.candidates[row - 1][column - 1][1 + i];
            else if (window.sudoku.numberIsAloneCandidateInSquare(square, window.sudoku.candidates[row - 1][column - 1][1 + i]))
                candidate = window.sudoku.candidates[row - 1][column - 1][1 + i];
            else
                i++;
        }
    }

    if (candidate !== 0)
        return { canSetCell: true, candidate: candidate };
    else
        return { canSetCell: false, candidate: 0 };
};

window.sudoku.solveSudoku = function () {
    var numberOfCellsSetInInputSudokuBoard, numberOfAddedNumbers, numberOfSimulations, numberOfCellsSetInFinalSudoku, index, obj, errorMessage;
    var i, row, column, square, candidate;

    $("input[data-startinteger='1']").removeClass("startInteger");
    $("input[data-startinteger='1']").removeAttr("data-startinteger");

    errorMessage = window.sudoku.readInputIntegers();

    if (errorMessage) {
        alert(errorMessage);
        return;
    }

    errorMessage = window.sudoku.validateSudokuRule(window.sudoku.sudokuBoardCertainty);

    if (errorMessage) {
        alert(errorMessage);
        return;
    }

    window.sudoku.calculateCandidateStructure(window.sudoku.sudokuBoardCertainty);
    numberOfCellsSetInInputSudokuBoard = 81 - window.sudoku.cellsRemainToSet.length;
    numberOfSimulations = 0;
    numberOfCellsSetInFinalSudoku = 0;
    index = 0;

    if (window.sudoku.cellsRemainToSet.length === 0) {
        alert("The sudoku is solved already.");
        return;
    }
    else if (window.sudoku.numberOfCandidates === 0) {
        alert("Not possible to add any number to the sudoku.");
        return;
    }

    while (window.sudoku.cellsRemainToSet.length > 0) {
        if (index === window.sudoku.cellsRemainToSet.length) {
            if (numberOfSimulations === 0 && window.sudoku.numberOfCandidates === 0) {
                numberOfCellsSetInFinalSudoku = 81 - window.sudoku.cellsRemainToSet.length;
                window.sudoku.copySudokuBoard(window.sudoku.sudokuBoardCertainty, window.sudoku.sudokuBoardBestSoFar);
                break;
            }

            if (numberOfSimulations === 0) {
                window.sudoku.copySudokuBoard(window.sudoku.sudokuBoardCertainty, window.sudoku.sudokuBoardTmp);
                numberOfSimulations++;
            }

            if (window.sudoku.numberOfCandidates > 0) {
                obj = window.sudoku.simulateOneCandidate();
                window.sudoku.setNewCellAndUpdateStructure(window.sudoku.sudokuBoardTmp, obj.row, obj.column, obj.candidate, -1);
            }
            else {
                if ((81 - window.sudoku.cellsRemainToSet.length) > numberOfCellsSetInFinalSudoku) {
                    numberOfCellsSetInFinalSudoku = 81 - window.sudoku.cellsRemainToSet.length;
                    window.sudoku.copySudokuBoard(window.sudoku.sudokuBoardTmp, window.sudoku.sudokuBoardBestSoFar);
                }

                window.sudoku.copySudokuBoard(window.sudoku.sudokuBoardCertainty, window.sudoku.sudokuBoardTmp);
                window.sudoku.calculateCandidateStructure(window.sudoku.sudokuBoardTmp);
                numberOfSimulations++;
            }

            if (numberOfSimulations > window.sudoku.maxNumberOfSimulations)
                break;

            index = 0;
        }

        row = window.sudoku.cellsRemainToSet[index][0];
        column = window.sudoku.cellsRemainToSet[index][1];
        obj = window.sudoku.canSetCell(row, column);

        if (obj.canSetCell) {
            if (numberOfSimulations === 0) 
                window.sudoku.setNewCellAndUpdateStructure(window.sudoku.sudokuBoardCertainty, row, column, obj.candidate, index);         
            else 
                window.sudoku.setNewCellAndUpdateStructure(window.sudoku.sudokuBoardTmp, row, column, obj.candidate, index);
            
            index = 0;
        }
        else
            index++;
    }

    if (window.sudoku.cellsRemainToSet.length === 0) {
        numberOfAddedNumbers = 81 - numberOfCellsSetInInputSudokuBoard;
        message = "Sudoku solved. Numbers added = " + numberOfAddedNumbers.toString();
    }
    else {
        numberOfAddedNumbers = numberOfCellsSetInFinalSudoku - numberOfCellsSetInInputSudokuBoard;
        message = "Sudoku partially solved. Numbers added = " + numberOfAddedNumbers.toString();
    }

    if (window.sudoku.cellsRemainToSet.length === 0 && numberOfSimulations === 0) //Sudoku solved in first try
        window.sudoku.showResult(window.sudoku.sudokuBoardCertainty, message);
    else if (window.sudoku.cellsRemainToSet.length === 0 && numberOfSimulations > 0) //Sudoku after simulation
        window.sudoku.showResult(window.sudoku.sudokuBoardTmp, message);
    else
        window.sudoku.showResult(window.sudoku.sudokuBoardBestSoFar, message); //Sudoku only partially solved
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