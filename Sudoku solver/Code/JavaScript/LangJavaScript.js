var fs = require('fs');
langJavaScript = {};

langJavaScript.forStatement = function() {
    var i, sumInteger1To10 = 0;

    for (i = 1; i <= 10; i++) {
        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);

    //or (curly braces optioinal if only one statement for-body)
    sumInteger1To10 = 0;
    for (i = 1; i <= 10; i++)
        sumInteger1To10 += i;

    console.log(sumInteger1To10);

    for (i = 1; i <= 10; i++) { //For several statements then must have curly braces
        if (i == 1)
            sumInteger1To10 = 0;

        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);
}

langJavaScript.readWriteTextFromFile = function () {
    var s = fs.readFileSync("C:\\tmp\\tmp.txt", { encoding: 'ascii' });
    console.log(s);

    f = fs.openSync("C:\\tmp\\tmp.txt", "w");
    fs.writeFileSync(f, s + " Hello World!", { encoding: 'ascii' });
    fs.closeSync(f);

    s = fs.readFileSync("C:\\tmp\\tmp.txt", { encoding: 'ascii' });
    console.log(s);
}

langJavaScript.whileStatement = function() {
    var i = 0, sumInteger1To10 = 0;

    while (++i <= 10) {
        sumInteger1To10 += i;
    }

    console.log(sumInteger1To10);

    //or (curly braces optioinal if only one statement in while-body)
    i = 0;
    sumInteger1To10 = 0;
    while (++i <= 10)
        sumInteger1To10 += i;

    console.log(sumInteger1To10);

    i = 1;
    sumInteger1To10 = 0;
    while (i <= 10) { //For several statements then must have curly braces
        sumInteger1To10 += i;
        i++;
    }

    console.log(sumInteger1To10);
}

langJavaScript.forStatement();
langJavaScript.readWriteTextFromFile();
langJavaScript.whileStatement();