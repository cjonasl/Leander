window.Jonas = new window.Object();

window.Jonas.ReturnPropertyDescriptorAsString = function (obj, prop) {
    var propertyDescriptorObject = window.Object.getOwnPropertyDescriptor(obj, prop);
    var arrayOfOwnPropertyNames = window.Object.getOwnPropertyNames(propertyDescriptorObject);
    var numberOfOwnPropertyNames = arrayOfOwnPropertyNames.length;
    var i, type, propertyName, propertyValue;

    var s = "{";

    for (i = 0; i < numberOfOwnPropertyNames; i++) {
        propertyName = arrayOfOwnPropertyNames[i];

        if (propertyDescriptorObject[propertyName] === null) {
            type = "null";
        }
        else {
            type = typeof propertyDescriptorObject[propertyName];
        }

        if ((type === "string") || (type === "number") || (type === "boolean")) {
            propertyValue = propertyDescriptorObject[propertyName].toString();
        }
        else {
            propertyValue = type;
        }

        if (s === "{") {
            s += propertyName + ": " + propertyValue;
        }
        else {
            s += ", " + propertyName + ": " + propertyValue;
        }
    }

    s += "}";

    return s;
}

window.Jonas.ReturnArrayWithDistinctOwnPropertyNamesAndSorted = function (obj) {
    var array1, array2, numberOfElements, i, str;

    array1 = window.Object.getOwnPropertyNames(obj).sort();

    numberOfElements = array1.length;
    array2 = [];

    for (i = 0; i < numberOfElements; i++) {
        str = array1[i];

        if (array1.indexOf(str) === i) {
            array2.push(str);
        }
        else
        {
            alert("OBS, dubbla properties av \"" + str + "\"!!!");
        }
    }

    return array2;
}

window.Jonas.IsPropertyOverridden = function(propertyName, currentLevel, arrayWithArraysWithOwnDistinctPropertyNames)
{
    var returnValue, i, j, numberOfLevels, tmpArray, numberOfOwnDistinctPropertiesInCurrentLevel, i; //deafult

    numberOfLevels = arrayWithArraysWithOwnDistinctPropertyNames.length;
    returnValue = false;

    i = currentLevel;

    while ((!returnValue) && (i < numberOfLevels))
    {
        tmpArray = arrayWithArraysWithOwnDistinctPropertyNames[i];
        numberOfOwnDistinctPropertiesInCurrentLevel = tmpArray.length;

        j = 0;

        while ((!returnValue) && (j < numberOfOwnDistinctPropertiesInCurrentLevel)) {

            if (propertyName === tmpArray[j])
            {
                returnValue = true;
            }
            else
            {
                j++;
            }
        }

        i++;
    }

    return returnValue;
}

