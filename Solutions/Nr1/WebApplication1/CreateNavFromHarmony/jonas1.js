window.jonas = new Object();

window.jonas.page = 0;
window.jonas.menu = 0;
window.jonas.sub1 = 0;
window.jonas.sub2 = 0;
window.jonas.tab = 0;
window.jonas.icon = "";
window.jonas.maxResourceId = 0;
window.jonas.previousResource = 0;
window.jonas.currentResource = 0;
window.jonas.nextResource = 0;
window.jonas.previousResourceInRenderOfResource = 0;
window.jonas.currentResourceInRenderOfResource = 0;
window.jonas.nextResourceInRenderOfResource = 0;
window.jonas.location = ""; //defaultLocation, nonDefaultLocation, searchResultResources or renderOfResource
window.jonas.modalState = "";
window.jonas.textareaDefaultLocationWidth = ""; //OBS: "px" after width so need to be a string
window.jonas.textareaDefaultLocationHeight = ""; //OBS: "px" after height so need to be a string
window.jonas.eventHandlerTextareaDefaultLocation = 0;
window.jonas.keyWordPhrase = "";
window.jonas.keyWordNote = "";
window.jonas.resourceTitle = "";
window.jonas.resourceKeyWords = "";
window.jonas.resourceNote = "";
window.jonas.resourcePrevious = 0;
window.jonas.resourceNext = 0;
window.jonas.resourceThumbUpLocation = "";
window.jonas.resourceHtmlFile = "";
window.jonas.resourceFiles = "";
window.jonas.resourceLinks = "";
window.jonas.numberOfKeyWords = 0;
window.jonas.textareaRenderOfHtmlResourceWidth = ""; //OBS: "px" after width so need to be a string
window.jonas.textareaRenderOfHtmlResourceHeight = ""; //OBS: "px" after height so need to be a string
window.jonas.htmlFileNameFullPath = ""; 
window.jonas.eventHandlerTextareaRenderOfHtmlResource = 0;
window.jonas.eventHandlerTextareaEditArbitraryTextFile = 0;
window.jonas.currentFileNameFullPathInTextareaEditArbitraryTextFile = "";
window.jonas.idTdBytesInDiary = "";
window.jonas.diaryFileNameFullPath = "";
window.jonas.currentTemplateId = 0;


window.jonas.resetAfterSaveOrCancelOfTextarea = function (btnSave, btnCancel, textarea, eventHandler) {
    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");
  
    if (eventHandler === "RenderOfHtmlResource") {
        textarea.on("change", window.jonas.textareaRenderOfHtmlResourceChange);
        textarea.on("mouseup", window.jonas.textareaRenderOfHtmlResourceResize);
        window.jonas.eventHandlerTextareaRenderOfHtmlResource = 1;
    }
    else if (eventHandler === "EditArbitraryTextFile") {
        $(".ui-dialog-titlebar-close").prop("disabled", false);
        textarea.on("change", window.jonas.textareaEditArbitraryTextFileChange);
        window.jonas.eventHandlerTextareaEditArbitraryTextFile = 1;
    }
    else
        alert("ERROR!! Eventhandler " + eventHandler + " is invalid in function resetAfterSaveOrCancelOfTextarea!");
};

window.jonas.turnOffEventHandlersThatMightBeOn = function () {
  if (window.jonas.eventHandlerTextareaDefaultLocation === 1) {
      $("#textareaDefaultLocation").off("change");
      $("#textareaDefaultLocation").off("mouseup");
      window.jonas.eventHandlerTextareaDefaultLocation = 0;
  }

  if (window.jonas.eventHandlerTextareaRenderOfHtmlResource === 1) {
      $("#textareaRenderOfHtmlResource").off("change");
      $("#textareaRenderOfHtmlResource").off("mouseup");
      window.jonas.eventHandlerTextareaRenderOfHtmlResource = 0;
  }
};

window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons = function (btnSave, btnCancel) {
    btnSave.prop("disabled", false);
    btnSave.css("background-color", "red");

    btnCancel.prop("disabled", false);
    btnCancel.css("background-color", "red");
};

window.jonas.textareaChange = function (btnSave, btnCancel, textarea, eventHandler) {
    textarea.off("change");

    window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons(btnSave, btnCancel);

    if (eventHandler === "RenderOfHtmlResource") {
        textarea.off("mouseup");
        window.jonas.eventHandlerTextareaRenderOfHtmlResource = 0;
    }
    else if (eventHandler === "EditArbitraryTextFile") {
        $(".ui-dialog-titlebar-close").prop("disabled", true);
        window.jonas.eventHandlerTextareaEditArbitraryTextFile = 0;
    }
    else
        alert("ERROR!! Eventhandler " + eventHandler + " is invalid in function textareaChange!");
};

