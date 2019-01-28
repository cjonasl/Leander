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


window.jonas.resetAfterSaveOrCancelOfTextarea = function (btnSave, btnCancel, textarea, eventHandler) {
    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");
  
    if (eventHandler === "RenderOfHtmlResource") {
        textarea.on("mouseup", window.jonas.textareaRenderOfHtmlResourceResize);
        window.jonas.eventHandlerTextareaRenderOfHtmlResource = 1;
    }
    else if (eventHandler === "EditArbitraryTextFile") {
        $(".ui-dialog-titlebar-close").prop("disabled", false);
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
    var width, height;

    width = textarea.css("width");
    height = textarea.css("height");

    if (width !== window.jonas.textareaRenderOfHtmlResourceWidth || height !== window.jonas.textareaRenderOfHtmlResourceHeight)
        window.jonas.textareaChange($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"), $("#textareaRenderOfHtmlResource"), "RenderOfHtmlResource"); 
};

window.jonas.registerNewRenderOfHtmlResource = function (previousResource, currentResource, nextResource, widthIframe, heightIframe, widthTextarea, heightTextarea, htmlFile, htmlFileText) {
    var textarea, iframe;

    window.jonas.previousResource = previousResource;
    window.jonas.currentResource = currentResource;
    window.jonas.nextResource = nextResource;

    window.jonas.previousResourceInRenderOfResource = previousResource;
    window.jonas.currentResourceInRenderOfResource = currentResource;
    window.jonas.nextResourceInRenderOfResource = nextResource;

    textarea = $("#textareaRenderOfHtmlResource");
    iframe = $("#iframeRenderOfHtmlResource");

    iframe.prop("width", widthIframe);
    iframe.prop("height", heightIframe);
    iframe.prop("src", "http://www.nr1web1.com/" + htmlFile);
    $("#inputWidthHeightIframe").val(widthIframe.toString() + " " + heightIframe.toString());
    $("#aRenderOfHtmlResource").prop("href", "http://www.nr1web1.com/" + htmlFile);
    window.jonas.htmlFileNameFullPath = htmlFile;
  
    textarea.css("width", widthTextarea + "px");
    textarea.css("height", heightTextarea + "px");
    textarea.val(htmlFileText);

    window.jonas.textareaRenderOfHtmlResourceWidth = widthTextarea + "px";
    window.jonas.textareaRenderOfHtmlResourceHeight = heightTextarea + "px";

    textarea.on("change", window.jonas.textareaRenderOfHtmlResourceChange);
    textarea.on("mouseup", window.jonas.textareaRenderOfHtmlResourceResize);
    window.jonas.eventHandlerTextareaRenderOfHtmlResource = 1;

    if (window.jonas.location !== "searchResultResources") {
        $("#liTab" + window.jonas.tab).removeClass("active");
        window.jonas.tab = 0;
    }

    window.jonas.setTitleTextInBrowser(window.jonas.page, window.jonas.menu, window.jonas.sub1, window.jonas.sub2, 0, " (Render of resource)");
    window.jonas.updateCssDisplayForContentDivs("renderOfResource");
    window.jonas.checkIconAndSetTitle("book-reader", "Render resource");
    window.jonas.handlePreviousCurrentNextResource(window.jonas.previousResourceInRenderOfResource, window.jonas.currentResourceInRenderOfResource, window.jonas.nextResourceInRenderOfResource);

    if ($("#divRenderOfHtmlResource").css("display") === "none") {
        $("#divRenderOfHtmlResource").css("display", "block");
        $("#divRenderOfSelfResource").css("display", "none");
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
    var str, width, height, widthHeightTextData;

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

    widthHeightTextData = {
        str: str,
        width: width,
        height: height,
        text: textarea.val()
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/SaveFileText",
        data: widthHeightTextData,
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {

                if (id) {
                    iframe.prop("src", "http://www.nr1web1.com/" + window.jonas.htmlFileNameFullPath);
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
                window.jonas.openModal("#dialogEditArbitraryTextFile", fileNameFullPath, 900, 500, "EditArbitraryTextFile");
            }
        }
    });
};