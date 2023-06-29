/* START - Include TOBOC.js */

function menuon(x) {
    x.style.background = "#C4C4C4";
    x.style.border = "1px solid #514C85";
}

function menuoff(x) {
    x.style.background = "#ffffff";
    x.style.border = "1px solid #a2a2a2";
}

function menuonmessagecenter(x) {
    //C4C4C4
    //x.style.background-image: url(/images/messagecenter/Orange-BG.jpg);
    x.style.background = '#F5C159';
    //x.style.border = "1px solid #514C85";
}
function menuoffmessagecenter(x) {
    //x.style.background-image: url(/images/HeaderBG.jpg);
    x.style.background = '';
    //x.style.border = "1px solid #e2e2e2";
}

function menuoffheader(x) {
    x.style.backgroundColor = "#ffffff";
    x.style.border = "1px solid #000000";
}

function CloseWindow() {
    self.close();
}

function CloseandRefresh() {
    opener.location.reload();
    self.close();
}

function CloseandRedirect(location) {
    opener.location.href = location;
    self.close();
}

function Confirm() {
    alert('Are You Sure?');
}

function CannotDelete() {
    alert('You may not delete a category if it contains pictures. Please delete all the pictures first.');
}

function confirm_delete() {
    if (confirm("Are you sure?") == true)
        return true;
    else
        return false;
}

//Show / Hide JS Functions
function showhide(layer_ref) {
    hza = document.getElementById(layer_ref);
    if (hza.style.display == 'block') {
        hza.style.display = 'none';
    }
    else {
        hza.style.display = 'block';
    }
}

function setHomePageLink() {
    /*<!-- based on script from http://javascript.internet.com/page-details/set-homepage-link.html -->*/
    if (document.all) {
        document.write('<a  rel="nofollow" href=\"#\" onClick="this.style.behavior=\'url(#default#homepage)\';this.setHomePage(\'http://www.toboc.com\');" title=\" IE users: Click here to make TOBOC.com your browser Start Page. (Opera: > Navigation > Set Home Page) \">');
        document.write('Make us your Homepage</a>');
    }
    // If it's Netscape 6, tell user to drag link onto Home button
    else if (document.getElementById) {
        document.write('<a  rel="nofollow" style=\'cursor:help\' href="http://www.toboc.com" title=\" Make TOBOC.com your browser Start Page: drag this link onto the browser top-left Home graphic button \" onClick=\'return false\'>Make us your Homepage</a>');
    }
    // If it's Netscape 4 or lower, give instructions to set Home Page
    else if (document.layers) {
        document.write('<span  title=\' Make TOBOC.com your browser Start Page: - Go to Preferences in the Edit Menu. Choose Navigator from the list on the left. Click on the Use Current Page button. \'>Make us your Homepage</span>');
    }
    // If it's any other browser, for which I don't know the specifications of home paging, display instructions
    else {
        document.write('<span  title=\' Make TOBOC.com your browser Start Page: - Go to Preferences in the Edit Menu. Choose Navigator from the list on the left. Click on the Use Current Page button. \'>Make us your Homepage</span>');
    }
}

var isIE = false; var navVer = navigator.appVersion; var ver = parseFloat(navVer);
var IEPos = navVer.indexOf('MSIE');
if (IEPos != -1) { isIE = true; ver = parseFloat(navVer.substring(IEPos + 5, navVer.indexOf(';', IEPos))); }
var isIE5up = (isIE && ver >= 5);

function addBookmark(bookmarkurl, bookmarktitle) {
    if (isIE5up) {
        window.external.AddFavorite(bookmarkurl, bookmarktitle);
    }
    else if (navigator.appName == "Netscape") {
        alert("Please press (CTRL-D) to bookmark this page.");
    }
    else {
        alert("Sorry! Your browser doesn't support this function.");
    }
}

function Closebtn(x) {

    document.getElementById(x).style.display = "none";
    return false;

}

function CompanyDescValidation(x) {

    var result = true;
    var quote;
    //            alert(x);
    var s = document.getElementById(x).value;
    quote = s.split(" ");

    //for (x = 0; x < quote.length; x++) {
    //  if (quote[x].length > 20) {
    //    result = false;

    //}
    //} 

    return result;
}


/* END - Include TOBOC.js */

/* START - Include Sectors.js */


