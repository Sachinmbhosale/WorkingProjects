
var varType = null;
var varUrl = null;
var varData = null;
var varContentType = null;
var varDataType = null;
var varProcessData = null;
var browser = "";
var status = false; //used to return true or false after a function call

//********************************************************
//ValidateInputData Function returns true or false with message to user on contorl specified
//********************************************************

function ValidateInputData(control, action, params) {
    if (action == "ValidateUser") {
        return true;
    }
}

////********************************************************
////GetScalar Function with Successs and Failur Rutine
////********************************************************

//function CallGetScalar(control, action, params) {
//    var status = false;
//    if (jQuery.browser.mozilla == true) browser = "firefox"
//    else if (jQuery.browser.msie == true) browser = "ie"

//    varType = "GET";
//    varUrl = "WCF/CoreServices.svc/GetScalar";
//    varData = '{"action":"' + action + '","methodparams":"' + params + '","browser":"' + browser + '"}';
//    varContentType = "application/json; charset=utf-8";
//    varDataType = "json";
//    varProcessData = true;
//    //CallService
//    $.ajax({
//        type: varType, //GET or POST or PUT or DELETE verb
//        url: varUrl, // Location of the service
//        data: varData, //Data sent to server
//        contentType: varContentType, // content type sent to server
//        dataType: varDataType, //Expected data format from server
//        processdata: varProcessData, //True or False
//        success: function(msg) {//On Successfull service call
//            GetScalarSucceeded(control, action, msg);
//        },
//        error: GetScalarServiceFailed// When Service call fails
//    });

//    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
//    var browser = "";

//    return status;
//}

//function GetScalarSucceeded(control, action, result) {//When service call is sucessful
//    var resultObject = null;
//    if (varDataType == "json") {
//        resultObject = result;
//    }
//}
//function GetScalarServiceFailed(result) {
//    alert('Service call failed: ' + result.status + '' + result.statusText);
//    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
//}

//********************************************************
//PostScalar Function with Successs and Failur Rutine
//********************************************************

function CallPostScalar(control, orgId, guid, action, params) {
    //reset variables
    var browser = "";
    status = false; 
    //check browser
    if (jQuery.browser.mozilla == true) browser = "firefox"
    else if (jQuery.browser.msie == true) browser = "ie"

    varType = "POST";
    varUrl = "WCF/CoreServices.svc/PostScalar";
    //We are passing multiple paramers to the service in json format 
    //varData = '{"action": "' + action + '","methodParams": "' + params + '","browser": "' + browser + '"}';
    varData = '{"orgId": "' + orgId + '","guid": "' + guid + '","action": "' + action + '","methodParams": "' + params + '","browser": "' + browser + '"}';   
    varContentType = "application/json; charset=utf-8";
    varDataType = "json";
    varProcessData = true;
    //CallService
    $.ajax({
        type: varType, //GET or POST or PUT or DELETE verb
        url: varUrl, // Location of the service
        data: varData, //Data sent to server
        contentType: varContentType, // content type sent to server
        dataType: varDataType, //Expected data format from server
        processdata: varProcessData, //True or False
        success: function(msg) {//On Successfull service call
            PostScalarSucceeded(control, action, msg);
        },
        error: PostScalarServiceFailed// When Service call fails
    });    
}

function PostScalarSucceeded(control, action, result) {//When service call is sucessful
    var resultObject = null;
    status = true;
    if (varDataType == "json") {
        // Constructed object name will be object.[ServiceName]Result
        if (action = "ValidateUser") {
            resultObject = result.PostScalarResult;
        }
    }
    if (resultObject != null) {
        if (resultObject.status = "0") {
            $(control).html(resultObject.message);
        }
        else {
            $(control).html("");
        }
    }
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}


function PostScalarServiceFailed(result) {
    status = false;
    alert('Service call failed: ' + result.status + '' + result.statusText);
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}

//********************************************************
//PostDataCollection Function with Successs and Failur Rutine
//********************************************************

function CallPostDataCollection(control, action, params) {
    var status = false;
    if (jQuery.browser.mozilla == true) browser = "firefox"
    else if (jQuery.browser.msie == true) browser = "ie"

    varType = "POST";
    varUrl = "WCF/CCoreServices.svc/PostDataCollection";
    varData = '{"action": "' + action + '","methodparams": "' + params + '","browser": "' + browser + '"}';
    varContentType = "application/json; charset=utf-8";
    varDataType = "json";
    varProcessData = true;
    //CallService
    $.ajax({
        type: varType, //GET or POST or PUT or DELETE verb
        url: varUrl, // Location of the service
        data: varData, //Data sent to server
        contentType: varContentType, // content type sent to server
        dataType: varDataType, //Expected data format from server
        processdata: varProcessData, //True or False
        success: function(msg) {//On Successfull service call
            PostDataCollectionSucceeded(control, action, msg);
        },
        error: PostDataCollectionServiceFailed// When Service call fails
    });   

    return status;
}

function PostDataCollectionSucceeded(control, action, result) {//When service call is sucessful
    var resultObject = null;
    if (varDataType == "json") {
        //WCF Service with multiple output paramaetrs 
        resultObject = result.PostDataCollectionResult.User;
        //resultObject = result.GetScalarResult.ProvinceInfo;           
    }

    for (i = 0; i < resultObject.length; i++) {
        var opt = document.createElement("option"); opt.text = resultObject[i];
        ProvinceDDL.options.add(opt)
    }
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";

}
function GetScalarServiceFailed(result) {
    alert('Service call failed: ' + result.status + '' + result.statusText);
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}
