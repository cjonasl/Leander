<?php

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

$from = [1, 2, 3, 4, 5, 6, 7, 8, 9];
$to = [4, 7];
copyList($from, $to);
print_r($to)

?>