window.jonas.textareaRenderOfHtmlResourceChange = function () {
    window.jonas.textareaChange($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"), $("#textareaRenderOfHtmlResource"), "RenderOfHtmlResource");   
};

window.jonas.textareaEditArbitraryTextFileChange = function () {
    window.jonas.textareaChange($("#btnEditArbitraryTextFileSave"), $("#btnEditArbitraryTextFileCancel"), $("#textareaEditArbitraryTextFile"), "EditArbitraryTextFile"); 
};

window.jonas.textareaRenderOfHtmlResourceResize = function () {
    var textarea, width, height;

    textarea = $("#textareaRenderOfHtmlResource");
    width = textarea.css("width");
    height = textarea.css("height");

    if (width !== window.jonas.textareaRenderOfHtmlResourceWidth || height !== window.jonas.textareaRenderOfHtmlResourceHeight)
        window.jonas.textareaChange($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"), textarea, "RenderOfHtmlResource");
};

window.jonas.registerNewRenderOfHtmlResource = function (previousResource, currentResource, nextResource, widthIframe, heightIframe, widthTextarea, heightTextarea, htmlFile, htmlFileText) {
    var textarea, iframe, link;

    window.jonas.previousResourceInRenderOfResource = previousResource;
    window.jonas.currentResourceInRenderOfResource = currentResource;
    window.jonas.nextResourceInRenderOfResource = nextResource;

    textarea = $("#textareaRenderOfHtmlResource");
    iframe = $("#iframeRenderOfHtmlResource");

    link = "http://www.nr1web1.com/JavaScriptHtml/" + htmlFile.replace("\\", "/");

    iframe.prop("width", widthIframe);
    iframe.prop("height", heightIframe);
    iframe.prop("src", link);
    $("#inputWidthHeightIframe").val(widthIframe.toString() + " " + heightIframe.toString());
    $("#aRenderOfHtmlResource").prop("href", link);
    window.jonas.htmlFileNameFullPath = htmlFile;
  
    textarea.css("width", widthTextarea + "px");
    textarea.css("height", heightTextarea + "px");
    textarea.val(htmlFileText);

    window.jonas.textareaRenderOfHtmlResourceWidth = widthTextarea + "px";
    window.jonas.textareaRenderOfHtmlResourceHeight = heightTextarea + "px";

    textarea.on("change", window.jonas.textareaRenderOfHtmlResourceChange);
    textarea.on("mouseup", window.jonas.textareaRenderOfHtmlResourceResize);
    window.jonas.eventHandlerTextareaRenderOfHtmlResource = 1;

    if (window.jonas.tab !== 0) {
        $("#liTab" + window.jonas.tab).removeClass("active");
        window.jonas.tab = 0;
    }

    window.jonas.setTitleTextInBrowser(window.jonas.page, window.jonas.menu, window.jonas.sub1, window.jonas.sub2, 0, " (Render of resource)");
    window.jonas.updateCssDisplayForContentDivs("renderOfResource");
    window.jonas.checkIconAndSetTitle("book-reader", "Render resource");
    window.jonas.handlePreviousCurrentNextResource(window.jonas.previousResourceInRenderOfResource, window.jonas.currentResourceInRenderOfResource, window.jonas.nextResourceInRenderOfResource);

    if ($("#divRenderOfHtmlResource").css("display") === "none") {
        $("#divRenderOfSelfResource").css("display", "none");
        $("#divRenderOfHtmlResource").css("display", "block");     
    }
};

window.jonas.registerNewRenderOfSelfResource = function (previousResource, currentResource, nextResource) {
    window.jonas.previousResourceInRenderOfResource = previousResource;
    window.jonas.currentResourceInRenderOfResource = currentResource;
    window.jonas.nextResourceInRenderOfResource = nextResource;

    if (window.jonas.tab !== 0) {
        $("#liTab" + window.jonas.tab).removeClass("active");
        window.jonas.tab = 0;
    }

    window.jonas.setTitleTextInBrowser(window.jonas.page, window.jonas.menu, window.jonas.sub1, window.jonas.sub2, 0, " (Render of resource)");
    window.jonas.updateCssDisplayForContentDivs("renderOfResource");
    window.jonas.checkIconAndSetTitle("book-reader", "Render resource");
    window.jonas.handlePreviousCurrentNextResource(window.jonas.previousResourceInRenderOfResource, window.jonas.currentResourceInRenderOfResource, window.jonas.nextResourceInRenderOfResource);

    if ($("#divRenderOfSelfResource").css("display") === "none") {
        $("#divRenderOfHtmlResource").css("display", "none");
        $("#divRenderOfSelfResource").css("display", "block"); 
    }
};

window.jonas.saveIframeDimension = function () {
    var id, htmlResourceIframeDimensionsData, str, v, width, height;

    str = $("#inputWidthHeightIframe").val().trim();
    v = str.split(" ");

    if (v.length === 2) {
        width = window.Number(v[0].trim());
        height = window.Number(v[1].trim());
    }

    if (!width || !height || width < 50 || width > 10000 || height < 50 || height > 10000) {
        alert("ERROR!! Width and height sholud be given blank separated and both in the range [50,10000], for example '300 500'.");
        return;
    }

    htmlResourceIframeDimensionsData = {
        id: window.jonas.currentResource,
        width: width,
        height: height
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/UpdateHtmlResourceIframeDimension",
        data: htmlResourceIframeDimensionsData,
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                $("#iframeRenderOfHtmlResource").prop("width", width);
                $("#iframeRenderOfHtmlResource").prop("height", height);
            }
        }
    });
};

