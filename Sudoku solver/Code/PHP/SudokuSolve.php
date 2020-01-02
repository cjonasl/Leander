<?php

class Target {
    const ROW = 1;
    const COLUMN = 2;
    const SQUARE = 3;
}

function run(&$sudokuArray) {
    $certaintySudokuBoard = null;
    $bestSoFarSudokuBoard = null;
    $workingSudokuBoard = returnTwoDimensionalDataStructure(9, 9);
    $candidatesAfterAddedNumbersWithCertainty = null;
    $numberOfAttemptsToSolveSudoku = 0;
    $maxNumberOfAttemptsToSolveSudoku = 1000;
    $numberOfCellsSetInInputSudokuBoard = 0;
    $numberOfCellsSetInBestSoFar = 0;
    $sudokuSolved = false;
    $numbersAddedWithCertaintyAndThenNoCandidates = false;
    $cellsRemainToSetAfterAddedNumbersWithCertainty = null;
    $cellsRemainToSet = [];
    $result = "S";
    $saveStateDone = false;
    $currentSudokuToSolve = 1;
    $numberOfSudokusToSolve = count($sudokuArray);

    $squareCellToRowColumnMapper = returnSquareCellToRowColumnMapper();
    $candidates = returnThreeDimensionalDataStructure(9, 9, 10);
    $certaintySudokuBoard = returnTwoDimensionalDataStructure(9, 9);
    $cellsRemainToSetAfterAddedNumbersWithCertainty = [];
    $candidatesAfterAddedNumbersWithCertainty = returnThreeDimensionalDataStructure(9, 9, 10);
    $bestSoFarSudokuBoard = returnTwoDimensionalDataStructure(9, 9);

    while ($currentSudokuToSolve <= $numberOfSudokusToSolve && $result == "S") {
        array_splice($cellsRemainToSet, 0, count($cellsRemainToSet));
        array_splice($cellsRemainToSetAfterAddedNumbersWithCertainty, 0, count($cellsRemainToSetAfterAddedNumbersWithCertainty));
        $numberOfAttemptsToSolveSudoku = 0;
        $numberOfCellsSetInBestSoFar = 0;
        $sudokuSolved = false;
        $numbersAddedWithCertaintyAndThenNoCandidates = false;
        $saveStateDone = false;

        getInputSudokuBoard($workingSudokuBoard, $cellsRemainToSet, $currentSudokuToSolve - 1, $sudokuArray);
        $numberOfCandidates = initCandidates($workingSudokuBoard, $squareCellToRowColumnMapper, $candidates);
        $numberOfCellsSetInInputSudokuBoard = 81 - count($cellsRemainToSet);

        while ($numberOfAttemptsToSolveSudoku < $maxNumberOfAttemptsToSolveSudoku && !$sudokuSolved && !$numbersAddedWithCertaintyAndThenNoCandidates) {
            if ($numberOfAttemptsToSolveSudoku > 0) {
                restoreState($cellsRemainToSet, $cellsRemainToSetAfterAddedNumbersWithCertainty, $numberOfCandidatesAfterAddedNumbersWithCertainty, $workingSudokuBoard, $certaintySudokuBoard, $candidates, $candidatesAfterAddedNumbersWithCertainty, $numberOfCandidates);
            }

            while ($numberOfCandidates > 0) {
                $number = 0;
                $i = 0;

                while ($i < count($cellsRemainToSet) && $number == 0) {
                    $row = $cellsRemainToSet[$i][0];
                    $column = $cellsRemainToSet[$i][1];
                    $number = tryFindNumberToSetInCellWithCertainty($row, $column, $candidates, $squareCellToRowColumnMapper);
                    $i = ($number == 0) ? $i + 1 : $i;
                }

                if ($number == 0) {
                    simulateOneNumber($candidates, $cellsRemainToSet, $i, $number);
                    $row = $cellsRemainToSet[$i][0];
                    $column = $cellsRemainToSet[$i][1];

                    if (!$saveStateDone) {
                        saveState($cellsRemainToSet, $cellsRemainToSetAfterAddedNumbersWithCertainty, $numberOfCandidates, $workingSudokuBoard, $certaintySudokuBoard, $candidates, $candidatesAfterAddedNumbersWithCertainty, $numberOfCandidatesAfterAddedNumbersWithCertainty);
                        $saveStateDone = true;
                    }
                }

                $workingSudokuBoard[$row - 1][$column - 1] = $number;
                array_splice($cellsRemainToSet, $i, 1);
                $numberOfCandidates -= updateCandidates($candidates, $squareCellToRowColumnMapper, $row, $column, $number);
            }

            if (count($cellsRemainToSet) == 0) {
                $sudokuSolved = true;
            }
            else if (!$saveStateDone) {
                $numbersAddedWithCertaintyAndThenNoCandidates = true;
                $numberOfCellsSetInBestSoFar = 81 - $cellsRemainToSet.Count;
            }
            else {
                $numberOfCellsSetInBestSoFar = checkIfCanUpdateBestSoFarSudokuBoard($numberOfCellsSetInBestSoFar, $cellsRemainToSet, $workingSudokuBoard, $bestSoFarSudokuBoard);
                $numberOfAttemptsToSolveSudoku++;
            }
        }

        $result = processResult($currentSudokuToSolve - 1, $sudokuArray, $sudokuSolved, $numberOfCellsSetInInputSudokuBoard, $numberOfCellsSetInBestSoFar, $workingSudokuBoard, $bestSoFarSudokuBoard);

        if (($currentSudokuToSolve % 500) == 0) {
            print("\r" . $currentSudokuToSolve);
        }

        $currentSudokuToSolve++;
    }

    return $result;
}

