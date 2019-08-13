<?php

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

function getInputSudokuBoard(&$args, &$sudokuBoard, &$cellsRemainToSet) {

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

function returnSudokuBoardAsString ($sudokuBoard) {
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

$from = [1, 2, 3, 4, 5, 6, 7, 8, 9];
$to = [4, 7];
copyList($from, $to);
print_r($to)

?>