window.Jonas.ShowPropertiesForSpecificLevel = function (arrayWithObjectsToAnalyze, i, arrayWithArraysWithOwnDistinctPropertyNames) {
    var j, type, theClass, obj, level, propertyName, numberOfOwnDistinctPropertyNames, propertyValue, finalString, listElement, propertyIsOverridden, tmpArray;
    var paragraphElement, spanElement, unorderedListElement, textNode;

    numberOfOwnDistinctPropertyNames = arrayWithArraysWithOwnDistinctPropertyNames[i].length;

    level = i + 1;

    paragraphElement = window.document.createElement('p');
    spanElement = window.document.createElement('span');
    unorderedListElement = window.document.createElement('ul');

    spanElement.setAttribute("style", "font-weight: bold");
    textNode = document.createTextNode("Level " + level.toString() + ", class: " + window.Object.prototype.toString.call(arrayWithObjectsToAnalyze[i]).slice(8, -1) + ", number of own distinct properties: " + numberOfOwnDistinctPropertyNames.toString());
    spanElement.appendChild(textNode);

    paragraphElement.appendChild(spanElement)
    paragraphElement.appendChild(window.document.createElement('br'));

    for (j = 0; j < numberOfOwnDistinctPropertyNames; j++) {
        propertyName = arrayWithArraysWithOwnDistinctPropertyNames[i][j];

        if (window.Jonas.IsPropertyOverridden(propertyName, level, arrayWithArraysWithOwnDistinctPropertyNames))
        {
            obj = arrayWithObjectsToAnalyze[i];
            propertyIsOverridden = true;
        }
        else
        {
            obj = arrayWithObjectsToAnalyze[arrayWithObjectsToAnalyze.length - 1];
            propertyIsOverridden = false;
        }

        try {
            if (obj[propertyName] === null) {
                type = "null";
            }
            else {
                type = typeof obj[propertyName];
            }

            if ((type === "null") || (type === "undefined"))
            {
                finalString = propertyName + ": " + type + ", (" + type + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
            }
            else if ((type === "string") || (type === "number") || (type === "boolean") || (type === "object") || (type === "function"))
            {
                theClass = window.Object.prototype.toString.call(obj[propertyName]).slice(8, -1);

                if ((type === "string") || (type === "number") || (type === "boolean"))
                {
                    propertyValue = obj[propertyName].toString();
                    finalString = propertyName + ": " + propertyValue + " (" + theClass + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
                }
                else
                {
                    if (theClass !== "Array") {
                        tmpArray = window.Jonas.ReturnArrayWithDistinctOwnPropertyNamesAndSorted(obj[propertyName]);
                    }

                    if (theClass === "Array") {
                        finalString = propertyName + ": object (Array, length = " + obj[propertyName].length.toString() + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
                    }
                    else if (theClass === "Function") {
                    
                        finalString = propertyName + ": function (Function, number of arguments = " + obj[propertyName].length.toString() + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ", own properties " + tmpArray.length.toString() + " stycken [" + tmpArray .join(", ") + "])";
                    }
                    else
                    {
                        finalString = propertyName + ": object (" + theClass + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ", own properties " + tmpArray.length.toString() + " stycken [" + tmpArray.join(", ") + "])";
                    }
                }
            }
            else
            {
                finalString = propertyName + ": TYPEN ÄR INTE string, number, boolean, undefined, object eller function SOM FÖRVÄNTAT UTAN \"" + type + "\"!!!";
            }
        }
        catch (err) {
            finalString = propertyName + ": An error occurred, error message = " + err.toString();
        }

        listElement = window.document.createElement('li');

        if (propertyIsOverridden)
        {
            listElement.setAttribute("style", "color: red");
        }

        listElement.appendChild(document.createTextNode(finalString));
        unorderedListElement.appendChild(listElement);
    }

    paragraphElement.appendChild(unorderedListElement);
    window.Jonas.elementWhereToPlaceInfo.appendChild(paragraphElement);
}

window.Jonas.ShowPropertyInfoForObject = function (obj, objDescription, elementWhereToPlaceInfo) {
    var headerElement, headerTextElement, arrayWithObjectsToAnalyze, numberOfLevels;
    var arrayWithArraysWithOwnDistinctPropertyNames, i;

    elementWhereToPlaceInfo.innerText = "";

    headerElement = document.createElement('h3');
    headerTextElement = document.createTextNode(objDescription + ' (class = ' + window.Object.prototype.toString.call(obj).slice(8, -1) + ')');
    headerElement.appendChild(headerTextElement);

    elementWhereToPlaceInfo.appendChild(headerElement);

    window.Jonas.elementWhereToPlaceInfo = elementWhereToPlaceInfo;

    arrayWithObjectsToAnalyze = [];

    while (obj != null) {
        arrayWithObjectsToAnalyze.push(obj);
        obj = window.Object.getPrototypeOf(obj);
    }

    arrayWithObjectsToAnalyze.reverse();
    numberOfLevels = arrayWithObjectsToAnalyze.length;
    arrayWithArraysWithOwnDistinctPropertyNames = [];

    for (i = 0; i < numberOfLevels; i++)
    {
        arrayWithArraysWithOwnDistinctPropertyNames.push(window.Jonas.ReturnArrayWithDistinctOwnPropertyNamesAndSorted(arrayWithObjectsToAnalyze[i]));
    }

    for (i = 0; i < numberOfLevels; i++) {
        window.Jonas.ShowPropertiesForSpecificLevel(arrayWithObjectsToAnalyze, i, arrayWithArraysWithOwnDistinctPropertyNames);
    }
}