function copyList(&$from, &$to) {
    array_splice($to, 0, count($to));

    for($i = 0; $i < count($from); $i++) {
        $to[] = $from[$i];
    }
}

function copySudokuBoard(&$sudokuBoardFrom, &$sudokuBoardTo) {
    for ($row = 1; $row <= 9; $row++) {
        for ($column = 1; $column <= 9; $column++) {
            $sudokuBoardTo[$row - 1][$column - 1] = $sudokuBoardFrom[$row - 1][$column - 1];
        }
    }
}

function copyCandidates(&$candidatesFrom, &$candidatesTo) {
    for ($row = 1; $row <= 9; $row++) {
        for ($column = 1; $column <= 9; $column++) {
            for ($i = 0; $i < 10; $i++) {
                $candidatesTo[$row - 1][$column - 1][$i] = $candidatesFrom[$row - 1][$column - 1][$i];
            }
        }
    }
}

function saveState(&$cellsRemainToSet, &$cellsRemainToSetAfterAddedNumbersWithCertainty, $numberOfCandidates, &$workingSudokuBoard, &$certaintySudokuBoard, &$candidates, &$candidatesAfterAddedNumbersWithCertainty, &$numberOfCandidatesAfterAddedNumbersWithCertainty) {
    copyList($cellsRemainToSet, $cellsRemainToSetAfterAddedNumbersWithCertainty);
    copySudokuBoard($workingSudokuBoard, $certaintySudokuBoard);
    copyCandidates($candidates, $candidatesAfterAddedNumbersWithCertainty);
    $numberOfCandidatesAfterAddedNumbersWithCertainty = $numberOfCandidates;
}

function restoreState(&$cellsRemainToSet, &$cellsRemainToSetAfterAddedNumbersWithCertainty, $numberOfCandidatesAfterAddedNumbersWithCertainty, &$workingSudokuBoard, &$certaintySudokuBoard, &$candidates, &$candidatesAfterAddedNumbersWithCertainty, &$numberOfCandidates) {
    copyList($cellsRemainToSetAfterAddedNumbersWithCertainty, $cellsRemainToSet);
    copySudokuBoard($certaintySudokuBoard, $workingSudokuBoard);
    copyCandidates($candidatesAfterAddedNumbersWithCertainty, $candidates);
    $numberOfCandidates = $numberOfCandidatesAfterAddedNumbersWithCertainty;
}

