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


window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource = function () {
    var btnSave, btnCancel, textarea;

    btnSave = $("#btnRenderOfHtmlResourceSave");
    btnCancel = $("#btnRenderOfHtmlResourceCancel");
    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");

    textarea = $("#textAreaRenderOfHtmlResource");
    textarea.on("change", window.jonas.textAreaRenderOfHtmlResourceChange);
    textarea.on("mouseup", window.jonas.textAreaRenderOfHtmlResourceResize);
    window.jonas.eventHandlerTextareaRenderOfHtmlResource = 1;
};

window.jonas.turnOffEventHandlersThatMightBeOn = function () {
  if (window.jonas.eventHandlerTextareaDefaultLocation === 1) {
      $("#textareaDefaultLocation").off("change");
      $("#textareaDefaultLocation").off("mouseup");
      window.jonas.eventHandlerTextareaDefaultLocation = 0;
  }

  if (window.jonas.eventHandlerTextareaRenderOfHtmlResource === 1) {
      $("#textAreaRenderOfHtmlResource").off("change");
      $("#textAreaRenderOfHtmlResource").off("mouseup");
      window.jonas.eventHandlerTextareaRenderOfHtmlResource = 0;
  }
};

window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons = function (btnSave, btnCancel) {
    btnSave.prop("disabled", false);
    btnSave.css("background-color", "red");

    btnCancel.prop("disabled", false);
    btnCancel.css("background-color", "red");
};

window.jonas.textAreaRenderOfHtmlResourceChange = function () {

    var textarea;

    textarea = $("#textAreaRenderOfHtmlResource");
    textarea.off("change");
    textarea.off("mouseup");
    window.jonas.eventHandlerTextareaRenderOfHtmlResource = 0;
    window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"));     
};

window.jonas.textAreaRenderOfHtmlResourceResize = function () {
    var textarea, width, height;

    textarea = $("#textAreaRenderOfHtmlResource");
    width = textarea.css("width");
    height = textarea.css("height");

    if (width !== window.jonas.textareaRenderOfHtmlResourceWidth || height !== window.jonas.textareaRenderOfHtmlResourceHeight)
      window.jonas.textAreaRenderOfHtmlResourceChange();
};

window.jonas.registerNewRenderOfHtmlResource = function (previousResource, currentResource, nextResource, widthIframe, heightIframe, widthTextarea, heightTextarea, htmlFile, htmlFileText) {
    var textarea, iframe, btnSave, btnCancel;

    window.jonas.previousResource = previousResource;
    window.jonas.currentResource = currentResource;
    window.jonas.nextResource = nextResource;

    window.jonas.previousResourceInRenderOfResource = previousResource;
    window.jonas.currentResourceInRenderOfResource = currentResource;
    window.jonas.nextResourceInRenderOfResource = nextResource;

    textarea = $("#textAreaRenderOfHtmlResource");
    iframe = $("#iframeRenderOfHtmlResource");
    btnSave = $("#btnRenderOfHtmlResourceSave");
    btnCancel = $("#btnRenderOfHtmlResourceCancel");

    iframe.prop("width", widthIframe);
    iframe.prop("height", heightIframe);
    iframe.prop("src", "http://www.nr1web1.com/" + htmlFile);
    $("#inputWidthHeightIframe").val(widthIframe.toString() + " " + heightIframe.toString());
    window.jonas.htmlFileNameFullPath = htmlFile;

    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");
  
    textarea.css("width", widthTextarea + "px");
    textarea.css("height", heightTextarea + "px");
    textarea.val(htmlFileText);

    window.jonas.textareaRenderOfHtmlResourceWidth = widthTextarea + "px";
    window.jonas.textareaRenderOfHtmlResourceHeight = heightTextarea + "px";

    textarea.on("change", window.jonas.textAreaRenderOfHtmlResourceChange);
    textarea.on("mouseup", window.jonas.textAreaRenderOfHtmlResourceResize);
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

window.jonas.handleTextAreaRenderOfHtmlResourceSave = function () {
    var textarea, width, height, widthHeightTextData, iframe;

    textarea = $("#textAreaRenderOfHtmlResource");
    iframe = $("#iframeRenderOfHtmlResource");

    width = textarea.css("width");
    height = textarea.css("height");

    widthHeightTextData = {
        id: window.jonas.currentResource,
        width: width,
        height: height,
        text: textarea.val()
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/UpdateFileTextAndTextareaDimensionForHtmlResource",
        data: widthHeightTextData,
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                iframe.prop("src", "http://www.nr1web1.com/" + window.jonas.htmlFileNameFullPath);
                window.jonas.textareaRenderOfHtmlResourceWidth = width; 
                window.jonas.textareaRenderOfHtmlResourceHeight = height;
                window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource();
                alert("Textarea saved");
            }
        }
    });
};

window.jonas.handleTextAreaRenderOfHtmlResourceCancel = function () {
    var textarea, iframe;

    textarea = $("#textAreaRenderOfHtmlResource");
    iframe = $("#iframeRenderOfHtmlResource");

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/GetFileTextForHtmlResource",
        data: { id: window.jonas.currentResource},
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                iframe.prop("src", "http://www.nr1web1.com/" + window.jonas.htmlFileNameFullPath);
                textarea.val(data);
            }
        }
    });
   
    textarea.css("width", window.jonas.textareaRenderOfHtmlResourceWidth);
    textarea.css("height", window.jonas.textareaRenderOfHtmlResourceHeight);

    window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource();
};