function ListBox1_onChange(list, h, hidSector) {
    //    var listBox1 = document.getElementById('subSector');
    ////    var listBox2 = document.getElementById('ListBox2');

    //    for (var i = 0; i < listBox1.options.length; i++) {
    //        if (listBox1.options[i].selected) {
    //            var newOption = window.document.createElement('OPTION');
    //            newOption.text = listBox1.options[i].text;
    //            newOption.value = listBox1.options[i].value;

    //            listBox2.options.add(newOption);
    //            listBox1.options.remove(i);
    //        }
    //    }

    persistOptionsList(list, h, hidSector);
}

function persistOptionsList(list, h, hidSector) {

    var listBox1 = document.getElementById(list);
    var hidSector1 = document.getElementById(hidSector);
    //  alert("Try to Hold The state");
    var optionsList = '';
    for (var i = 0; i < listBox1.options.length; i++) {
        var optionText = listBox1.options[i].text;
        var optionValue = listBox1.options[i].value;

        if (optionsList.length > 0)
            optionsList += ';';

        optionsList += optionText + ':' + optionValue;
    }
    document.getElementById(h).value = optionsList;
    document.getElementById(hidSector).innerHTML = hidSector1.innerHTML;
}
function delItemFromList(z, h, hidSector) {

    var list = document.getElementById(z);
    var hidSector1 = document.getElementById(hidSector).innerHTML;

    // Delete has been pressed
    if (list.selectedIndex >= 0) {
        for (x = list.selectedIndex; x + 1 < list.options.length; x++) {
            list.options[x].text = list.options[x + 1].text
            list.options[x].value = list.options[x + 1].value
        }
        --list.options.length;
        document.getElementById(hidSector).innerHTML = ++hidSector1;
    }
    else {
        alert("There is no selection to delete,Please select a sector");

    }
    //        checkSubsectors(z);

    persistOptionsList(z, h, hidSector);
    return false;
}
function checkSubsectors(z) {
    for (x = 0; x < document.getElementById(z).length; x++) {
        document.getElementById(z).options[x].checked = false;
    }
}


function checkSubsectors(z) {
    for (x = 0; x < document.getElementById(z).length; x++) {
        document.getElementById(z).options[x].checked = false;
    }
}

/* END - Include Sectors.js */

/* START - Include Fin.js */