function getInputSudokuBoard(&$sudokuBoard, &$cellsRemainToSet, $index, &$sudokuArray) {

    $sudokuBoardString = str_replace("\r\n", "\n", $sudokuArray[$index]);

    $rows = explode("\n", $sudokuBoardString);

    if (count($rows) != 9) {
        return "Number of rows in input file are not 9 as expected!";
    }

    for ($row = 1; $row <= 9; $row++) {
        $columns = explode(" ", $rows[$row - 1]);

        if (count($columns) != 9) {
            return "Number of columns in input file in row $row  are not 9 as expected!";
        }

        for ($column = 1; $column <= 9; $column++) {
            $a = is_numeric($columns[$column - 1]);
            $b = 0;
            $c = 0;

            if ($a) {
                $b = intval($columns[$column - 1]);
                $c = floatval($columns[$column - 1]);
            }

            if (!$a || $b != $c) {
                return "The value \"" . $columns[$column - 1] . "\" in row " . $row . " and column " .  $column . " in input file is not a valid integer!";
            }

            if ($b < 0 || $b > 9) {
                return "The value \"" . $columns[$column - 1] . "\" in row " . $row . " and column " . $column . " in input file is not an integer in the interval [0, 9] as expected!";
            }

            $sudokuBoard[$row - 1][$column - 1] = $b;

            if ($b == 0) {
                $cellsRemainToSet[] = [$row, $column];
            }
        }
    }

    return null;
}

function candidateIsAlonePossible($number, &$candidates, &$squareCellToRowColumnMapper, $t, $target) {
    $numberOfOccurenciesOfNumber = 0;

    for ($i = 0; $i < 9; $i++) {
        switch ($target) {
            case Target::ROW:
                $row = $t;
                $column = $i + 1;
                break;
            case Target::COLUMN:
                $row = $i + 1;
                $column = $t;
                break;
            case Target::SQUARE:
                $row = $squareCellToRowColumnMapper[$t - 1][$i][0];
                $column = $squareCellToRowColumnMapper[$t - 1][$i][1];
                break;
        }

        $n = $candidates[$row - 1][$column - 1][0];

        if ($n > 0) {
            for ($j = 0; $j < $n; $j++) {
                if ($candidates[$row - 1][$column - 1][1 + $j] == $number) {
                    $numberOfOccurenciesOfNumber++;

                    if ($numberOfOccurenciesOfNumber > 1)
                        return false;
                }
            }
        }
    }

    return true;
}

function removeNumberIfItExists(&$v, $number) {
    $returnValue = 0;
    $index = -1;
    $i = 1;

    while ($i <= $v[0] && $number >= $v[$i] && $index == -1) { //The numbers in v are in increasing order
        if ($v[$i] == $number) {
            $index = $i;
            $returnValue = 1;
        }
        else
            $i++;
    }

    if ($index != -1) {
        while ($index + 1 <= $v[0]) {
            $v[$index] = $v[$index + 1];
            $index++;
        }

        $v[0]--;
    }

    return $returnValue;
}

function returnNumberOfOccurenciesOfNumber(&$sudokuBoard, &$squareCellToRowColumnMapper, $number, $t, $target) { //t refers to a row, column or square
    $n = 0;

    for ($i = 0; $i < 9; $i++)  {
        switch ($target) {
            case Target::ROW:
                $row = $t;
                $column = $i + 1;
                break;
            case Target::COLUMN:
                $row = $i + 1;
                $column = $t;
                break;
            case Target::SQUARE:
                $row = $squareCellToRowColumnMapper[$t - 1][$i][0];
                $column = $squareCellToRowColumnMapper[$t - 1][$i][1];
                break;
        }

        if ($sudokuBoard[$row - 1][$column - 1] == $number)
            $n++;
    }

    return $n;
}

function returnTwoDimensionalDataStructure($m, $n) {
    $v = [];

    for ($i = 0; $i < $m; $i++)
        $v[] = [];

    for ($i = 0; $i < $m; $i++)
        for ($j = 0; $j < $n; $j++)
            $v[$i][] = 0;
 
    return $v;
}

function returnThreeDimensionalDataStructure($l, $m, $n) {
    $v = [];

    for ($i = 0; $i < $l; $i++)
        $v[] = [];

    for ($i = 0; $i < $l; $i++)
        for ($j = 0; $j < $m; $j++)
            $v[$i][] = [];

    for ($i = 0; $i < $l; $i++)
        for ($j = 0; $j < $m; $j++)
            for ($k = 0; $k < $n; $k++)
                $v[$i][$j][] = 0;

    return $v;
}

