
window.sudoku.debugReturnCandidates = function (v) {
    var n, i, str;

    str = "";

    n = v[0];

    if (n > 0) {
        for (i = 0; i < n; i++) {
            if (i === 0)
                str += v[1 + i].toString();
            else
                str += (", " + v[1 + i].toString());
        }
    }

    return str;
};

window.sudoku.debugReturnSudokuBoardRow = function (rowNr) {
    var column, str;

    str = "";

    for (column = 1; column <= 9; column++) {
        if (column === 1)
            str += window.sudoku.sudokuBoardTmp[rowNr - 1][column - 1].toString();
        else
            str += (", " + window.sudoku.sudokuBoardTmp[rowNr - 1][column - 1].toString());
    }

    return str;
};

window.sudoku.debug = function (r, c, candidate, minNumberOfCandidates, v) {
    var i, row, column, str1, str2;

    if (!window.sudoku.debugSimulationNr)
        window.sudoku.debugSimulationNr = 0;

    window.sudoku.debugSimulationNr++;

    str1 = "<div><h1>Simulation" + window.sudoku.debugSimulationNr.toString() + "(row, column, candidate)=(" + r.toString() + ", " + c.toString() + ", " + candidate.toString() + ")</h1>";
    str1 += "<h3>Sudoku board</h3>";
    str1 += "<ul>";

    for (row = 1; row <= 9; row++) {
        str1 += "<li>" + window.sudoku.debugReturnSudokuBoardRow(row) + "</li>";
    }

    str1 += "</ul>";

    str1 += "<h3>minNumberOfCandidates = " + minNumberOfCandidates.toString() + "</h3>";

    str1 += "<h3>Cells with minNumberOfCandidates</h3>";

    str1 += "<ul>";
    for (i = 0; i < v.length; i++) {
        str1 += "<li>[" + v[i][0].toString() + ", " + v[i][1].toString() + "]</li>";
    }
    str1 += "</ul>";

    str1 += "<ul>";

    str1 += "<h3>Candidates</h3>";

    for (row = 1; row <= 9; row++) {
        for (column = 1; column <= 9; column++) {
            str2 = window.sudoku.debugReturnCandidates(window.sudoku.candidates[row - 1][column - 1]);
            str1 += ("<li>" + row.toString() + column.toString() + ": " + str2 + "</li>");
        }
    }

    str1 += "</ul></div><hr /><br /><br />";

    $("#divDebug").append(str1);
};
