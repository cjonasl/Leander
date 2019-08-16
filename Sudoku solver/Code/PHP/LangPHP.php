<?php

function forStatement() {
    $sumInteger1To10 = 0;

    for ($i = 1; $i <= 10; $i++) {
        $sumInteger1To10 += $i;
    }

    print($sumInteger1To10 . "\r\n");

    //or (curly braces optioinal if only one statement for-body)
    $sumInteger1To10 = 0;
    for ($i = 1; $i <= 10; $i++)
        $sumInteger1To10 += $i;

    print($sumInteger1To10 . "\r\n");

    for ($i = 1; $i <= 10; $i++) { //For several statements then must have curly braces
        if (i == 1)
            $sumInteger1To10 = 0;

        $sumInteger1To10 += i;
    }

    print($sumInteger1To10 . "\r\n");
}

function readWriteTextFromFile() {
    $n = filesize("C:\\tmp\\tmp.txt");
    $f = fopen("C:\\tmp\\tmp.txt", "r");
    $s = fread($f, $n);
    fclose($f);
    print($s . "\r\n");

    $f = fopen("C:\\tmp\\tmp.txt", "w");
    fwrite($f, $s . " Hello World!");
    fclose($f);

    $f = fopen("C:\\tmp\\tmp.txt", "r");
    $n += 13; //Length of " Hello World!" is 13
    $s = fread($f, $n);
    fclose($f);
    print($s . "\r\n");
}

function whileStatement() {
    $i = 0;
    $sumInteger1To10 = 0;

    while (++$i <= 10) {
        $sumInteger1To10 += $i;
    }

    print($sumInteger1To10 . "\r\n");

    //or (curly braces optioinal if only one statement in while-body)
    $i = 0;
    $sumInteger1To10 = 0;
    while (++$i <= 10)
        $sumInteger1To10 += $i;

    print($sumInteger1To10 . "\r\n");

    $i = 1;
    $sumInteger1To10 = 0;
    while ($i <= 10) { //For several statements then must have curly braces
        $sumInteger1To10 += $i;
        $i++;
    }

    print($sumInteger1To10 . "\r\n");
}

forStatement();
readWriteTextFromFile();
whileStatement();

?>