window.Jonas.ReturnPropertiesForSpecificLevel = function (arrayWithObjectsToAnalyze, i, arrayWithArraysWithOwnDistinctPropertyNames) {
    var j, type, theClass, obj, level, propertyName, numberOfOwnDistinctPropertyNames, propertyValue, finalString, propertyIsOverridden, tmpArray, returnString;

    numberOfOwnDistinctPropertyNames = arrayWithArraysWithOwnDistinctPropertyNames[i].length;

    level = i + 1;

    returnString = "<p><span><b>Level " + level.toString() + ", class: " + window.Object.prototype.toString.call(arrayWithObjectsToAnalyze[i]).slice(8, -1) + ", number of own distinct properties: " + numberOfOwnDistinctPropertyNames.toString() + "</b></span><br/><ul>";

    for (j = 0; j < numberOfOwnDistinctPropertyNames; j++) {
        propertyName = arrayWithArraysWithOwnDistinctPropertyNames[i][j];

        if (window.Jonas.IsPropertyOverridden(propertyName, level, arrayWithArraysWithOwnDistinctPropertyNames)) {
            obj = arrayWithObjectsToAnalyze[i];
            propertyIsOverridden = true;
        }
        else {
            obj = arrayWithObjectsToAnalyze[arrayWithObjectsToAnalyze.length - 1];
            propertyIsOverridden = false;
        }

        try {
            if (obj[propertyName] === null) {
                type = "null";
            }
            else {
                type = typeof obj[propertyName];
            }

            if ((type === "null") || (type === "undefined")) {
                finalString = propertyName + ": " + type + ", (" + type + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
            }
            else if ((type === "string") || (type === "number") || (type === "boolean") || (type === "object") || (type === "function")) {
                theClass = window.Object.prototype.toString.call(obj[propertyName]).slice(8, -1);

                if ((type === "string") || (type === "number") || (type === "boolean")) {
                    propertyValue = obj[propertyName].toString();
                    finalString = propertyName + ": " + propertyValue + " (" + theClass + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
                }
                else {
                    if (theClass !== "Array") {
                        tmpArray = window.Jonas.ReturnArrayWithDistinctOwnPropertyNamesAndSorted(obj[propertyName]);
                    }

                    if (theClass === "Array") {
                        finalString = propertyName + ": object (Array, length = " + obj[propertyName].length.toString() + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ")";
                    }
                    else if (theClass === "Function") {

                        finalString = propertyName + ": function (Function, number of arguments = " + obj[propertyName].length.toString() + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ", own properties " + tmpArray.length.toString() + " stycken [" + tmpArray.join(", ") + "])";
                    }
                    else {
                        finalString = propertyName + ": object (" + theClass + ", property descriptor: " + window.Jonas.ReturnPropertyDescriptorAsString(arrayWithObjectsToAnalyze[i], propertyName) + ", own properties " + tmpArray.length.toString() + " stycken [" + tmpArray.join(", ") + "])";
                    }
                }
            }
            else {
                finalString = propertyName + ": TYPEN ÄR INTE string, number, boolean, undefined, object eller function SOM FÖRVÄNTAT UTAN \"" + type + "\"!!!";
            }
        }
        catch (err) {
            finalString = propertyName + ": An error occurred, error message = " + err.toString();
        }

        if (propertyIsOverridden) {
            returnString += "<li style='color: red'>" + finalString + "</li>"
        }
        else {
            returnString += "<li>" + finalString + "</li>"
        }
    }

    returnString += "</ul></p>";

    return returnString;
}

window.Jonas.ReturnPropertyInfoForObject = function (obj, objDescription) {
    var arrayWithObjectsToAnalyze, numberOfLevels, arrayWithArraysWithOwnDistinctPropertyNames, returnString, i;

    returnString = "<h3>" + objDescription + " (class = " + window.Object.prototype.toString.call(obj).slice(8, -1) + ")</h3>";

    arrayWithObjectsToAnalyze = [];

    while (obj != null) {
        arrayWithObjectsToAnalyze.push(obj);
        obj = window.Object.getPrototypeOf(obj);
    }

    arrayWithObjectsToAnalyze.reverse();
    numberOfLevels = arrayWithObjectsToAnalyze.length;
    arrayWithArraysWithOwnDistinctPropertyNames = [];

    for (i = 0; i < numberOfLevels; i++) {
        arrayWithArraysWithOwnDistinctPropertyNames.push(window.Jonas.ReturnArrayWithDistinctOwnPropertyNamesAndSorted(arrayWithObjectsToAnalyze[i]));
    }

    for (i = 0; i < numberOfLevels; i++) {
        returnString += window.Jonas.ReturnPropertiesForSpecificLevel(arrayWithObjectsToAnalyze, i, arrayWithArraysWithOwnDistinctPropertyNames);
    }

    return returnString;
}

window.Jonas.ReturnArrayWithKeys = function (v) {
    var u = [];

    if (Array.isArray(v)) {
        for (var i = 0; i < v.length; i++) {
            u.push("");
        }
    }
    else {
        for (var key in v) {
            u.push(key + ": ");
        }
    }

    return u;
}