function returnSquareCellToRowColumnMapper() {
    $v = returnThreeDimensionalDataStructure(9, 9, 2);

    $index = [0, 0, 0, 0, 0, 0, 0, 0, 0];

    for ($row = 1; $row <= 9; $row++) {
        for ($column = 1; $column <= 9; $column++) {
            $square = 1 + 3 * intdiv($row - 1,  3) + intdiv($column - 1,  3);
            $v[$square - 1][$index[$square - 1]][0] = $row;
            $v[$square - 1][$index[$square - 1]][1] = $column;
            $index[$square - 1]++;
        }
    }

    return $v;
}

function returnSudokuBoardAsString(&$sudokuBoard) {
    $sb = "";

    for ($row = 1; $row <= 9; $row++) {
        if ($row > 1)
            $sb .= "\r\n";

        for ($column = 1; $column <= 9; $column++) {
            if ($column == 1)
                $sb .= $sudokuBoard[$row - 1][$column - 1];
            else
                $sb .= (" " . $sudokuBoard[$row - 1][$column - 1]);
        }
    }

    return $sb;
}

function simulateOneNumber(&$candidates, &$cellsRemainToSet, &$index, &$number) {
    $v = [];
    $minNumberOfCandidates = 9;

    for ($i = 0; $i < count($cellsRemainToSet); $i++) {
        $row = $cellsRemainToSet[$i][0];
        $column = $cellsRemainToSet[$i][1];
        $numberOfCandidates = $candidates[$row - 1][$column - 1][0];

        if ($numberOfCandidates > 0 && $numberOfCandidates < $minNumberOfCandidates)
            $minNumberOfCandidates = $numberOfCandidates;
    }

    for ($i = 0; $i < count($cellsRemainToSet); $i++) {
        $row = $cellsRemainToSet[$i][0];
        $column = $cellsRemainToSet[$i][1];

        if ($candidates[$row - 1][$column - 1][0] == $minNumberOfCandidates)
            $v[] = $i;
    }

    $tmp = rand(0, count($v) - 1);
    $index = $v[$tmp];
    $row = $cellsRemainToSet[$index][0];
    $column = $cellsRemainToSet[$index][1];
    $number = $candidates[$row - 1][$column - 1][1 + rand(0, $minNumberOfCandidates - 1)];
}

function checkIfCanUpdateBestSoFarSudokuBoard($numberOfCellsSetInBestSoFar, &$cellsRemainToSet, &$workingSudokuBoard, &$bestSoFarSudokuBoard) {
    $retVal = $numberOfCellsSetInBestSoFar; //Default

    if ($numberOfCellsSetInBestSoFar < (81 - count($cellsRemainToSet))) {
        $retVal = 81 - count($cellsRemainToSet);
        copySudokuBoard($workingSudokuBoard, $bestSoFarSudokuBoard);
    }

    return $retVal;
}

function initCandidates(&$sudokuBoard, &$squareCellToRowColumnMapper, &$candidates) {
    $numberOfCandidates = 0;

    for ($row = 1; $row <= 9; $row++) {
        for ($column = 1; $column <= 9; $column++) {
            $square = 1 + 3 * intdiv($row - 1,  3) + intdiv($column - 1,  3);

            if ($sudokuBoard[$row - 1][$column - 1] != 0) {
                $candidates[$row - 1][$column - 1][0] = -1; //Indicates that the cell is set already
            }
            else {
                $n = 0;
                $candidates[$row - 1][$column - 1][0] = 0; //Number of candidates is set in index 0

                for ($number = 1; $number <= 9; $number++) {
                    if (
                        (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $row, Target::ROW) == 0) &&
                        (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $column, Target::COLUMN) == 0) &&
                        (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $square, Target::SQUARE) == 0)
                    ) {
                        $n++;
                        $candidates[$row - 1][$column - 1][0] = $n;
                        $candidates[$row - 1][$column - 1][$n] = $number;
                        $numberOfCandidates++;
                    }
                }
            }
        }
    }

    return $numberOfCandidates;
}

