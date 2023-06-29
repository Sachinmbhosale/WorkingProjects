/* Start - Homepage.js */
var tlh_sell = tlh_buy = 25; //	TL Height of Sell and Buy
var tlcount = 20; //	TL Count
var curdh_sell = curdh_buy = 0; //	Current Div height of Selling and Buying Tl
var cv_sell = cv_buy = 0; //	Current Value of Selling and Buying TL
var t_sell, t_buy; //	Time Interval id of Selling and Buying TL
var i_buy = i_sell = 1; 	//	Scroll Interval period of Selling and Buying TL (in Sec)
var att_sell = att_buy = { scroll: { to: [0, 0]} }; //	Attribute of Selling and Buying TL
var time_int_sell = time_int_buy = 2000; //	Time Interval of call rotate function of Selling and Buying TL
function rotate_sell()	//	Rotate script for Selling TL
{ 
    if (cv_sell == (tlcount + 1)) {
        cv_sell = 0;
        i_sell = 0;
        time_int_sell = 100;
    }
    else {
        i_sell = 1;
        time_int_sell = 2000;
    }

    curdh_sell = cv_sell * tlh_sell;
    att_sell = { scroll: { to: [0, curdh_sell]} };
    anim_sell = new YAHOO.util.Scroll('idSellLead', att_sell, i_sell);
    anim_sell.animate();
    cv_sell++;

    t_sell = setTimeout("rotate_sell()", time_int_sell);
}
function startSellTL() {
 
    t_sell = setTimeout("rotate_sell()", 500);

}
function stopSellTL() {
    clearTimeout(t_sell);
}

function rotate_buy()	//	Rotate script for Buying TL
{
    if (cv_buy == (tlcount + 1)) {
        cv_buy = 0;
        i_buy = 0;
        time_int_buy = 100;
    }
    else {
        i_buy = 1;
        time_int_buy = 2000;
    }
    curdh_buy = cv_buy * tlh_buy;
    att_buy = { scroll: { to: [0, curdh_buy]} };
    anim_buy = new YAHOO.util.Scroll('idBuyLead', att_buy, i_buy);
    anim_buy.animate();
    cv_buy++;
    
    t_buy = setTimeout("rotate_buy()", time_int_buy);
}

function startBuyTL() {
    t_buy = setTimeout("rotate_buy()", 500);
}
function stopBuyTL() {
    clearTimeout(t_buy);
}

rotate_sell();
rotate_buy();

var clrarr = new Array();
var StatstimeDelay = 500;
clrarr[0] = "shblack";
clrarr[1] = "shwhite";
var idSH = 0;
var dgi = document.getElementById("HpStats1_lblStatsHead");
if (dgi != null) {
    changeColor();
} else {
    dgi = document.getElementById("HpStats1_lblStatsHead");
    if (dgi != null) {
        changeColor();
    }
}

function changeColor() {
    if (idSH == (clrarr.length)) {
        idSH = 0;
    }
    dgi.className = clrarr[idSH];
    idSH++;
    setTimeout("changeColor()", StatstimeDelay);
}

/* <!--- TobocTips Script -->*/
var cont = new Array();
var lenAllowed = 50;
var timeDelay = 3000;
var tipOne = "Did you know TRADE PLUS members enjoy 25 messages to other Toboc members?";
var tipTwo = "Why be listed only in 1 sub-sector when your business is into multiple products – Only TRADE PLUS members get 10 sub-sector listings!";
var tipThree = "Please avoid uploading the picture with contact information";
cont[0] = "";
//        alert(lenAllowed +""+timeDelay+""+ tipOne +tipTwo +tipThree);
cont[1] = "<a onmouseover='this.style.cursor=\"pointer\" ' onfocus='this.blur();' ";
if (tipOne.length > lenAllowed) {
    cont[1] = cont[1] + " onclick=\"document.getElementById('divTip1').style.display = 'block'; document.getElementById('spnTipContent1').innerHTML='" + tipOne + "';\""
}
cont[1] = cont[1] + " >" + tipOne.substr(0, lenAllowed);
if (tipOne.length > lenAllowed) {
    cont[1] = cont[1] + "...";
}
cont[1] = cont[1] + " </a>";
//        alert(cont[1]);

cont[2] = "<a onmouseover='this.style.cursor=\"pointer\" ' onfocus='this.blur();' ";
if (tipTwo.length > lenAllowed) {
    cont[2] = cont[2] + " onclick=\"document.getElementById('divTip2').style.display = 'block'; document.getElementById('spnTipContent2').innerHTML='" + tipTwo + "';\""
}
cont[2] = cont[2] + " >" + tipTwo.substr(0, lenAllowed);
if (tipTwo.length > lenAllowed) {
    cont[5] = cont[5] + "...";
}
cont[2] = cont[2] + " </a>";

cont[3] = "<a onmouseover='this.style.cursor=\"pointer\" ' onfocus='this.blur();' ";
if (tipThree.length > lenAllowed) {
    cont[3] = cont[3] + " onclick=\"document.getElementById('divTip3').style.display = 'block'; document.getElementById('spnTipContent3').innerHTML='" + tipThree + "';\""
}
cont[3] = cont[3] + " >" + tipThree.substr(0, lenAllowed);
if (tipThree.length > lenAllowed) {
    cont[3] = cont[3] + "...";
}
cont[3] = cont[3] + " </a>";

cont[4] = "<a onmouseover='this.style.cursor=\"pointer\" ' onfocus='this.blur();' ";
if (tipOne.length > lenAllowed) {
    cont[4] = cont[4] + " onclick=\"document.getElementById('divTip1').style.display = 'block'; document.getElementById('spnTipContent1').innerHTML='" + tipOne + "';\""
}
cont[4] = cont[4] + " >" + tipOne.substr(0, lenAllowed);
if (tipOne.length > lenAllowed) {
    cont[4] = cont[4] + "...";
}
cont[4] = cont[4] + " </a>";

cont[5] = "<a onmouseover='this.style.cursor=\"pointer\" ' onfocus='this.blur();' ";
if (tipTwo.length > lenAllowed) {
    cont[5] = cont[5] + " onclick=\"document.getElementById('divTip2').style.display = 'block'; document.getElementById('spnTipContent2').innerHTML='" + tipTwo + "';\""
}
cont[5] = cont[5] + " >" + tipTwo.substr(0, lenAllowed);
if (tipTwo.length > lenAllowed) {
    cont[5] = cont[5] + "...";
}
cont[5] = cont[5] + " </a>";

curval = 2;
sp1 = document.getElementById("spnTobocTips");
changeTL();

var st;
function changeTL() {
    if (sp1 == null)
        sp1 = document.getElementById("spnTobocTips");
    if (sp1 == null) {
        st = setTimeout("changeTL()", timeDelay);
        return;
    }
    if (curval == 6) curval = 1;
    sp1.innerHTML = cont[curval + 0];
    curval++;
    st = setTimeout("changeTL()", timeDelay);
}

function startScroll() {    
    st = setTimeout("changeTL()", timeDelay);
}

function stopScroll() {
    clearTimeout(st);
}
/* END - Homepage.js*/

//End Add
