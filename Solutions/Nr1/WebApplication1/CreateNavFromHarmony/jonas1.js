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
window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource = 0;
window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource = 0;


window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource = function () {
    var btnSave, btnCancel, textarea;

    btnSave = $("#btnRenderOfHtmlResourceSave");
    btnCancel = $("#btnRenderOfHtmlResourceCancel");
    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");

    textarea = $("#textAreaRenderOfHtmlResource");

    if (window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource === 0) {
        textarea.on("change", window.jonas.textAreaRenderOfHtmlResourceChange);
        window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource = 1;
    }

    if (window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource === 0) {
        textarea.on("mouseup", window.jonas.textAreaRenderOfHtmlResourceResize);
        window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource = 1;
    }
};

window.jonas.turnOffEventHandlersThatMightBeOn = function () {
  if (window.jonas.eventHandlerTextareaDefaultLocation === 1) {
      $("#textareaDefaultLocation").off("change");
      $("#textareaDefaultLocation").off("mouseup");
      window.jonas.eventHandlerTextareaDefaultLocation = 0;
  }

  if (window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource === 1) {
      $("#textareaDefaultLocation").off("change");
      window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource = 0;
  }

  if (window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource === 1) {
      $("#textareaDefaultLocation").off("mouseup");
      window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource = 0;
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
    window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource = 0;

    if (window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource === 1)
        window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"));
};

window.jonas.textAreaRenderOfHtmlResourceResize = function () {
    var textarea, width, height;

    textarea = $("#textAreaRenderOfHtmlResource");
    width = textarea.css("width");
    height = textarea.css("height");

    if (width !== textAreaWidth !== window.jonas.textareaRenderOfHtmlResourceWidth || height !== window.jonas.textareaRenderOfHtmlResourceHeight) {
        textarea.off("mouseup");
        window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource = 0;

        if (window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource === 1)
          window.jonas.setBackgroundRedAndSetDisabledToFalseForSaveAndCancelButtons($("#btnRenderOfHtmlResourceSave"), $("#btnRenderOfHtmlResourceCancel"));
    }
};

window.jonas.registerNewRenderOfHtmlResource = function (previousResource, currentResource, nextResource, widthIframe, heightIframe, widthTextarea, heightTextarea, text) {
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

    btnSave.prop("disabled", true);
    btnSave.css("background-color", "green");
    btnCancel.prop("disabled", true);
    btnCancel.css("background-color", "green");
  
    textarea.css("width", widthTextarea + "px");
    textarea.css("height", heightTextarea + "px");
    textarea.val(text);

    window.jonas.textareaRenderOfHtmlResourceWidth = widthTextarea;
    window.jonas.textareaRenderOfHtmlResourceHeight = heightTextarea;

    textarea.on("change", window.jonas.textAreaRenderOfHtmlResourceChange);
    window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource = 1;

    textarea.on("mouseup", window.jonas.textAreaRenderOfHtmlResourceResize);
    window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource = 1;

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

    if (!width || !height || width < 50 || width > 1000 || height < 50 || height > 1000) {
        alert("ERROR!! Width and height sholud be given blank separated and both in the range [50,1000], for example '300 500'.");
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
    var textarea, str, htmlResourceFileTextAndTextareaDimension, fileText = null, index, width = 0, height = 0;

    textarea = $("#textAreaRenderOfHtmlResource");

    if (window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource === 0)
        fileText = textarea.val();

    if (window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource === 0) {
        str = textarea.css("width");
        index = str.indexOf("px");
        width = window.Number(str.substring(0, index));

        str = textarea.css("height");
        index = str.indexOf("px");
        height = window.Number(str.substring(0, index));
    }

    htmlResourceFileTextAndTextareaDimension = {
        id: window.jonas.currentResource,
        fileText: fileText,
        width: width,
        height: height
    };

    $.ajax({
        url: "http://www.Nr1Web1.com/Main/UpdateFileTexAndTextareaDimensiontForHtmlResource",
        data: htmlResourceFileTextAndTextareaDimension,
        error: function (data) { alert("An error happened! Error message: " + data.responseText); console.log(data); },
        method: "post",
        success: function (data) {
            if ((typeof data === "string") && (data.length >= 5) && (data.substring(0, 5) === "ERROR")) {
                alert(data);
                return;
            }
            else {
                window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource();
                alert("Textarea saved");
            }
        }
    });
};

window.jonas.handleTextAreaRenderOfHtmlResourceCancel = function () {
    var textarea;

    textarea = $("#textAreaRenderOfHtmlResource");

    if (window.jonas.eventHandlerChangeTextareaRenderOfHtmlResource === 0) {
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
                    textarea.val(data);
                }
            }
        });
    }

    if (window.jonas.eventHandlerMouseupTextareaRenderOfHtmlResource === 0) {
        textarea.css("width", window.jonas.textareaRenderOfHtmlResourceWidth);
        textarea.css("height", window.jonas.textareaRenderOfHtmlResourceHeight);
    }

    window.jonas.resetAfterSaveOrCancelOfTextareaForHtmlResource();
};