function tryFindNumberToSetInCellWithCertainty($row, $column, &$candidates, &$squareCellToRowColumnMapper) {
    $number = 0;

    $square = 1 + 3 * intdiv($row - 1,  3) + intdiv($column - 1,  3);
    $numberOfCandidatesInCell = $candidates[$row - 1][$column - 1][0];

    if ($numberOfCandidatesInCell == 1) 
        $number = $candidates[$row - 1][$column - 1][1];
    else {
        $i = 1;
        while ($i <= $numberOfCandidatesInCell && $number == 0) {
            $candidate = $candidates[$row - 1][$column - 1][$i];

            if (candidateIsAlonePossible($candidate, $candidates, $squareCellToRowColumnMapper, $row, Target::ROW) ||
                candidateIsAlonePossible($candidate, $candidates, $squareCellToRowColumnMapper, $column, Target::COLUMN) ||
                candidateIsAlonePossible($candidate, $candidates, $squareCellToRowColumnMapper, $square, Target::SQUARE))
                $number = $candidate;
            else
                $i++;
        }
    }

    return $number;
}

function updateCandidates(&$candidates, &$squareCellToRowColumnMapper, $row, $column, $number) {
    $totalNumberOfCandidatesRemoved = $candidates[$row - 1][$column - 1][0]; //Remove all candidates in that cell
    $candidates[$row - 1][$column - 1][0] = -1; //Indicates that the cell is set already

    $square = 1 + 3 * intdiv($row - 1,  3) + intdiv($column - 1,  3);

    for ($c = 1; $c <= 9; $c++) {
        if ($c != $column && $candidates[$row - 1][$c - 1][0] > 0) {
            $totalNumberOfCandidatesRemoved += removeNumberIfItExists($candidates[$row - 1][$c - 1], $number);
        }
    }

    for ($r = 1; $r <= 9; $r++) {
        if ($r != $row && $candidates[$r - 1][$column - 1][0] > 0) {
            $totalNumberOfCandidatesRemoved += removeNumberIfItExists($candidates[$r - 1][$column - 1], $number);
        }
    }

    for ($i = 0; $i < 9; $i++) {
        $r = $squareCellToRowColumnMapper[$square - 1][$i][0];
        $c = $squareCellToRowColumnMapper[$square - 1][$i][1];

        if ($r != $row && $c != $column && $candidates[$r - 1][$c - 1][0] > 0) {
            $totalNumberOfCandidatesRemoved += removeNumberIfItExists($candidates[$r - 1][$c - 1], $number);
        }
    }

    return $totalNumberOfCandidatesRemoved;
}

function processResult($index, &$sudokuArray, $sudokuSolved, $numberOfCellsSetInInputSudokuBoard, $numberOfCellsSetInBestSoFar, &$workingSudokuBoard, &$bestSoFarSudokuBoard) {

    if ($sudokuSolved) {
        $msg = "S";
    }
    else {
        $tmp1 = $numberOfCellsSetInBestSoFar - $numberOfCellsSetInInputSudokuBoard;
        $tmp2 = 81 - $numberOfCellsSetInBestSoFar;
        $msg = "The sudoku was partially solved. $tmp1 number(s) added to the original $numberOfCellsSetInInputSudokuBoard . Unable to set $tmp2 number(s).";
    }

    if ($sudokuSolved || $bestSoFarSudokuBoard == null) {
        $sudokuArray[$index] = returnSudokuBoardAsString($workingSudokuBoard);
    }
    else {
        $sudokuArray[$index] = returnSudokuBoardAsString($bestSoFarSudokuBoard);
    }
    
    return $msg;
}

$start = date("Y,m,d,H,i,s") . "\r\n";

$n = filesize("C:\\Sudoku\\Solve\\StartSudokuBoards.txt");
$f = fopen("C:\\Sudoku\\Solve\\StartSudokuBoards.txt", "r");
$s = fread($f, $n);
fclose($f);

$sudokuArray = explode("\r\n-- New sudoku --\r\n", $s);

$result = run($sudokuArray);

if ($result == "S") {
    $end = date("Y,m,d,H,i,s");
    $str = $start . $end;
    $f = fopen("C:\\Sudoku\\Solve\\StartEnd.txt", "w");
    fwrite($f, $str);
    fclose($f);

    $f = fopen("C:\\Sudoku\\Solve\\SolveResult.txt", "w");
    $n = count($sudokuArray);

    for($i = 0; $i < $n; $i++) {
        fwrite($f, $sudokuArray[$i]);

        if ($i < ($n - 1)) {
            fwrite($f, "\r\n-- New sudoku --\r\n");
        }
    }

    fclose($f);
}
else {
    $f = fopen("C:\\Sudoku\\Solve\\Error.txt", "w");
    fwrite($f, $result);
    fclose($f);
}

?>