//Financial Info Script
function CR() {
    var a = window.open('../../Information.htm#Fin1', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function QR() {
    var a = window.open('../../Information.htm#Fin1', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function WCTR() {
    var a = window.open('../../Information.htm#Fin2', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function ATR() {
    var a = window.open('../../Information.htm#Fin2', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function ITR() {

    var a = window.open('../../Information.htm#Fin3', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function DTER() {
    var a = window.open('../../Information.htm#Fin3', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function A() {
    var a = window.open('../../Information.htm#Term', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function L() {

    var a = window.open('../../Information.htm#Term', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function Others() {
    var a = window.open('/Information.htm#Term', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
function Testing() {
    var a = window.open('/Information.htm#Fin1', 'FINANCIAL', 'toolbars=no,scrollbars=yes,resizable=no,width=900px,height=600px');
}
/* END - Include Fin.js */

/*  Product Scroll - HP Random Products*/
var att1, att2, att3, att4;
var anim1, anim2, anim3, anim4;
var cs = 1, t;
var Incr = 1;
var isInit = 0;
var error = 0;
//Updated By Ramesh Vagh - Resolved Bug #1910 - Featured product images list is not rotating
function InitAnimation() {    
//Updated by Ramesh Vagh : Resolved YAHOO utility related error.
    try {
        att1 = {
            scroll: { to: [0, 0] }
        };
        att2 = {
            scroll: { to: [0, 157] }
        };
        att3 = {
            scroll: { to: [0, 315] }
        };
        att4 = {
            scroll: { to: [0, 480] }
        };

        anim1 = new YAHOO.util.Scroll('banner', att1);
        anim2 = new YAHOO.util.Scroll('banner', att2);
        anim3 = new YAHOO.util.Scroll('banner', att3);
        anim4 = new YAHOO.util.Scroll('banner', att4);

        YAHOO.util.Event.on('pagi1', 'click', function () {
            anim1.animate();
            cs = 1;
        });
        YAHOO.util.Event.on('pagi2', 'click', function () {
            anim2.animate();
            cs = 2;
        });
        YAHOO.util.Event.on('pagi3', 'click', function () {
            anim3.animate();
            cs = 3;
        });
        YAHOO.util.Event.on('pagi4', 'click', function () {
            anim4.animate();
            cs = 4;
        });
        error = 0;
    } catch (err) {error = 1; }
    finally { return; }
}

function Rotate() {   
    if (isInit == 0)
        InitAnimation();

    //Updated by Ramesh Vagh : Resolved YAHOO utility related error.
    if (error == 1)
        return;
    switch (cs) {
        case 1:
            anim1.animate();
            break;
        case 2:
            anim2.animate();
            break;
        case 3:
            anim3.animate();
            break;
        case 4:
            anim4.animate();
            break;
    }
    changepagi(cs);
    //--Commented by Raghu on 9:45 AM 5/24/2011 for Bugzilla Issue # 925
    //if (cs >= 4) cs = 0;
    //cs++;
    //-----

    //-----New Code by Raghu on 9:45 AM 5/24/2011 for Bugzilla Issue # 925
    if (cs >= 4) {
        Incr = 0;
    }
    else if (cs <= 1) {
        Incr = 1;
    }
    if (Incr == 1) {
        cs++
    }
    else {
        cs--
    }
    if (cs == -1 || cs == 0) {
        cs = 1
    }
    //--------------
    t = setTimeout("Rotate()", 3000);
}
//Update End

Rotate();
function stopScrollTLImg() {
    clearTimeout(t);
}

function startScrollTLImg() {
    t = setTimeout("Rotate()", 3000);
}

function changepagi(d) {
    var curpagi;
    for (i = 1; i <= 4; i++) {
        curpagi = "pagi" + i;
        var ctrl = document.getElementById(curpagi);
        if (ctrl == null)
            return;
        ctrl.className = "inactiveli";
    }
    document.getElementById("pagi" + d).className = "activeli";
}

/*  END - Product Scroll - HP Random Products*/

/* START - Include InternationalNews.js*/
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
    if (document.getElementById('tdTradeNews') == null)
        return;
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

/* END - Include InternationalNews.js*/

//Added By Ramesh Vagh : Based on the membership level user can see the company uploads in search resuilt page
function CompanyUploadViewConfirm(item, membershiplevel) {
    var url = window.location.href;
    var urlArray = url.split('//');
    var urlHost = urlArray[1].split('/');
    url = urlArray[0] + "//" + urlHost[0] + "/ProfileRegistration.aspx";

    var msg = "Please register your profile to view '" + item + "' of this company.";
    if (membershiplevel == "0") {
        msg = "Please complete your profile to view '" + item + "' of this company.";
        url = urlArray[0] + "//" + urlHost[0] + "/controlpanel.aspx";
    }
    else if (membershiplevel == "1") {
        msg = "Please upgrade your profile to view '" + item + "' of this company.";
        url = urlArray[0] + "//" + urlHost[0] + "/TradePlusMemberUpgrade.aspx";
    }
    var r = confirm(msg);
    if (r == true)
        window.location = url;
}
function SetRecurringText() {


    if (document.getElementById('<%=ckbRecurringPaymeny.ClientID%>').checked) {

        ShoppingCart_ProfileOptions_DataGrid1_ctl02_Label4.innerHTML = ShoppingCart_ProfileOptions_DataGrid1_ctl02_Label4.innerHTML + ' Recurring'
        ShoppingCart_ProfileOptions_DataGrid1_ctl03_Label4.innerHTML = ShoppingCart_ProfileOptions_DataGrid1_ctl03_Label4.innerHTML + ' Recurring'
        ShoppingCart_ProfileOptions_DataGrid1_ctl04_Label4.innerHTML = ShoppingCart_ProfileOptions_DataGrid1_ctl04_Label4.innerHTML + ' Recurring'
        ShoppingCart_ProfileOptions_DataGrid1_ctl05_Label4.innerHTML = ShoppingCart_ProfileOptions_DataGrid1_ctl05_Label4.innerHTML + ' Recurring'
        ShoppingCart_ProfileOptions_DataGrid1_ctl06_Label4.innerHTML = ShoppingCart_ProfileOptions_DataGrid1_ctl06_Label4.innerHTML + ' Recurring'
    }
    else {
        ShoppingCart_ProfileOptions_DataGrid1_ctl02_Label4.innerHTML = ' Month'
        ShoppingCart_ProfileOptions_DataGrid1_ctl03_Label4.innerHTML = ' Months'
        ShoppingCart_ProfileOptions_DataGrid1_ctl04_Label4.innerHTML = ' Months'
        ShoppingCart_ProfileOptions_DataGrid1_ctl05_Label4.innerHTML = ' Months'
        ShoppingCart_ProfileOptions_DataGrid1_ctl06_Label4.innerHTML = ' Months'
    }
}
    
//End Add