window.Jonas.ReturnArrayWithValues = function (v) {
    var u = [];

    if (Array.isArray(v)) {
        for (var i = 0; i < v.length; i++) {
            u.push(v[i]);
        }
    }
    else {
        for (var key in v) {
            u.push(v[key]);
        }
    }

    return u;
}

window.Jonas.ReturnInfoAboutObjectOrArrayAsString = function (v) {
    var str, arrayWithKeys, arrayWithValues;

    if (v === undefined) {
        str = "undefined";
    }
    else if (v === null) {
        str = "null";
    }
    else if ((Array.isArray(v)) && (v.length === 0)) {
        return "[]";
    }
    else if ((Array.isArray(v)) && (v.length === 0)) {
        return "[]";
    }
    else if ((typeof (v) === "object") && (Object.keys(v).length === 0)) {
        str = "{}";
    }
    else if (((Array.isArray(v)) && (v.length > 0)) || (typeof (v) === "object")) {
        if (Array.isArray(v)) {
            str = "[";
        }
        else {
            str = "{";
        }

        arrayWithKeys = window.Jonas.ReturnArrayWithKeys(v);
        arrayWithValues = window.Jonas.ReturnArrayWithValues(v);

        for (var i = 0; i < (arrayWithValues.length - 1) ; i++) {
            if (arrayWithValues[i] === undefined) {
                str += (arrayWithKeys[i] + "undefined, ");
            }
            else if (arrayWithValues[i] === null) {
                str += (arrayWithKeys[i] + "null, ");
            }
            else if ((window.isNaN(arrayWithValues[i])) && (typeof (arrayWithValues[i]) === "number")) {
                str += (arrayWithKeys[i] + "NaN, ");
            }
            else if ((Array.isArray(arrayWithValues[i])) || (typeof (arrayWithValues[i]) === "object")) {
                str += ((arrayWithKeys[i] + window.Jonas.ReturnInfoAboutObjectOrArrayAsString(arrayWithValues[i]) + ", "));
            }
            else {
                str += ((arrayWithKeys[i] + arrayWithValues[i].toString() + ", "));
            }
        }

        if (arrayWithValues[arrayWithValues.length - 1] === undefined) {
            str += (arrayWithKeys[arrayWithValues.length - 1] + "undefined");
        }
        else if (arrayWithValues[arrayWithValues.length - 1] === null) {
            str += (arrayWithKeys[arrayWithValues.length - 1] + "null");
        }
        else if ((window.isNaN(arrayWithValues[arrayWithValues.length - 1])) && (typeof (arrayWithValues[arrayWithValues.length - 1]) === "number")) {
            str += (arrayWithKeys[arrayWithValues.length - 1] + "NaN");
        }
        else if ((Array.isArray(arrayWithValues[arrayWithValues.length - 1])) || (typeof (arrayWithValues[arrayWithValues.length - 1]) === "object")) {
            str += ((arrayWithKeys[arrayWithValues.length - 1] + window.Jonas.ReturnInfoAboutObjectOrArrayAsString(arrayWithValues[arrayWithValues.length - 1])));
        }
        else {
            str += (arrayWithKeys[arrayWithValues.length - 1] + arrayWithValues[arrayWithValues.length - 1].toString());
        }

        if (Array.isArray(v)) {
            str += "]";
        }
        else {
            str += "}";
        }
    }
    else {
        str = v.toString();
    }

    return str;
}