window.jonas.textareaSave = function (id, currentFileName, btnSave, btnCancel, iframe, textarea, eventHandler) {
    var str, width, height, saveFileTextData, link;

    if (id)
        str = window.jonas.currentResource.toString();
    else
        str = currentFileName;

    if (id) {
        width = textarea.css("width");
        height = textarea.css("height");
    }
    else {
        width = 0;
        height = 0;
    }

    saveFileTextData = {
        str: str,
        width: width,
        height: height,
        text: textarea.val()
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/SaveFileText",
        data: saveFileTextData,
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {

                if (id) {
                    link = "http://www.nr1web1.com/JavaScriptHtml/" + window.jonas.htmlFileNameFullPath.replace("\\", "/");
                    iframe.prop("src", link);
                    window.jonas.textareaRenderOfHtmlResourceWidth = width;
                    window.jonas.textareaRenderOfHtmlResourceHeight = height;
                }

                window.jonas.resetAfterSaveOrCancelOfTextarea(btnSave, btnCancel, textarea, eventHandler);
                alert("Textarea saved");
            }
        }
    });
};

window.jonas.textareaCancel = function (id, currentFileName, btnSave, btnCancel, iframe, textarea, eventHandler) {
    var str;

    if (id)
        str = window.jonas.currentResource.toString();
    else
        str = currentFileName;
    
    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetFileText",
        data: { str: str},
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                if (iframe)
                    iframe.prop("src", "http://www.nr1web1.com/" + window.jonas.htmlFileNameFullPath);

                textarea.val(data);
            }
        }
    });

    if (id) {
        textarea.css("width", window.jonas.textareaRenderOfHtmlResourceWidth);
        textarea.css("height", window.jonas.textareaRenderOfHtmlResourceHeight);
    }

    window.jonas.resetAfterSaveOrCancelOfTextarea(btnSave, btnCancel, textarea, eventHandler);
};

window.jonas.editTextFile = function(fileNameFullPath) {
    var textarea;

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetFileText",
        data: { str: fileNameFullPath },
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                textarea = $("#textareaEditArbitraryTextFile");
                window.jonas.currentFileNameFullPathInTextareaEditArbitraryTextFile = fileNameFullPath;
                textarea.val(data);
                textarea.on("change", window.jonas.textareaEditArbitraryTextFileChange);
                window.jonas.eventHandlerTextareaEditArbitraryTextFile = 1;
                window.jonas.openModal("#dialogEditArbitraryTextFile", fileNameFullPath, 1200, 700, "EditArbitraryTextFile");
            }
        }
    });
};

window.jonas.addNewWorkDay = function (diaryFolder) {
    var str, id;

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/AddNewWorkDay",
        data: { diaryFolder: diaryFolder.replace(/##/g, "\\") },
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else { //Success, an object of type DayDateDiaryBytesInDiary expected back from server
                str = "<a class='aGreen' href=\"javascript: window.jonas.openModalToEditDiaryDay('AAAAA', 'BBBBB')\">" + data.Diary + "</a>";
                str = str.replace("AAAAA", diaryFolder + "##" + data.Diary);
                id = "tdBytesInDiary" + data.Day.toString();
                str = str.replace("BBBBB", id);
                $("#headerRowTableDayDateDiaryBytesInDiary").after("<tr><td>" + data.Day + "</td><td>" + data.Date + "</td><td>" + str + "</td><td id='" + id + "'>" + data.BytesInDiary + "</td></tr>");
                $("#divAddNewWorkDay").hide();

                if (data.WarningMessage)
                    alert(data.WarningMessage);
            }
        }
    });
};

