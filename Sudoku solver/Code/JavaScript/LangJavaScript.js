"use strict";

var fs = require('fs');
var langJavaScript = {};


langJavaScript.Expression = function () {
    var a;

    //An expression should be terminated by a semicolon (works without also)
    a = 2;
    console.log(a);
};

langJavaScript.Statement = function () {
    return 3.14; //A statement should be terminated by a semicolon (works without also)
};

langJavaScript.forLoop = function () {
    var i, sumInteger1To10 = 0;

    for (i = 1; i <= 10; i++) {
        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);

    //Curly braces are optioinal if only one expression/statement
    sumInteger1To10 = 0;
    for (i = 1; i <= 10; i++)
        sumInteger1To10 += i;

    console.log(sumInteger1To10);

    for (i = 1; i <= 10; i++) { //Must have curly braces when several expressions/statements
        if (i === 1) //Does not work without the parentheses
            sumInteger1To10 = 0;

        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);
};

langJavaScript.readWriteTextFromFile = function () {
    var s, f;

    s = fs.readFileSync("C:\\tmp\\tmp.txt", { encoding: 'ascii' });
    console.log(s);

    f = fs.openSync("C:\\tmp\\tmp.txt", "w");
    fs.writeFileSync(f, s + " Hello World!", { encoding: 'ascii' });
    fs.closeSync(f);

    s = fs.readFileSync("C:\\tmp\\tmp.txt", { encoding: 'ascii' });
    console.log(s);
};

langJavaScript.whileLoop = function () {
    var i = 0, sumInteger1To10 = 0;

    while (++i <= 10) { //Does not work without the parentheses
        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);

    //Curly braces are optioinal if only one expression/statement
    i = 0;
    sumInteger1To10 = 0;
    while (++i <= 10)
        sumInteger1To10 += i;

    console.log(sumInteger1To10);

    i = 1;
    sumInteger1To10 = 0;
    while (i <= 10) { //Must have curly braces when several expressions/statements
        sumInteger1To10 += i;
        i++;
    }

    console.log(sumInteger1To10);
};

langJavaScript.Expression();
console.log(langJavaScript.Statement());
langJavaScript.forLoop();
langJavaScript.readWriteTextFromFile();
langJavaScript.whileLoop();