//Take only properties where the value is of type number, string or boolean
window.Jonas.ReturnTableWrtOwnAndNotOwnPropertiesAndEnumerableAndNotEnumerable = function (obj) {
    var objNotOwnAndNotEnumerable = {}, objNotOwnAndIsEnumerable = {}, objIsOwnAndNotEnumerable = {}, objIsOwnAndIsEnumerable = {};
    var i;
    var isEnumerable = [];

    var v = Object.getOwnPropertyNames(obj); //Own and both enumerable and not enumerable

    for (i = 0; i < v.length; i++) {
        isEnumerable[i] = obj.propertyIsEnumerable(v[i]);
    }

    var p = Object.getPrototypeOf(obj);
    var ownPropertyNames, i, str, s1, s2, s3, s4;

    while (p != null) {
        var ownPropertyNames = Object.getOwnPropertyNames(p);

        for (i = 0; i < ownPropertyNames.length; i++) {
            if (v.indexOf(ownPropertyNames[i]) === -1) {
                v.push(ownPropertyNames[i]);
                isEnumerable.push(p.propertyIsEnumerable(ownPropertyNames[i]));
            }
        }

        p = Object.getPrototypeOf(p);
    }

    for (i = 0; i < v.length; i++) {
        if ((typeof obj[v[i]] === "string") || (typeof obj[v[i]] === "number") || (typeof obj[v[i]] === "boolean")) {
            if ((!obj.hasOwnProperty(v[i])) && (!isEnumerable[i])) {
                objNotOwnAndNotEnumerable[v[i]] = obj[v[i]];
            }
            else if ((!obj.hasOwnProperty(v[i])) && (isEnumerable[i])) {
                objNotOwnAndIsEnumerable[v[i]] = obj[v[i]];
            }
            else if ((obj.hasOwnProperty(v[i])) && (!isEnumerable[i])) {
                objIsOwnAndNotEnumerable[v[i]] = obj[v[i]];
            }
            else {
                objIsOwnAndIsEnumerable[v[i]] = obj[v[i]];
            }
        }
    }

    s1 = window.Jonas.ReturnInfoAboutObjectOrArrayAsString(objNotOwnAndNotEnumerable);
    s2 = window.Jonas.ReturnInfoAboutObjectOrArrayAsString(objNotOwnAndIsEnumerable);
    s3 = window.Jonas.ReturnInfoAboutObjectOrArrayAsString(objIsOwnAndNotEnumerable);
    s4 = window.Jonas.ReturnInfoAboutObjectOrArrayAsString(objIsOwnAndIsEnumerable);

    if (s1 === "{}")
        s1 = "";
    else
        s1 = s1.substring(1, s1.length - 1);

    if (s2 === "{}")
        s2 = "";
    else
        s2 = s2.substring(1, s2.length - 1);

    if (s3 === "{}")
        s3 = "";
    else
        s3 = s3.substring(1, s3.length - 1);

    if (s4 === "{}")
        s4 = "";
    else
        s4 = s4.substring(1, s4.length - 1);

    str = "<table style='border: 1px solid black; border-collapse: collapse;'><tr style='border: 1px solid black; border-collapse: collapse;'><td style='border: 1px solid black; border-collapse: collapse; min-width: 100px;'><b>Own\\Enumerable</b></td><td style='border: 1px solid black; border-collapse: collapse; min-width: 100px;'><b>No</b></td><td style='border: 1px solid black; border-collapse: collapse; min-width: 100px;'><b>Yes</b></td></tr>";
    str += "<tr style='border: 1px solid black; border-collapse: collapse;'><td style='border: 1px solid black; border-collapse: collapse;'><b>No</b></td><td style='border: 1px solid black; border-collapse: collapse;'>" + s1 + "</td><td style='border: 1px solid black; border-collapse: collapse;'>" + s2 + "</td></tr>";
    str += "<tr style='border: 1px solid black; border-collapse: collapse;'><td style='border: 1px solid black; border-collapse: collapse;'><b>Yes</b></td><td style='border: 1px solid black; border-collapse: collapse;'>" + s3 + "</td><td style='border: 1px solid black; border-collapse: collapse;'>" + s4 + "</td></tr></table>";

    return str;
}

window.Jonas.returnIntegerRandomNumber = function(minIncluded, maxExcluded) {
    var n = minIncluded + Math.round(Math.random() * (maxExcluded - minIncluded));

    if (n === maxExcluded) {
        n = minIncluded;
    }

    return n;
}

window.Jonas.ReturnSample = function (totalSize, sampleSize) {
    var v = [], sample = [];
    var i, index;

    for (i = 0; i < totalSize; i++) {
        v.push(i);
    }

    for (i = 0; i < sampleSize; i++) {
        index = window.Jonas.returnIntegerRandomNumber(0, v.length);
        sample.push(v[index]);
        v.splice(index, 1);
    }

    return sample;
}

window.Jonas.ReturnIdsCommaseparatedFromElementsInjQueryObject = function (obj) {
    var id, n, str;

    n = 0;
    str = "";

    if (obj.length === 0) {
        str = "No elements";
    }
    else {
        obj.each(function () {
            id = $(this).attr("id");

            if (id === undefined) {
                n++;
                id = "UnknownId" + n.toString();
            }

            if (str === "") {
                str += id;
            }
            else {
                str += ", " + id;
            }
        });
    }

    return str;
}