window.jonas.openModalToEditDiaryDay = function (diaryFileNameFullPath, idTdBytesInDiary) {
    diaryFileNameFullPath = diaryFileNameFullPath.replace(/##/g, "\\");
    window.jonas.idTdBytesInDiary = idTdBytesInDiary;
    window.jonas.diaryFileNameFullPath = diaryFileNameFullPath;
    window.jonas.editTextFile(diaryFileNameFullPath);
};

window.jonas.updateBytesInDiaryAndDiaerWarningMessage = function () {
    var divWarningMessage;

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetBytesInDiaryWarningMessage",
        data: { diaryFileNameFullPath: window.jonas.diaryFileNameFullPath },
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else { //Success, an object of type BytesInDiaryWarningMessage expected back from server
                $("#" + window.jonas.idTdBytesInDiary).text(data.BytesInDiary);

                divWarningMessage = $("#warningMessageDayDateDiaryBytesInDiary");

                if (data.WarningMessage && (divWarningMessage.css("display") === "none")) {
                    divWarningMessage.show();
                }
                else if (!data.WarningMessage && (divWarningMessage.css("display") === "block")) {
                    divWarningMessage.hide();
                }

                if (data.WarningMessage)
                    divWarningMessage.text(data.WarningMessage);

                window.jonas.idTdBytesInDiary = "";
                window.jonas.diaryFileNameFullPath = "";
            }
        }
    });
};

window.jonas.FillDivAdhocCodeKeyWords = function () {
    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetKeyWords",
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "get",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                window.jonas.addCheckboxes($("#divAdhocCodeKeyWords"), 10, "adhocCodeKeyWord", data);
            }
        }
    });
};

window.jonas.FillDivAdhocCodeTemplates = function () {
    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetAdhocTemplates",
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "get",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                window.jonas.addRadiobuttons($("#divAdhocCodeTemplates"), 10, "adhocCodeTemplate", "adhocCode", "onchange=window.jonas.getNewAdhocTemplate(this.id)", data);
            }
        }
    });
};

window.jonas.getNewAdhocTemplate = function (id) {
    var currentTemplateId;

    currentTemplateId = window.Number(id.substring(17));

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetNewAdhocTemplate",
        data: { id: currentTemplateId }, //17=Length of prefix adhocCodeTemplate
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "get",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                window.jonas.updateCheckboxesCheckedStatus($("#divAdhocCodeKeyWords"), 16, data.KeyWords.split(","));
                $("#textareaAdhocCode").val(data.Text);
                window.jonas.currentTemplateId = currentTemplateId;
            }
        }
    });
};

window.jonas.RunAdhoc = function () {
    var title, keyWords, note, keyWordIdsCommaSeparated, codeText, templateData;

    title = $("#inputAdhocCodeTitle").val().trim();
    keyWords = $("input[type='checkbox']:checked", "#divAdhocCodeKeyWords");
    note = $("#inputAdhocCodeNote").val().trim(); 
    codeText = $("#textareaAdhocCode").val().trim();

    if (!title) {
        alert("Title must be given!");
        return;
    }

    if (keyWords.length === 0) {
        alert("At least one key word must be given!");
        return;
    }

    keyWordIdsCommaSeparated = "";

    for (i = 0; i < keyWords.length; i++) {
        if (keyWords[i].checked && keyWordIdsCommaSeparated === "") {
            keyWordIdsCommaSeparated = keyWords[i].id.substring(16); //16=Length of prefix adhocCodeKeyWord
        }
        else if (keyWords[i].checked && keyWordIdsCommaSeparated !== "") {
            keyWordIdsCommaSeparated += ("," + keyWords[i].id.substring(16)); //16=Length of prefix adhocCodeKeyWord
        }
    }

    templateData = {
        id: window.jonas.currentTemplateId,
        title: title,
        keyWords: keyWordIdsCommaSeparated,
        note: note,
        codeText: codeText
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/AddAdhocResource",
        data: { templateData: templateData },
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                window.jonas.maxResourceId = data;

                if (window.jonas.currentTemplateId === 1) {
                    alert("Resource " + data + " was successfully created.");
                    $("#inputAdhocCodeTitle").val("");
                    $("#inputAdhocCodeNote").val();
                    $("#textareaAdhocCode").val("");
                }
                else {
                    window.jonas.renderResource(data, false);
                }
            }
        }
    });
};