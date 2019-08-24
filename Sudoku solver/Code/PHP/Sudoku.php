﻿<?php

class Target {
    const ROW = 1;
    const COLUMN = 2;
    const SQUARE = 3;
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

function saveState(&$ccellsRemainToSet, &$ccellsRemainToSetAfterAddedNumbersWithCertainty, &$cnumberOfCandidates, &$cworkingSudokuBoard, &$ccertaintySudokuBoard, &$ccandidates, &$ccandidatesAfterAddedNumbersWithCertainty, &$cnumberOfCandidatesAfterAddedNumbersWithCertainty) {
    copyList($cellsRemainToSet, $cellsRemainToSetAfterAddedNumbersWithCertainty);
    copySudokuBoard($workingSudokuBoard, $certaintySudokuBoard);
    copyCandidates($candidates, $candidatesAfterAddedNumbersWithCertainty);
    $numberOfCandidatesAfterAddedNumbersWithCertainty = $numberOfCandidates;
}

function restoreState(&$ccellsRemainToSet, &$ccellsRemainToSetAfterAddedNumbersWithCertainty, &$cnumberOfCandidatesAfterAddedNumbersWithCertainty, &$cworkingSudokuBoard, &$ccertaintySudokuBoard, &$ccandidates, &$ccandidatesAfterAddedNumbersWithCertainty, &$cnumberOfCandidates) {
    copyList($cellsRemainToSetAfterAddedNumbersWithCertainty, $cellsRemainToSet);
    copySudokuBoard($certaintySudokuBoard, $workingSudokuBoard);
    copyCandidates($candidatesAfterAddedNumbersWithCertainty, $candidates);
    $numberOfCandidates = $numberOfCandidatesAfterAddedNumbersWithCertainty;
}

function getInputSudokuBoard(&$args, &$sudokuBoard, &$cellsRemainToSet) {
    $n = count($args);

    if ($n == 0) {
        return "An input file is not given to the program (first parameter)!";
    }
    else if ($n > 2) {
        return "At most two parameters may be given to the program!";
    }
    else if (!is_file($args[0])) {
        return "The given input file in first parameter does not exist!";
    }
    else if ($n == 2 && !is_dir($args[1])) {
        return "The directory given in second parameter does not exist!";
    }

    $n = filesize($args[0]);
    $f = fopen($args[0], "r");
    $sudokuBoardString = str_replace("\r\n", "\n", trim(fread($f, $n)));
    fclose($f);


    $rows = explode("\n", $sudokuBoardString);

    if (count($rows) != 9) {
        return "Number of rows in input file are not 9 as expected!";
    }

    for ($row = 1; $row <= 9; $row++) {
        $columns = explode(" ", $rows[$row - 1]);

        if (count($columns) != 9) {
            return "Number of columns in input file in row " . $row . " are not 9 as expected!";
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
                $cellsRemainToSet[] = [$row, $colum];
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

    $n = $v[0];
    $i = 1;
    while ($i <= $n && $index == -1) {
        if ($v[$i] == $number) {
            $index = $i;
            $returnValue = 1;
        }
        else
            $i++;
    }

    if ($index != -1) {
        while ($index + 1 <= $n) {
            $v[$index] = $v[$index + 1];
            $index++;
        }

        $v[0]--;
    }

    return $returnValue;
}

function returnNumberOfOccurenciesOfNumber(&$sudokuBoard, &$squareCellToRowColumnMapper, $number, $t, $target) { //t refers to a row, column or square
    $n = 0;

    for ($i = 0; $i < 9; $i++)
    {
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
                $row = squareCellToRowColumnMapper[$t - 1][$i][0];
                $column = squareCellToRowColumnMapper[$t - 1][$i][1];
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

    return v;
}

function returnSudokuBoardAsString($sudokuBoard) {
    $sb = "";

    for ($row = 1; $row <= 9; $row++) {
        if ($row > 1)
            $sb += "\r\n";

        for ($column = 1; $column <= 9; $column++) {
            if ($column == 1)
                $sb += $sudokuBoard[$row - 1][$column - 1];
            else
                $sb += (" " . $sudokuBoard[$row - 1][$column - 1]);
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

    return retVal;
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
        $r = squareCellToRowColumnMapper[$square - 1][$i][0];
        $c = squareCellToRowColumnMapper[$square - 1][$i][1];

        if ($r != $row && $c != $column && $candidates[$r - 1][$c - 1][0] > 0) {
            $totalNumberOfCandidatesRemoved += removeNumberIfItExists($candidates[$r - 1][$c - 1], $number);
        }
    }

    return $totalNumberOfCandidatesRemoved;
}

function validateSudokuBoard(&$sudokuBoard, &$squareCellToRowColumnMapper) {
    for ($row = 1; $row <= 9; $row++) {
        for ($column = 1; $column <= 9; $column++) {
            $square = 1 + 3 * intdiv($row - 1,  3) + intdiv($column - 1,  3);
            $number = $sudokuBoard[$row - 1][$column - 1];

            if ($number != 0) {
                if (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $row, Target::ROW) > 1) {
                    return "The input sudoku is incorrect! The number " . $number . " occurs more than once in row " . $row;
                }
                else if (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $column, Target::COLUMN) > 1) {
                    return "The input sudoku is incorrect! The number " . $number . " occurs more than once in column " . $column;
                }
                else if (returnNumberOfOccurenciesOfNumber($sudokuBoard, $squareCellToRowColumnMapper, $number, $square, Target::SQUARE) > 1) {
                    return "The input sudoku is incorrect! The number " . $number . " occurs more than once in square " . $square;
                }
            }
        }
    }

    return null;
}

function printSudokuBoard($solved, &$args, $message, &$sudokuBoard) {
    $index = 1 + strrpos($args[0], "\\");
    $fileName = substr($args[0], $index, strlen($args[0]) - $index);
    $dt = date('Y.m.d.H.i.s.') . gettimeofday()['usec'];
    $dt = substr($dt, 0, strlen($dt) - 3);

    if (solved)
        suffix = "__Solved_" . $dt . ".txt";
    else
        suffix = "__Partially_solved_" . $dt . ".txt";

    if (count($args) == 2) {
        $c = trim(args[1]);
        $c = $c[strlen($c) - 1];
    }

            if (args.Length == 2)
            {
                c = args[1].Trim()[args[1].Trim().Length - 1];
                fileNameFullpath = args[1].Trim() + ((c == '\\') ? "" : "\\") + (new FileInfo(args[0])).Name + suffix;
            }
            else
                fileNameFullpath = args[0] + suffix;

            File.WriteAllText(fileNameFullpath, message + "\r\n\r\n" + ReturnSudokuBoardAsString(sudokuBoard));
}

function printResult($initialSudokuBoardHasCandidates, &$args, $msg, $sudokuSolved, $numberOfCellsSetInInputSudokuBoard, $numberOfCellsSetInBestSoFar, &$workingSudokuBoard, &$bestSoFarSudokuBoard) {
    if ($initialSudokuBoardHasCandidates) {
        if ($sudokuSolved) {
            $tmp1 = 81 - $numberOfCellsSetInInputSudokuBoard;
            $msg = "The sudoku was solved. " . $tmp1 . " number(s) added to the original " . $numberOfCellsSetInInputSudokuBoard . ".";
        }
        else {
            $tmp1 = $numberOfCellsSetInBestSoFar - $numberOfCellsSetInInputSudokuBoard;
            $tmp2 = 81 - $numberOfCellsSetInBestSoFar;
            $msg = "The sudoku was partially solved. " . $tmp1 . " number(s) added to the original " . $numberOfCellsSetInInputSudokuBoard . ". Unable to set " . $tmp2 . " number(s).";
        }

        if ($sudokuSolved || $bestSoFarSudokuBoard == null) {
            printSudokuBoard($sudokuSolved, $args, $msg, $workingSudokuBoard);
        }
        else {
            printSudokuBoard($sudokuSolved, $args, $msg, $bestSoFarSudokuBoard);
        }
    }

    print($msg);
}

$from = [1, 2, 3, 4, 5, 6, 7, 8, 9];
$to = [4, 7];
copyList($from, $to);
print_r($to)

?>