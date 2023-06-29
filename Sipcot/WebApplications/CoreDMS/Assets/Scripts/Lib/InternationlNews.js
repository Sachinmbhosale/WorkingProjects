var myImg = new Array()
var myHeadings = new Array()
var mynewsbody = new Array()
var readmorelink = new Array()
var imageObj = new Image();

myImgEnd = ""

var d = 0;

var request = false;
function GetTradeNews() {
    try {
        request = new XMLHttpRequest();
    } catch (trymicrosoft) {
        try {
            request = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (othermicrosoft) {
            try {
                request = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (failed) {
                request = false;
            }
        }
    }

    if (!request)
        alert("Error initializing XMLHttpRequest!");
    else
        getTradeNewsInfo('+')
}

function getTradeNewsInfo(val) {
    var url = window.location.href;
    var urlArray = url.split('//');
    var urlHost = urlArray[1].split('/');
    
    url = urlArray[0] + "//" + urlHost[0] + "/AjaxTradeNews.aspx?";
    var params = "param=";
    if (val == '-') {
        params = params + "previous"
    }
    else {
        params = params + "next"
    }
    request.open("POST", url + params, true);
    request.onreadystatechange = updatePage;
    request.send(params);
}

function updatePage() {
    if (request.readyState == 4) {
        if (request.status == 200) {
            var response = request.responseText
            document.getElementById('tdTradeNews').innerHTML = response;
        } else
            document.getElementById('tdTradeNews').innerHTML = 'Error in getting Trade News';
    }
}

function loadImg() {
    for (idx = 0; idx < myImg.length; idx++) {
        imageObj.src = myImg[idx];
    }
    document.Form1.imgSrc.src = myImg[0];
    document.getElementById('newsbody').innerHTML = mynewsbody[0];
    document.getElementById('headlines').innerHTML = myHeadings[0];
    document.getElementById('readmore').href = readmorelink[0];
    var intervalId = window.setInterval("next()", 10000);
}

function prev() {
    try {
        request = new XMLHttpRequest();
    } catch (trymicrosoft) {
        try {
            request = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (othermicrosoft) {
            try {
                request = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (failed) {
                request = false;
            }
        }
    }
    if (!request)
        alert("Error initializing XMLHttpRequest!");
    else
        getTradeNewsInfo('-')
}

function next() {
    GetTradeNews('+');    
}