function PropertyGrid() {

   //private variables
   var Properties = new Array();
   var EditorsLookup = new Array();
   var PropertiesLookup = new Array();
   var groupCount = 0;
   var subGroupCount = 0;
   var cssItem = 0;
   //could attach public property here
   var styleOnChange = "onObjectPropertiesChanged";

   //public variables
   this.OutputDivId = "pg2";
   this.DefaultSizeType = "px";
   this.PropertyGridOnChange = styleOnChange;


   //public methods
   PropertyGrid.prototype.GroupCount = function () { return Properties.length; }

   //LOAD EVENT
   $().ready(function () {
   });

   var OnTestChange = function (handler) {
      styleOnChange = handler;
   }

   var CreateItem = function (groupName, rowType, propertyName, drpFields, isSubGroup, userData, defaultValue, displayName) {
      //create css property
      //first check if groupName exists
      var foundGroup = false;
      for (var i = 0; i < Properties.length; i++) {
         if (Properties[i].GroupName == groupName) {
            //found group - check if its going to be a sub group or a item within group?
            if (isSubGroup != "") {
               //sub group - search the inner subgroups loop
               var foundSubGroup = false;
               for (var x = 0; x < Properties[i].SubGroups.length; x++) {
                  if (Properties[i].SubGroups[x].GroupName == isSubGroup) {
                     //sub group found add item
                     foundSubGroup = true;
                     Properties[i].SubGroups[x].Items[Properties[i].SubGroups[x].Items.length] = new ClassItem(propertyName, rowType, drpFields, cssItem, userData, defaultValue, displayName);
                     cssItem++;
                  }
               }
               if (!foundSubGroup) {
                  //create sub group
                  Properties[i].SubGroups[Properties[i].SubGroups.length] = new Group(isSubGroup, subGroupCount, true);
                  Properties[i].SubGroups[Properties[i].SubGroups.length - 1].Items[Properties[i].SubGroups[Properties[i].SubGroups.length - 1].Items.length] = new ClassItem(propertyName, rowType, drpFields, cssItem, userData, defaultValue, displayName);
                  cssItem++;
                  subGroupCount++;
               }
            }
            else {
               //item within group
               Properties[i].Items[Properties[i].Items.length] = new ClassItem(propertyName, rowType, drpFields, cssItem, userData, defaultValue, displayName);
               cssItem++;
            }
            foundGroup = true;
         }
      }
      if (!foundGroup) {
         //create new group with item
         Properties[Properties.length] = new Group(groupName, groupCount, false);
         if (!isSubGroup) {
            Properties[Properties.length - 1].Items[Properties[Properties.length - 1].Items.length] = new ClassItem(propertyName, rowType, drpFields, cssItem, userData, defaultValue, displayName);
         }
         else {
            Properties[Properties.length - 1].SubGroups[Properties[Properties.length - 1].SubGroups.length] = new Group(isSubGroup, subGroupCount, true);
            Properties[Properties.length - 1].SubGroups[Properties[Properties.length - 1].SubGroups.length - 1].Items[Properties[Properties.length - 1].SubGroups[Properties[Properties.length - 1].SubGroups.length - 1].Items.length] = new ClassItem(propertyName, rowType, drpFields, cssItem, userData, defaultValue);
            cssItem++;
            subGroupCount++;
         }

         groupCount++;
         cssItem++;
      }
   }

   var RenderGrid = function () {
      //creates the html output from Properties Object
      var htmlOut = "<div id=\"propertyGridContainer\">";
      try {
         for (var i = 0; i < Properties.length; i++) {
            //htmlOut += "<div id=\"" + Properties[i].OutputDivId + "\" onclick=\"javascript:PropertyGrid.Expand('" + Properties[i].OutputDivId + "',false);\" class=\"pgGroup\"><div id=\"image" + Properties[i].OutputDivId + "\" class=\"pgGroupShrink bkMinimise\">&nbsp;</div><div class=\"pgGroupText\">" + Properties[i].GroupName + "</div></div>";
            /*htmlOut += "<div id=\"" + Properties[i].OutputDivId + "\" onclick=\"javascript:PropertyGrid.Expand('" + Properties[i].OutputDivId + "',false);\" class=\"pgGroup\">"+
            "<div id=\"image" + Properties[i].OutputDivId + "\" class=\"pgGroupShrink bkMinimise\">&nbsp;</div>"+
            "<div class=\"pgGroupText\">" + Properties[i].GroupName + "</div>" +
            "</div>";*/

            htmlOut += //"<div id=\"" + Properties[i].OutputDivId + "\" class=\"pgGroup\">"+
                           "<div data-role=\"collapsible-set\" data-theme=\"e\" data-content-theme=\"d\">" +
                              "<div data-role=\"collapsible\" data-collapsed=\"true\">" +
            //"<div id=\"image" + Properties[i].OutputDivId + "\" class=\"pgGroupShrink bkMinimise\">&nbsp;</div>"+
                                    "<h3>" + Properties[i].GroupName + "</h3>";

            //render group items
            htmlOut += "<div id=\"child" + Properties[i].OutputDivId + "\">";
            for (var x = 0; x < Properties[i].Items.length; x++) {
               htmlOut += RenderGridInput(Properties[i].Items[x].Type, Properties[i].Items[x].OutputDivId, Properties[i].Items[x].CssId, Properties[i].Items[x].DropFields, Properties[i].Items[x].DefaultValue, Properties[i].Items[x].DisplayName);
            }
            //check for subgroups
            if (Properties[i].SubGroups.length > 0) {
            for (var ii = 0; ii < Properties[i].SubGroups.length; ii++) {
            htmlOut += "<div id=\"" + Properties[i].SubGroups[ii].OutputDivId + "\" class=\"pgSubGroup\" onclick=\"PropertyGrid.Expand('" + Properties[i].SubGroups[ii].OutputDivId + "','true');\"><div id=\"image" + Properties[i].SubGroups[ii].OutputDivId + "\" class=\"pgSubGroupShrink bkMinimiseSub\">&nbsp;</div><div class=\"pgSubGroupText\">" + Properties[i].SubGroups[ii].GroupName + "</div></div>";
            //render sub group items
            htmlOut += "<div id=\"child" + Properties[i].SubGroups[ii].OutputDivId + "\">";
            for (var xx = 0; xx < Properties[i].SubGroups[ii].Items.length; xx++) {
               htmlOut += RenderGridInput(Properties[i].SubGroups[ii].Items[xx].Type, Properties[i].SubGroups[ii].Items[xx].OutputDivId, Properties[i].SubGroups[ii].Items[xx].CssId, Properties[i].SubGroups[ii].Items[xx].DropFields, Properties[i].Items[x].DefaultValue, Properties[i].Items[x].DisplayName);
            }
            htmlOut += "</div>";
            }
            }

            htmlOut += "</div>" + "</div>" + "</div>";
         }
      } catch (e) { alert(e); }
      document.getElementById(PropertyGrid.OutputDivId).innerHTML = htmlOut + "</div>";
      //$("input[class*='pgInputNum']").spinbox();
   }

   var RenderGridInput = function (type, outputDivId, cssId, fields, defaultValue, displayName) {
      //creates the output row with the input controls for different types
      switch (type) {
         case 'label':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                           "<input id=\"" + inputId + "\"value=\"" + defaultValue + "\"></input>" +
                        "</div>";
            return label;

         case 'input':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + inputId + "', this.value);\" value=\"" + defaultValue + "\"></input>" +
                        "</div>";
            return label;

         case 'integerInput':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onkeypress='PropertyGrid.validateNumbericalKey(event)' onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + inputId + "', this.value);\" value=\"" + defaultValue + "\"></input>" +
                        "</div>";
            return label;

         case 'boolean':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + outputDivId + "\" style=\"text-align:center;\">" + displayName + "</label>" +
                           "<select name=\"slider\" id=\"" + outputDivId + "\" data-role=\"slider\" data-theme=\"a\"onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + outputDivId + "', this.value);\">" 
            var items1 = fields.split('|');
            for (i = 0; i < items1.length; i++) {
               if (items1[i].toString().toLowerCase() == defaultValue.toString().toLowerCase()) {
                  label += "<option value=\"" + items1[i] + "\"selected>" + items1[i] + "</option>";
               }
               else {
                  label += "<option value=\"" + items1[i] + "\">" + items1[i] + "</option>";
               }
            }
            label += "</select> " + "</div>";
                           "</select> " +
                        "</div>";
            return label;

         case 'color':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                        "<select name=\"slider\" id=\"" + inputId + "\" data-theme=\"a\"onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + outputDivId + "', this.value);\">";
            var items = fields.split('|');
            for (i = 0; i < items.length; i++) {
               label += "<option value=\"" + items[i] + "\">" + items[i] + "</option>";
            }
            label += "</select> " + "</div>";

            return label;

         case 'num':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                           "<input style=\"text-align:center;\" id=\"" + inputId + "\" class=\"pgInputNum\" onchange=\"Javascript:PropertyGrid.UpdateStyleInnerNum('" + cssId + "','" + inputId + "', this.value);\" value=\"" + defaultValue + "\"></input>" +
                        "</div>";
            return label;

         case 'picture':
            var inputId = outputDivId;
            var source = "";
            if (defaultValue != null) {
               source = defaultValue.get_source();
            }
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + " URL" + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + inputId + "', String.isNullOrEmpty(this.value) ? null : new lt.Annotations.Core.AnnPicture(this.value));\" value=\"" + source + "\"></input>" +
                        "</div>";
            return label;

         case 'media':
            var inputId = outputDivId;
            var source1 = "";
            var source2 = "";
            var source3 = "";
            if (defaultValue.get_source1() != null) {
               source1 = defaultValue.get_source1();
            }
            if (defaultValue.get_source2() != null) {
               source2 = defaultValue.get_source2();
            }
            if (defaultValue.get_source3() != null) {
               source3 = defaultValue.get_source3();
            }
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + " URL 1" + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onchange=\"Javascript:PropertyGrid.UpdateStyleInnerMedia('" + cssId + "','" + inputId + "','" + "1" + "', this.value);\" value=\"" + source1 + "\"></input>" +
                           "</div>" +
                           "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + " URL 2" + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onchange=\"Javascript:PropertyGrid.UpdateStyleInnerMedia('" + cssId + "','" + inputId + "','" + "2" + "', this.value);\" value=\"" + source2 + "\"></input>" +
                           "</div>" +
                           "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + " URL 3" + "</label>" +
                           "<input style=\"text-align:center;\"id=\"" + inputId + "\" onchange=\"Javascript:PropertyGrid.UpdateStyleInnerMedia('" + cssId + "','" + inputId + "','" + "3" + "', this.value);\" value=\"" + source3 + "\"></input>" +
                        "</div>";
            return label;

         case 'drop':
            return "<div id=\"hold" + outputDivId + "\" class=\"pgGroupItem\"><div class=\"pgGroupItemText\">" + displayName + "</div><div class=\"pgInputHolder\"><select id=\"" + outputDivId + "\" class=\"pgInputDrop\" onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + outputDivId + "',this.value);\"><option value='none'>none</option><option value='solid'>solid</option><option value='ridge'>ridge</option><option value='dashed'>dashed</option><option value='dotted'>dotted</option><option value='double'>double</option><option value='groove'>groove</option><option value='inset'>inset</option></select></div></div>";
         case 'cdrop':
            var inputId = outputDivId;
            var label = "<div data-role=\"fieldcontain\">" +
                           "<label for=\"" + inputId + "\" style=\"text-align:left;\">" + displayName + "</label>" +
                        "<select name=\"slider\" id=\"" + inputId + "\" data-theme=\"a\"onchange=\"Javascript:PropertyGrid.UpdateStyleInner('" + cssId + "','" + outputDivId + "', this.value);\">";
            var items = fields.split('|');
            for (i = 0; i < items.length; i++) {
               if (items[i].toString().toLowerCase() == defaultValue.toString().toLowerCase()) {
                  label += "<option value=\"" + items[i] + "\"selected>" + items[i] + "</option>";
               }
               else {
                  label += "<option value=\"" + items[i] + "\">" + items[i] + "</option>";
               }
            }
            label += "</select> " + "</div>";

            return label;
         default:
            return "<div id=\"hold" + outputDivId + "\" class=\"pgGroupItem\"><div class=\"pgGroupItemText\">" + cssId + "</div><div class=\"pgInputHolder\"><input id=\"" + outputDivId + "\" class=\"pgInput\"></input></div></div>";
      }
   }

   PropertyGrid.prototype.validateNumbericalKey = function (event) {
      var keyEvent = event || window.event;

      var key = keyEvent.keyCode || keyEvent.which;

      switch (key) {
          case 8:
          case 37:
          case 39:
          case 46:
              return;
      }

      key = String.fromCharCode(key);

      var regex = /[0-9]/;

      if (!regex.test(key)) {
         keyEvent.returnValue = false;
         if (keyEvent.preventDefault) {
            keyEvent.preventDefault();
         }
      }
   }

   PropertyGrid.prototype.UpdateStyleInnerNum = function (cssId, divId, value) {
      if (value.indexOf('px') != -1 || value.indexOf('em') != -1) {
         PropertyGrid.UpdateStyleInner(cssId, divId, parseFloat(value));
      }
      else {
         //add default value extension
         PropertyGrid.UpdateStyleInner(cssId, divId, parseFloat(value));
      }
   }

   PropertyGrid.prototype.UpdateStyleInnerMedia = function (cssId, divId, id, value) {
      var media = EditorsLookup[cssId + divId].get_value().clone();
      switch (id) {
         case "1":
            media.set_source1(value);
            break;
         case "2":
            media.set_source2(value);
            break;
         case "3":
            media.set_source3(value);
            break;
      }
      PropertyGrid.UpdateStyleInner(cssId, divId, media);
   }

   PropertyGrid.prototype.UpdateStyleInner = function (cssId, divId, value) {
      //fire back to handler
      if (styleOnChange.indexOf('.') == -1) {
         gthis[styleOnChange](value, EditorsLookup[cssId+divId]);
      }
      else {
         gthis[styleOnChange.substring(0, styleOnChange.indexOf('.'))][styleOnChange.substring(styleOnChange.indexOf('.') + 1)](value, EditorsLookup[cssId + divId]);
      }
   }
   //global this function for callback
   var gthis = (function () { return this; })();

   PropertyGrid.prototype.AttachColorPicker = function (cssId, outputDivId) {
      $("#color" + outputDivId).ColorPicker({
         onSubmit: function (hsb, hex, rgb, el) {
            //$(el).val(hex);
            $(el).css("backgroundColor", "#" + hex);
            $(el).ColorPickerHide();
            PropertyGrid.UpdateStyleInner(cssId, outputDivId, "#" + hex);
         },
         onChange: function (hsb, hex, rgb) {
            $("#color" + outputDivId).css('backgroundColor', '#' + hex);
            $("input[id='" + outputDivId + "']").val('#' + hex);
            PropertyGrid.UpdateStyleInner(cssId, outputDivId, "#" + hex);
         },
         onBeforeShow: function () {
            var color = $("input[id='" + outputDivId + "']").val();
            $(this).ColorPickerSetColor($("input[id='" + outputDivId + "']").val());
         }
      });
   }

   PropertyGrid.prototype.ClearValues = function () {
      for (var i = 0; i < PropertiesLookup.length; i++) {
         var propLookup = PropertiesLookup[i][1];
         document.getElementById(propLookup).value = "";
      }
   }

   PropertyGrid.prototype.EmptyGrid = function () {
      Properties = new Array();
      EditorsLookup = new Array();
      PropertiesLookup = new Array();
      groupCount = 0;
      subGroupCount = 0;
      cssItem = 0;
      document.getElementById(PropertyGrid.OutputDivId).innerHTML = '';
   }

   PropertyGrid.prototype.HideItems = function () {
      //hide groups and sub groups
      $("div[id*=childpg1_group_]").css('display', 'none');
      $("div[id*=childpg1_subgroup_]").css('display', 'none');
      $("div[id*=imagepg1_group_]").removeClass('bkMinimise').addClass('bkExpand');
      $("div[id*=imagepg1_subgroup_]").removeClass('bkMinimiseSub').addClass('bkExpandSub');
   }

   PropertyGrid.prototype.Expand = function (id, sub) {
      //SHOW / HIDE PARTICLUAR LEVELS
      if (document.getElementById('child' + id).style.display == "" || document.getElementById('child' + id).style.display == "block") {
         //MAKE SMALL
         document.getElementById('child' + id).style.display = "none";
         if (sub == 'true') {
            $("#image" + id).removeClass('bkMinimiseSub').addClass('bkExpandSub');
         }
         else {
            $("#image" + id).removeClass('bkMinimise').addClass('bkExpand');
         }
      }
      else {
         //SHOW
         document.getElementById('child' + id).style.display = "block";
         if (sub == 'true') {
            $("#image" + id).removeClass('bkExpandSub').addClass('bkMinimiseSub');
         }
         else {
            $("#image" + id).removeClass('bkExpand').addClass('bkMinimise');
         }
      }
   }

   var Group = function (groupName, id, subgroup) {
      this.GroupName = groupName;
      if (subgroup) {
         this.OutputDivId = 'pg1_subgroup_' + id;
      }
      else {
         this.OutputDivId = 'pg1_group_' + id;
      }
      this.Items = new Array();
      this.SubGroups = new Array();
   }

   var ClassItem = function (propertyName, rowType, drpFields, id, userData, defaultValue, displayName) {
      this.CssId = propertyName;
      this.DisplayName = displayName;
      this.OutputDivId = 'pg1_item_' + id;
      this.Type = rowType;
      this.Value = '';
      this.DropFields = drpFields;
      this.UserData = userData;
      this.DefaultValue = defaultValue;

      //add an item to PropertiesLookup its a quick way to reference css keyname with id needs to be output to
      PropertiesLookup[PropertiesLookup.length] = [propertyName, this.OutputDivId, rowType];
      EditorsLookup[propertyName + this.OutputDivId] = userData;
   }



   PropertyGrid.prototype.FillCss2 = function () {
      PropertyGrid.EmptyGrid();
      //in format [groupName],[rowType],[cssName],[drpFields],[isSubGroup]
      //rowTypes can  be [input],[color],[num],[drop],[cdrop] cdrop is customized drop list
      CreateItem("background", "input", "background", "", "");
      CreateItem("background", "input", "background-image", "", "");
      CreateItem("background", "input", "background-position", "", "");
      CreateItem("background", "color", "background-color", "", "");
      CreateItem("background", "cdrop", "background-repeat", "|repeat|no-repeat|repeat-x|repeat-y", "");

      CreateItem("font", "input", "font", "", "");
      CreateItem("font", "cdrop", "font-family", " |Arial|Arial Black|Bookman Old Style|Comic Sans MS|Courier|Courier New|Gadget|Garamond|Georgia|Helvetica|Impact|Lucida Console|Lucida Sans Unicode|Lucida Grande|MS Sans Serif|MS Serif|Palatino Linotype|Symbol|Tahoma|Times New Roman|Trebuchet MS|Verdana|Webdings|Wingdings", "");
      CreateItem("font", "cdrop", "font-style", "|normal|italic|oblique|inherit", "");
      CreateItem("font", "num", "font-size", "", "");
      CreateItem("font", "num", "font-weight", "bold|bolder|lighter|normal", "");
      CreateItem("font", "cdrop", "font-varient", "|normal|small-caps", "");

      CreateItem("text", "input", "text", "", "");
      CreateItem("text", "cdrop", "text-align", "|center|justify|left|right", "");
      CreateItem("text", "cdrop", "text-decoration", "|underline|overline|none|blink|both", "");
      CreateItem("text", "input", "text-indent", "", "");
      CreateItem("text", "cdrop", "text-justify", "|auto|distribute|distribute-all-lines|inter-word|newspaper", "");
      CreateItem("text", "cdrop", "text-transform", "|capitalize|lowercase|none|uppercase", "");
      CreateItem("text", "color", "color", "", "");

      //RENDER THE ITEMS
      RenderGrid();
   }

   var EnumEditObject = function (properties, group, subgroup) {
      for (var propertyName in properties) {
         var propetyInfo = properties[propertyName];
         var values = ""
         if (propetyInfo != null && propetyInfo.get_isVisible()) {
            if (propetyInfo.get_editorType() && propetyInfo.get_editorType().get_properties() != null) {
               EnumEditObject(propetyInfo.get_editorType().get_properties(), propetyInfo.get_category());
            }
            else {
               var editorType = propetyInfo.get_editorType();
               var uiEditor = "label";
               if (editorType instanceof (lt.Annotations.Automation.AnnStringEditor)) {
                  if (!propetyInfo.get_isReadOnly())
                     uiEditor = "input";
                  else
                     uiEditor = "label";
               }
               else if (editorType instanceof (lt.Annotations.Automation.AnnColorEditor)) {
                  uiEditor = 'color';
               }
               else if (editorType instanceof (lt.Annotations.Automation.AnnBooleanEditor)) {
                  uiEditor = 'boolean';
               }
               else if (editorType instanceof (lt.Annotations.Automation.AnnDoubleEditor)) {
                  uiEditor = 'num';
               }
               else if (editorType instanceof (lt.Annotations.Automation.AnnPictureEditor)) {
                  var uiEditor = "picture";
               }
               else if (editorType instanceof (lt.Annotations.Automation.AnnIntegerEditor)) {
                  var uiEditor = "integerInput";
               } 
               else if (editorType instanceof (lt.Annotations.Automation.AnnMediaEditor)) {
                  var uiEditor = "media";
               }

               //uiEditor = 'cdrop'
               if (uiEditor != 'boolean') {
                  if (uiEditor == 'color') {
                     uiEditor = 'cdrop'
                     values += "Transparent|" + "Red|" + "Blue|" + "Green|" + "Yellow|" + "Black|" + "White";
                  }
                  else {
                     for (var i in propetyInfo.get_values()) {
                        uiEditor = 'cdrop'
                        values += i + "|";
                     }

                     values = values.slice(0, values.length - 1);
                  }
               }
               else {
                  for (var i in propetyInfo.get_values()) {
                     uiEditor = 'boolean'
                     values += i + "|";
                  }

                  values = values.slice(0, values.length - 1);
               }

               CreateItem(propetyInfo.get_category(), uiEditor, propertyName, values, "", propetyInfo.get_editorType(), propetyInfo.get_value(), propetyInfo.get_displayName());
            }
         }
      }
   }

   PropertyGrid.prototype.EditObject = function (annObject) {
      PropertyGrid.EmptyGrid();

      if (annObject != null) {
         var annEditObject = new lt.Annotations.Automation.AnnObjectEditor(annObject);

         //remove Hyperlink property from media object and audio object
         if (annObject.get_id() == lt.Annotations.Core.AnnObject.mediaObjectId || annObject.get_id() == lt.Annotations.Core.AnnObject.audioObjectId) {
            delete annEditObject.get_properties().Hyperlink;
         }

         EnumEditObject(annEditObject.get_properties(), "", "");
         //RENDER THE ITEMS
         RenderGrid();
         $('#pg2').trigger('create');
      }
   }
}
