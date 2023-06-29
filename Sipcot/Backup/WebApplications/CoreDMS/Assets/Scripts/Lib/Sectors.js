
function ListBox1_onChange(list,h,hidSector) {
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


//function delItemFromList(event, list) {
//   
//    if (event.keyCode == 46) {
//        // Delete has been pressed
//        if (list.selectedIndex >= 0) {
//            for (x = list.selectedIndex; x + 1 < list.options.length; x++) {
//                list.options[x].text = list.options[x + 1].text
//                list.options[x].value = list.options[x + 1].value
//            }
//            --list.options.length;
//        }
//    }
//    persistOptionsList();
//}
