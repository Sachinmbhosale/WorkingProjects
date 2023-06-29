/// <reference path="../jQuery/jquery-ui-1.8.18.custom.min.js" />//
/// <reference path="../jQuery/jquery-1.4.1.js" />
/// <reference path="http://ajax.googleapis.com/ajax/libs/jquery/1.6.4/jquery.min.js" />

_imageViewer = null;
_automationManager = null;
var _svgRenderingEngine = null;
var _html5RenderingEngine = null;
_activeAutomation = null;
var textIsready = false;
var imageViewerAutomationControl;
var PropertyGrid = new PropertyGrid();
_propertiesVisible = false;
var _leadtoolsRESTServicesHostAppName;
var _leadtoolsRESTServicesHost;
var _audio = null;
var _video = null;
_scrollTop = 0;

_images = null;
var _enableSvg = false;

var _isTouchDevice = isTouchDevice();
if (isTouchDevice()) {
    _addCss('Styles/AnnotationToolbar.touchDevices.css');
}
else {
    _addCss('Styles/AnnotationToolbar.css');
}

if (!lt.LTHelper.supportsCanvas || (lt.LTHelper.browser === lt.LTBrowser.internetExplorer && lt.LTHelper.device !== lt.LTDevice.desktop && lt.LTHelper.version <= 9)) {
    window.location.replace('NoHTML5Support.htm');
}

function _addCss(fileName) {
    var fileref = document.createElement('link');
    fileref.setAttribute('rel', 'stylesheet');
    fileref.setAttribute('type', 'text/css');
    fileref.setAttribute('href', fileName);
    if (!ss.isUndefined(fileref)) {
        document.getElementsByTagName('head')[0].appendChild(fileref);
    }
}

var imageData = function (id, xRes, yRes) {
    this.imageId = id;
    this.xResolution = xRes;
    this.yResolution = yRes;
}

// RightClickInteractiveMode - our own interactive service
var _this = null;
RightClickInteractiveMode = function RightClickInteractiveMode() {
    RightClickInteractiveMode.initializeBase(this);
    _this = this;
}

RightClickInteractiveMode.prototype = {

    // Helper toString method
    toString: function RightClickInteractiveMode$toString() {
        return "RightClick";
    },

    // Called by the base class when the mode is started
    start: function RightClickInteractiveMode$start(viewer) {
        this.set_workOnImageRectangle(false);

        // Always call base class start method
        RightClickInteractiveMode.callBaseMethod(this, "start", [viewer]);

        // Subscribe to the Tap event
        var service = RightClickInteractiveMode.callBaseMethod(this, "get_interactiveService");
        service.add_tap(this._service_Tap);
    },

    // Called by the base class when the mode is stopped
    stop: function RightClickInteractiveMode$stop(viewer) {
        // Check if we have started
        if (this.get_isStarted()) {
            // UnSubscribe to events
            var service = RightClickInteractiveMode.callBaseMethod(this, "get_interactiveService");
            service.remove_tap(this._service_Tap);

            // Always call base class stop method
            RightClickInteractiveMode.callBaseMethod(this, "stop", [viewer]);
        }
    },

    // Called when the user taps
    _service_Tap: function RightClickInteractiveMode$_service_DragStarted(sender, e) {
        if (!_this.canStartWork(e)) {
            return;
        }

        if (_activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.run) {
            return;
        }

        // Inform whoever is listening that we have started working
        _this.onWorkStarted(lt.LeadEventArgs.Empty);

        // Inform whoever is listening that we have stoped working
        _this.onWorkCompleted(lt.LeadEventArgs.Empty);

        // Display our own context menu
        myContextMenu(e.get_position().get_x(), e.get_position().get_y());
    }
}
// Our RightClick interactive mode class, derives from lt.Controls.ImageViewerInteractiveMode
RightClickInteractiveMode.registerClass("RightClickInteractiveMode", lt.Controls.ImageViewerInteractiveMode);

// Display our own context menu (really, just display the properties dialog for the currently selected object
function myContextMenu(x, y) {
    // get the automation object and select the object under the current pointer position
    var automation = _activeAutomation;
    var automationControl = automation.get_automationControl();
    var container = automation.get_container();
    var point = lt.LeadPointD.create(x, y);
    point = container.get_mapper().pointToContainerCoordinates(point);
    var objects = container.hitTestPoint(point); // perform the hit test
    if (objects != null && objects.length > 0) { // if we hit an object, select it and then show the properties
        var targetObject = objects[objects.length - 1];
        automation.selectObject(targetObject);
        automationControl.automationInvalidate();
        if (automation.get_canLock()) {
            showProperties();
        }
    }
}

// Hides the properties dialog
function closeProperties() {
    _propertiesVisible = false;
    $("#propertiesPage").dialog('close');
    updateToolbar();
}

// Display the properties dialog
function showProperties() {
    _scrollTop = $("#ImageViewer_div").scrollTop();
    var editObject = _activeAutomation.get_currentEditObject();

    if (editObject != null) {
        if (editObject.get_isLocked()) {
            alert('Cannot change properties for a locked object.');
            return;
        }
        if ((editObject.get_id() === lt.Annotations.Core.AnnObject.groupObjectId) ||
           (editObject.get_id() === lt.Annotations.Core.AnnObject.selectObjectId)) {
            alert('Cannot change properties for a group.');
            return;
        }
    }
    PropertyGrid.EditObject(editObject);

    var content = document.getElementById("Content");
    var element = document.getElementById("propertiesDialog");
    if (element != null && !_propertiesVisible) {
        content.removeChild(element);
        element = null;
    }

    element = document.createElement("a");
    element.setAttribute("id", "propertiesDialog");
    // element.setAttribute("href", "#propertiesPage"); // commented by Yogeesha to hide annotation properties
    element.setAttribute("data-rel", "dialog");
    element.setAttribute("data-transition", "flip");

    content.appendChild(element);
    // _propertiesVisible = true;  // commented by Yogeesha to hide annotation properties
    // $("#propertiesDialog").click();  // commented by Yogeesha to hide annotation properties
}

function keyDown(e) {
    if (_propertiesVisible == true) {
        return;
    }
    if (e.keyCode == 93) {
        if (_activeAutomation != null && _activeAutomation.get_canShowProperties()) {
            // showProperties(); // commented by Yogeesha to hide annotation properties
            e.preventDefault();
            e.stopImmediatePropagation();
        }
        else {
            alert("Select an annotation object first.");
            e.preventDefault();
            e.stopImmediatePropagation();
        }
    }
    else if (e.keyCode == 46) { // Delete
        annDelete();
        e.preventDefault();
        e.stopImmediatePropagation();
    }
    else if (e.keyCode == 90 && e.ctrlKey) { // CTRL + Z
        annUndo();
    }
    else if (e.keyCode == 89 && e.ctrlKey) { // CTRL + Y
        annRedo();
    }
}

function CustomAnnotations() {
}
////////////////////////////////////////////////////////////////////////////////
// AnnTriangleObject
CustomAnnotations.AnnTriangleObject = function AnnTriangleObject() {
    AnnTriangleObject.initializeBase(this);
    this.set_isClosed(true); // triangle is a closed figure
    this.setId(-99); // set the object id
    this.set_tag(null);
}
CustomAnnotations.AnnTriangleObject.prototype = {
    create: function AnnTriangleObject$create() {
        // define the custom annotation object (triangle is polyline with only 3 points)
        return new CustomAnnotations.AnnTriangleObject();
    }
}
CustomAnnotations.AnnTriangleObject.registerClass('CustomAnnotations.AnnTriangleObject', lt.Annotations.Core.AnnPolylineObject);

////////////////////////////////////////////////////////////////////////////////
// AnnTriangleDrawDesigner
CustomAnnotations.AnnTriangleDrawDesigner = function AnnTriangleDrawDesigner(automationControl, container, annPolyineObject) {
    AnnTriangleDrawDesigner.initializeBase(this, [automationControl, container, annPolyineObject]);
}
CustomAnnotations.AnnTriangleDrawDesigner.prototype = {
    // override the onPointerDown method and add 3 points for our triangle
    onPointerDown: function AnnTriangleDrawDesigner$onPointerDown(sender, e) {
        var handled = CustomAnnotations.AnnTriangleDrawDesigner.callBaseMethod(this, 'onPointerDown', [sender, e]);
        if (this.get_targetObject().get_points().get_count() < 3) {
            this.get_targetObject().set_tag('drawing');
            if (e.get_button() === lt.Annotations.Core.AnnMouseButton.left) {
                this.startWorking();
                this.get_targetObject().get_points().add(e.get_location());
                handled = true;
            }
        }

        CustomAnnotations.AnnTriangleDrawDesigner.callBaseMethod(this, 'invalidate', [this.get_targetObject().getInvalidateRect()]);
        return handled;
    },

    // override the onPointerUp method and end the drawing when we have our 3 points
    onPointerUp: function AnnTriangleDrawDesigner$onPointerUp(sender, e) {
        var handled = CustomAnnotations.AnnTriangleDrawDesigner.callBaseMethod(this, 'onPointerUp', [sender, e]);
        handled = true;
        if (this.get_targetObject().get_points().get_count() >= 3) {
            this.get_targetObject().set_tag(null);
            this.endWorking();
        }
        return handled;
    }
}
CustomAnnotations.AnnTriangleDrawDesigner.registerClass('CustomAnnotations.AnnTriangleDrawDesigner', lt.Annotations.Designers.AnnDrawDesigner);

////////////////////////////////////////////////////////////////////////////////
// AnnTriangleEditDesigner
// We won't actually need to do any customization of this class.
CustomAnnotations.AnnTriangleEditDesigner = function AnnTriangleEditDesigner(automationControl, container, annPolylineObject) {
    AnnTriangleEditDesigner.initializeBase(this, [automationControl, container, annPolylineObject]);
}
CustomAnnotations.AnnTriangleEditDesigner.registerClass('CustomAnnotations.AnnTriangleEditDesigner', lt.Annotations.Designers.AnnPolylineEditDesigner);

////////////////////////////////////////////////////////////////////////////////
// AnnTriangleRenderer
CustomAnnotations.AnnTriangleRenderer = function AnnTriangleRenderer() {
    AnnTriangleRenderer.initializeBase(this);
}
CustomAnnotations.AnnTriangleRenderer.prototype = {

    // Override the Render method in order to draw the 3 points as the user creates them.    
    render: function AnnTriangleRenderer$render(mapper, annObject) {
        CustomAnnotations.AnnTriangleRenderer.callBaseMethod(this, 'render', [mapper, annObject]);
        // if we are finished 'drawing', allow the base class AnnPolylineObjectRenderer to handle the job
        if (annObject.get_tag() !== 'drawing') {
            return;
        }
        var engine = Type.safeCast(this.get_renderingEngine(), lt.Annotations.Rendering.AnnHtml5RenderingEngine);
        if (engine != null) {
            var context = engine.get_context();
            if (context != null) {
                context.save();
                var points = mapper.pointsFromContainerCoordinates(annObject.get_points().toArray(), annObject.get_fixedStateOperations());
                lt.Annotations.Rendering.AnnHtml5RenderingEngine.setStroke(context, lt.Annotations.Core.AnnStroke.create(lt.Annotations.Core.AnnSolidColorBrush.create('green'), lt.LeadLengthD.create(1)));
                context.beginPath();
                for (var x = 0; x < points.length; x++) {
                    var point = points[x];
                    if (!point.get_isEmpty()) {
                        var rect = lt.LeadRectD.create(point.get_x() - 10, point.get_y() - 10, 20, 20);
                        lt.Annotations.Rendering.AnnHtml5RenderingEngine.drawEllipse(context, rect);
                    }
                }
                context.closePath();
                context.stroke();
                context.restore();
            }
        }
    }
}

CustomAnnotations.AnnTriangleRenderer.registerClass('CustomAnnotations.AnnTriangleRenderer', lt.Annotations.Rendering.AnnPolylineObjectRenderer);

////////////////////////////////////////////////////////////////////////////////
// AnnSvgTriangleRenderer

CustomAnnotations.AnnSvgTriangleRenderer = function AnnSvgTriangleRenderer() {
    CustomAnnotations.AnnSvgTriangleRenderer.initializeBase(this);
}
CustomAnnotations.AnnSvgTriangleRenderer.prototype = {

    addObject: function AnnSvgTriangleRenderer$addObject(annObject) {
        if (this.get_renderingEngine() != null) {
            CustomAnnotations.AnnSvgTriangleRenderer.callBaseMethod(this, 'addObject', [annObject]);
            var g = lt.Annotations.Rendering.AnnSvgHelpers.getElementById(annObject.get_stateId());
            var id = g.id;
            var ellipseGroup = lt.Annotations.Rendering.AnnSvgHelpers.createGroup();
            ellipseGroup.id = lt.Annotations.Rendering.AnnSvgHelpers.getGroupId(annObject.get_stateId());
            lt.Annotations.Rendering.AnnSvgHelpers.appendElement(g, ellipseGroup);
        }
    },

    removeObject: function AnnSvgTriangleRenderer$removeObject(annObject) {
        if (this.get_renderingEngine() != null) {
            var objectElement = lt.Annotations.Rendering.AnnSvgHelpers.getElementById(annObject.get_stateId());
            objectElement.removeChild(lt.Annotations.Rendering.AnnSvgHelpers.getElementById(lt.Annotations.Rendering.AnnSvgHelpers.getGroupId(annObject.get_stateId())));
            CustomAnnotations.AnnSvgTriangleRenderer.callBaseMethod(this, 'removeObject', [annObject]);
        }
    },

    render: function AnnSvgTriangleRenderer$render(mapper, annObject) {
        CustomAnnotations.AnnSvgTriangleRenderer.callBaseMethod(this, 'render', [mapper, annObject]);
        var ellipseGroup = lt.Annotations.Rendering.AnnSvgHelpers.getElementById(lt.Annotations.Rendering.AnnSvgHelpers.getGroupId(annObject.get_stateId()));
        if (annObject.get_tag() !== 'drawing') {
            while (ellipseGroup.hasChildNodes()) {
                ellipseGroup.removeChild(ellipseGroup.lastChild);
            }
            return;
        }
        var engine = Type.safeCast(this.get_renderingEngine(), lt.Annotations.Rendering.AnnSvgRenderingEngine);
        if (engine != null && !engine.get_stateless()) {
            while (ellipseGroup.hasChildNodes()) {
                ellipseGroup.removeChild(ellipseGroup.lastChild);
            }
            if (ellipseGroup != null) {
                var points = mapper.pointsFromContainerCoordinates(annObject.get_points().toArray(), annObject.get_fixedStateOperations());
                if (points != null) {
                    for (var x = 0; x < points.length; x++) {
                        var point = points[x];
                        if (!point.get_isEmpty()) {
                            var rect = lt.LeadRectD.create(point.get_x() - 10, point.get_y() - 10, 20, 20);
                            var ellipse = lt.Annotations.Rendering.AnnSvgHelpers.createEllipse();
                            var rx = rect.get_width() / 2;
                            var ry = rect.get_height() / 2;
                            var cx = rect.get_left() + rx;
                            var cy = rect.get_top() + ry;
                            ellipse.setAttribute('cx', cx);
                            ellipse.setAttribute('cy', cy);
                            ellipse.setAttribute('rx', rx);
                            ellipse.setAttribute('ry', ry);
                            ellipse.setAttribute('stroke', 'green');
                            ellipse.setAttribute('fill', 'none');
                            ellipse.setAttribute('stroke-width', '1');
                            ellipseGroup.appendChild(ellipse);
                        }
                    }
                }
            }
        }
    }
}


CustomAnnotations.AnnSvgTriangleRenderer.registerClass('CustomAnnotations.AnnSvgTriangleRenderer', lt.Annotations.Rendering.AnnSvgPolylineObjectRenderer);

// Create the custom automation object and hook the designers
function createTriangleAutomationObject(annObject) {
    var automationObj = new lt.Annotations.Automation.AnnAutomationObject();
    automationObj.set_id(annObject.get_id());
    automationObj.set_name("Triangle");
    automationObj.set_drawDesignerType(CustomAnnotations.AnnTriangleDrawDesigner); // hook the custom draw designer
    automationObj.set_editDesignerType(CustomAnnotations.AnnTriangleEditDesigner); // hook the custom edit designer
    automationObj.set_runDesignerType(lt.Annotations.Designers.AnnRunDesigner);

    var annTriangleRenderer = new CustomAnnotations.AnnTriangleRenderer();
    var annPolylineRenderer = _html5RenderingEngine.get_renderers()[lt.Annotations.Core.AnnObject.polylineObjectId];

    annTriangleRenderer.set_locationsThumbStyle(annPolylineRenderer.get_locationsThumbStyle());
    annTriangleRenderer.set_rotateCenterThumbStyle(annPolylineRenderer.get_rotateCenterThumbStyle());
    annTriangleRenderer.set_rotateGripperThumbStyle(annPolylineRenderer.get_rotateGripperThumbStyle());

    _html5RenderingEngine.get_renderers()[annObject.get_id()] = annTriangleRenderer;   // hook the custom renderer
    automationObj.set_objectTemplate(annObject);

    var annSvgTriangleRenderer = new CustomAnnotations.AnnSvgTriangleRenderer();
    var annSvgPolylineRenderer = _svgRenderingEngine.get_renderers()[lt.Annotations.Core.AnnObject.polylineObjectId];

    annSvgTriangleRenderer.set_locationsThumbStyle(annSvgPolylineRenderer.get_locationsThumbStyle());
    annSvgTriangleRenderer.set_rotateCenterThumbStyle(annSvgPolylineRenderer.get_rotateCenterThumbStyle());
    annSvgTriangleRenderer.set_rotateGripperThumbStyle(annSvgPolylineRenderer.get_rotateGripperThumbStyle());

    _html5RenderingEngine.get_renderers()[annObject.get_id()] = annTriangleRenderer;   // hook the custom renderer
    annTriangleRenderer.initialize(_html5RenderingEngine);

    _svgRenderingEngine.get_renderers()[annObject.get_id()] = annSvgTriangleRenderer;   // hook the custom renderer
    annSvgTriangleRenderer.initialize(_svgRenderingEngine);

    return automationObj;
}


function isTouchDevice() {
    try {
        document.createEvent('TouchEvent');
        return 'ontouchstart' in document.documentElement;
    } catch (ex) {
        return false;
    }
}

function touchScroll(div) {
    if (isTouchDevice()) {
        div.addEventListener('touchstart', touchstart, false);
        div.addEventListener('touchmove', touchmove, false);
    }
}

function touchmove(e) {
    /// <param name="e" type="ElementEvent">
    /// </param>
    var targetTouchePageY = e.touches[0].pageY;
    var targetTouchePageX = e.touches[0].pageX;
    if ((e.currentTarget.scrollTop < e.currentTarget.scrollHeight - e.currentTarget.offsetHeight && e.currentTarget.scrollTop + targetTouchePageY < this._scrollStartPosY - 5) || (!!e.currentTarget.scrollTop && e.currentTarget.scrollTop + targetTouchePageY > this._scrollStartPosY + 5)) {
        e.preventDefault();
    }
    if ((e.currentTarget.scrollLeft < e.currentTarget.scrollWidth - e.currentTarget.offsetWidth && e.currentTarget.scrollLeft + targetTouchePageX < this._scrollStartPosX - 5) || (!!e.currentTarget.scrollLeft && e.currentTarget.scrollLeft + targetTouchePageX > this._scrollStartPosX + 5)) {
        e.preventDefault();
    }
    e.currentTarget.scrollTop = this._scrollStartPosY - targetTouchePageY;
    e.currentTarget.scrollLeft = this._scrollStartPosX - targetTouchePageX;
}

function touchstart(e) {
    /// <param name="e" type="ElementEvent">
    /// </param>
    var targetTouchePageY = e.touches[0].pageY;
    var targetTouchePageX = e.touches[0].pageX;
    this._scrollStartPosY = e.currentTarget.scrollTop + targetTouchePageY;
    this._scrollStartPosX = e.currentTarget.scrollLeft + targetTouchePageX;
}
function setImage(imagePath, reinitialize) {
    try {
        if (reinitialize) {
            $('#imageViewerContainer').empty();
            IsFirstRun = true;
        }
        window.onload();

        this.imagesData = new Object();
        this.imagesData[imagePath] = new imageData(imagePath, 100, 100);

        var ids = $('#ImagesIds');
        for (var id in imagesData) {
            createAutomation();
            var data = imagesData[id];
            ids.append($('<option></option>').val(data.imageId).html(data.imageId));
        }

        $("select option:contains(" + imagePath.toLowerCase() + ")").attr('selected', true);
        onImageChanged();
    }
    catch (ex) {
    }
}


// BS - Functions added by Yogeesha

// Zoom in/out buttons:
var zoomFactor = 1.2;

function zoomIn() {
    try {
        var viewer = this._imageViewer;

        var interactiveMode = new lt.Controls.ImageViewerPanZoomInteractiveMode();
        viewer.set_defaultInteractiveMode(interactiveMode);

        viewer.zoom(lt.Controls.ImageViewerSizeMode.none, viewer.get_currentScaleFactor() * zoomFactor, viewer.get_defaultZoomOrigin());
    } catch (ex) {
    }
}

function zoomOut() {
    try {
        var viewer = this._imageViewer;

        var interactiveMode = new lt.Controls.ImageViewerPanZoomInteractiveMode();
        viewer.set_defaultInteractiveMode(interactiveMode);

        viewer.zoom(lt.Controls.ImageViewerSizeMode.none, viewer.get_currentScaleFactor() / zoomFactor, viewer.get_defaultZoomOrigin());
    } catch (ex) {
    }
}

function zoomViewer() {
    try {
        var viewer = this._imageViewer;
        var zoomSelect = document.getElementById("zoomSelect");
        switch (zoomSelect.selectedIndex) {
            case 0:  // Actual size
                viewer.zoom(lt.Controls.ImageViewerSizeMode.actualSize, 1, viewer.get_defaultZoomOrigin());
                zoomSelect.selectedIndex = 0;
                break;
            case 1:  // Fit page
                viewer.zoom(lt.Controls.ImageViewerSizeMode.fit, 1, viewer.get_defaultZoomOrigin());
                zoomSelect.selectedIndex = 1;
                break;

            case 2: // Fit width
                viewer.zoom(lt.Controls.ImageViewerSizeMode.fitWidth, 1, viewer.get_defaultZoomOrigin());
                zoomSelect.selectedIndex = 2;
                break;

            case 3: // Current
                var percentage = parseInt(zoomSelect.value.replaceAll("%", "").replaceAll("Current: ", ""));
                viewer.zoom(lt.Controls.ImageViewerSizeMode.None, percentage / 100, viewer.get_defaultZoomOrigin());
                break;

            default:
                var percentage = parseInt(zoomSelect.value.replaceAll("%", ""));
                viewer.zoom(lt.Controls.ImageViewerSizeMode.None, percentage / 100, viewer.get_defaultZoomOrigin());
                break;
        }
    } catch (ex) {
    }
}

// BE - Functions added by Yogeesha


// Initialize viewer objects
var IsFirstRun = true;
window.onload = function () {

    if (IsFirstRun) {
        IsFirstRun = false;
        this._leadtoolsRESTServicesHostAppName = 'LOTEXServicesHost';
        this._leadtoolsRESTServicesHost = null;
        _imageViewer = new lt.Controls.ImageViewer(new lt.Controls.ImageViewerCreateOptions($("#imageViewerContainer")[0].id, 'ImageViewer'));

        _imageViewer.set_imageHorizontalAlignment(lt.Controls.ControlAlignment.center);

        var optionsToolBarDiv = document.getElementById('optionsToolBar');
        var annObjectsToolBarDiv = document.getElementById('annObjectsToolBar');
        touchScroll(optionsToolBarDiv);
        touchScroll(annObjectsToolBarDiv);

        var imageControlsOkButton = document.getElementById('imageControlsOkButton');

        // BS - Commented by Yogeesha to hide annotation properties
        //        imageControlsOkButton.addEventListener('click', function (e) {
        //            showDialog('imageControlsDiv', false);
        //        }, false);
        // BE - Commented by Yogeesha to hide annotation properties

        updateLayoutSize();

        if (lt.LTHelper.OS == lt.LTOS.android) {
            $('#annLoadOption').remove();
            $('#annSaveOption').remove();
        }
        if (lt.LTHelper.OS !== lt.LTOS.iOS) {
            _imageViewer.set_useDpi(true);
        }
        else {
            _imageViewer.set_useDpi(false);
        }

        _imageViewer.add_imageError(function (sender, e) {
            var image = e.get_nativeElementEvent().srcElement;
            alert('Cannot open: ' + image.src);
            endOperation(false);
        });

        _imageViewer.add_imageChanged(function (sender, e) {
            endOperation(true);
        });

        $(window).bind("resize", onContentSizeChange);
        $(window).bind("orientationchange", onContentSizeChange);

        $(window).bind("blur", function () {
            imageViewerAutomationControl.handleLostFocus();
        });

        $("#btnDone").bind("click", onPropertiesChanged);
        $("#btnOk").bind("click", onBtnSubmitPassword);
        $("#btnCancel").bind("click", onBtnCancelPassword);
        $("#btnClose").bind("click", function () {
            $("#mediaPlayerPage").dialog('close');
        });
        $("#page1").live('pageshow', function (event) {
            var hidScroll = document.getElementById('imageViewerContainer');
            $("#ImageViewer_div").scrollTop(_scrollTop);
            onContentSizeChange();
            _imageViewer.invalidate();
            window.setTimeout(function () {
                imageViewerAutomationControl.automationInvalidate();
            }, 200);
        });

        if (_automationManager == null) {
            _automationManager = new lt.Annotations.Automation.AnnAutomationManager();
            _automationManager.createDefaultObjects();
        }
        /*   else
        {
        _automationManager.set_renderingEngine(null);
        _activeAutomation = null;
        _automationManager.get_automations().clear();
        }*/

        if (_svgRenderingEngine == null)
            _svgRenderingEngine = new lt.Annotations.Rendering.AnnSvgRenderingEngine();

        if (_html5RenderingEngine == null)
            _html5RenderingEngine = new lt.Annotations.Rendering.AnnHtml5RenderingEngine();

        if (_enableSvg) {
            _automationManager.set_renderingEngine(_svgRenderingEngine);
            imageViewerAutomationControl = new lt.Annotations.Automation.ImageViewerSvgAutomationControl(_imageViewer, _svgRenderingEngine);
        }
        else {
            _automationManager.set_renderingEngine(_html5RenderingEngine);
            imageViewerAutomationControl = new lt.Annotations.Automation.ImageViewerAutomationControl(_imageViewer, _html5RenderingEngine);
        }

        imageViewerAutomationControl.add_automationGotFocus(automation_GotFocus);
        imageViewerAutomationControl.add_automationTransformChanged(automation_TransformChanged);

        _imageViewer.set_defaultInteractiveMode(imageViewerAutomationControl);

        // register custom triangle object
        // Create a triangle object
        var triangle = new CustomAnnotations.AnnTriangleObject();
        // Create user defined automation object
        var triangleAutomation = createTriangleAutomationObject(triangle);

        var automationObjects = _automationManager.get_objects();
        automationObjects.add(triangleAutomation);

        // Set http://www as the default hyperlink for all object templates
        // Set the default stroke thickness to 2

        var isDesktop = (lt.LTHelper.device === lt.LTDevice.desktop);

        var automationObjectsCount = automationObjects.get_count();

        for (var i = 0; i < automationObjectsCount; ++i) {
            var automationObject = automationObjects.get_item(i);
            var annObjectTemplate = automationObject.get_objectTemplate();

            if (annObjectTemplate != null) {

                if (!isDesktop && annObjectTemplate.get_supportsStroke()) {
                    var stroke = annObjectTemplate.get_stroke();

                    stroke.set_strokeThickness(lt.LeadLengthD.create(2));
                    annObjectTemplate.set_stroke(stroke);
                }

                if (Type.canCast(annObjectTemplate, lt.Annotations.Core.AnnAudioObject)) {
                    var audioObject = Type.safeCast(annObjectTemplate, lt.Annotations.Core.AnnAudioObject);
                    audioObject.get_media().set_source1("http://demo.lotex.co.in/media/mp3/NewAudio.mp3");
                    audioObject.get_media().set_type1("audio/mp3");
                    audioObject.get_media().set_source2("http://demo.lotex.co.in/media/wav/newaudio.wav");
                    audioObject.get_media().set_type2("audio/wav");
                    audioObject.get_media().set_source3("http://demo.lotex.co.in/media/OGG/NewAudio_uncompressed.ogg");
                    audioObject.get_media().set_type3("audio/ogg");
                }
                else if (Type.canCast(annObjectTemplate, lt.Annotations.Core.AnnMediaObject)) {
                    var videoObject = Type.safeCast(annObjectTemplate, lt.Annotations.Core.AnnMediaObject);
                    videoObject.get_media().set_source1("http://demo.lotex.co.in/media/mp4/dada_h264.mp4");
                    videoObject.get_media().set_type1("video/mp4");
                    videoObject.get_media().set_source2("http://demo.lotex.co.in/media/WebM/DaDa_VP8_Vorbis.mkv");
                    videoObject.get_media().set_type2("video/webm");
                    videoObject.get_media().set_source3("http://demo.lotex.co.in/media/OGG/DaDa_Theora_Vorbis.ogg");
                    videoObject.get_media().set_type3("video/ogg");
                }

                annObjectTemplate.set_hyperlink("http://www");
            }
        }

        /* //Commented by yogeesha  
        this.imagesData = new Object();
        for (var i = 1; i <= PagesCount; i++) {
        var imgFullPath = imageLocation.toLowerCase() + i + '.jpg';
        this.imagesData[imgFullPath] = new imageData(imgFullPath, 300, 300);
        //this.imagesData["Images/PngImage.png"] = new imageData("Images/PngImage.png", 72, 72);
        }

        var ids = $('#ImagesIds');
        for (var id in imagesData) {
        createAutomation();
        var data = imagesData[id];
        ids.append($('<option></option>').val(data.imageId).html(data.imageId));
        }
        */


        //    ids.selectedIndex = 0;
        //    onImageChanged();
        document.onkeydown = keyDown; // handle the keydown event
        document.oncontextmenu = function () { return false; }; // disable the default context menu
        var interactiveService = _imageViewer.get_interactiveService();
        interactiveService.set_preventContextMenu(true); // disable the default context menu
        if (lt.LTHelper.supportsTouch) {
            interactiveService.set_enableHold(true); // enable hold (for touch devices)
            interactiveService.add_hold(function (sender, e) {
                if (_activeAutomation.get_manager().get_userMode() !== lt.Annotations.Core.AnnUserMode.run) {
                    myContextMenu(e.get_position().get_x(), e.get_position().get_y());
                }
            });
        }
        if (lt.LTHelper.supportsMouse) {
            // create our own right click interactive mode and set it to the viewer control
            var rightClick = new RightClickInteractiveMode();
            _imageViewer.setMouseInteractiveMode(lt.Controls.MouseButton.right, rightClick);
        }

        if (lt.LTHelper.device === lt.LTDevice.mobile || lt.LTHelper.device === lt.LTDevice.tablet) {
            document.documentElement.style.webkitTouchCallout = "none";
            document.documentElement.style.webkitUserSelect = "none";
        }

        // Toolbar stuff
        if ((lt.LTHelper.device == lt.LTDevice.tablet) || (lt.LTHelper.device == lt.LTDevice.desktop)) {
            var toolbar = document.getElementById('toolRibbon1');
            updateToolbar();
        }

        $("#useSVGRendering").click(function () {

            if (_imageViewer != null) {
                var interactiveMode = _imageViewer.get_defaultInteractiveMode();
                interactiveMode.stop(_imageViewer);

                if (_activeAutomation != null) {
                    _activeAutomation.detach();
                }
            }

            if ($(this).prop('checked')) {
                _enableSvg = true;
            }
            else {
                _enableSvg = false;
            }

            imageViewerAutomationControl.set_renderingEngine(null);
            if (_enableSvg) {
                _automationManager.set_renderingEngine(_svgRenderingEngine);
                imageViewerAutomationControl = new lt.Annotations.Automation.ImageViewerAutomationControl(_imageViewer, _svgRenderingEngine);
            }
            else {
                _automationManager.set_renderingEngine(_html5RenderingEngine);
                imageViewerAutomationControl = new lt.Annotations.Automation.ImageViewerAutomationControl(_imageViewer, _html5RenderingEngine);
            }


            _activeAutomation = _automationManager.get_automations().get_item($("#ImagesIds")[0].selectedIndex);

            _imageViewer.set_defaultInteractiveMode(imageViewerAutomationControl);
            _activeAutomation.attach(imageViewerAutomationControl);
            imageViewerAutomationControl.add_automationGotFocus(automation_GotFocus);
        });
    }
}

function onPropertiesChanged(event, ui) {
    closeProperties();
}

function onBtnCancelPassword(event, ui) {
    _propertiesVisible = false;
    $("#passwordPage").dialog('close');
}

function onBtnSubmitPassword(event, ui) {
    var inputVal = $("#password").val();
    var annObject = $("#passwordPage").data("annObject");
    var lock = $("#passwordPage").data("Lock");
    if (lock) {
        if (inputVal === "") {
            alert("Enter a password with at least 1 character.");
        }
        else {
            annObject.lock(inputVal);
            _propertiesVisible = false;
            $("#passwordPage").dialog('close');
            updateToolbar();
        }
    }
    else {
        if (annObject.get_isLocked()) {
            annObject.unlock(inputVal);
            if (annObject.get_isLocked()) {
                alert("Incorrect password.");
            }
            else {
                _propertiesVisible = false;
                $("#passwordPage").dialog('close');
            }
            updateToolbar();
        }
    }

    if (_activeAutomation && _activeAutomation.get_automationControl()) {
        _activeAutomation.get_automationControl().automationInvalidate();
    }
}

function onBtnDelete(event, ui) {
    if (canDeleteObject()) {
        _activeAutomation.deleteSelectedObjects();
    }
}

function onObjectClicked(objectIndex) {
    if (objectIndex === 0) {
        _imageViewer.set_defaultInteractiveMode(new lt.Controls.ImageViewerPanZoomInteractiveMode());
    }
    else {
        var interactive = _imageViewer.get_defaultInteractiveMode();

        if (interactive != imageViewerAutomationControl) {
            if (interactive != null)
                interactive.stop(_imageViewer);
            _imageViewer.set_defaultInteractiveMode(imageViewerAutomationControl);
            _activeAutomation.attach(imageViewerAutomationControl);
        }

        _activeAutomation.get_manager().set_currentObjectId(objectIndex);
    }
}

function DetectIPhones() {

    var ua = navigator.userAgent.toLowerCase();

    var isIphone = ua.indexOf("iphone") > -1;

    var isIPod = ua.indexOf("iPod ") > -1;



    return isIphone || isIPod;
}

function updateLayoutSize() {
    var windowHeight = window.innerHeight ? window.innerHeight : $(window).height();

    var viewerPadding = ($("#Content").outerHeight() - $("#Content").height());

    var toolRibbon1Height = 0;
    var toolRibbon2Height = 0;

    var optionsToolBar = document.getElementById('optionsToolBar');
    var annObjectsToolBar = document.getElementById('annObjectsToolBar');

    $("#imageViewerContainer").height(windowHeight);

    if (optionsToolBar.offsetWidth < optionsToolBar.scrollWidth) {
        $("#toolRibbon1").height(optionsToolBar.offsetHeight);
        toolRibbon1Height = optionsToolBar.offsetHeight;
    } else {
        $("#toolRibbon1").height(optionsToolBar.scrollHeight);
        toolRibbon1Height = optionsToolBar.scrollHeight
    }

    if (annObjectsToolBar.offsetWidth < annObjectsToolBar.scrollWidth) {
        $("#toolRibbon2").height(annObjectsToolBar.offsetHeight);
        toolRibbon2Height = annObjectsToolBar.offsetHeight;
    } else {
        $("#toolRibbon2").height(annObjectsToolBar.scrollHeight);
        toolRibbon2Height = annObjectsToolBar.scrollHeight;
    }

    $("#Content").height(windowHeight - toolRibbon1Height - toolRibbon2Height - viewerPadding);
    $("#imageViewerContainer").height(windowHeight - toolRibbon1Height - toolRibbon2Height - viewerPadding);

    _imageViewer.onSizeChanged(lt.LeadEventArgs.Empty);
}

function onContentSizeChange() {
    window.setTimeout(updateLayoutSize, 200);
}

function onInputFileChange() {
    var fileBrowser = document.getElementById('inputFileBrowser');
    var content = null;

    if (window.File && window.FileReader && window.FileList && window.Blob) {
        var file = fileBrowser.files[0];
        if (file != null) {
            var reader = new FileReader();
            var readerLoad = function () {
                content = reader.result;
                reader.onload = null;
                loadAnnotations(content, 1);
            };
            reader.onload = readerLoad;
            reader.readAsText(file, 'UTF-8');
        }
    }
    else if (typeof (window.ActiveXObject) != "undefined") {
        var fileName = fileBrowser.value;
        try {
            var fso = new ActiveXObject('Scripting.FileSystemObject');
            var file = fso.OpenTextFile(fileName, 1, false);
            content = file.ReadAll();
            file.Close();

            loadAnnotations(decode_utf8(content), 1);
        }
        catch ($e1) {
            alert('Unable to access local files due to browser security settings.\n' + 'Try the following fixes:\n' + '- Go to Tools->Internet Options->Security->Trusted Sites\n' + 'And add this address to the Trusted sites.\n\n' + '- Go to Tools->Internet Options->Security->Custom Level.\n' + 'Find the setting for "Initialize and script ActiveX controls not marked as safe" and change it to "Enable" or "Prompt"');
        }
    }
    else {
        alert('Your browser does not support HTML 5 FileReader API or ActiveX objects. Load & Save is disabled.');
    }
}

function loadAnnotations(content, pageNumber) {
    try {

        //BS -Below lines added by yogeesha because annotations were not loading
        _activeAutomation = _automationManager.get_automations().get_item($("#ImagesIds")[0].selectedIndex);
        _activeAutomation.set_active(true);
        //BE -Above lines added by yogeesha because annotations were not loading

        var codecs = new lt.Annotations.Core.AnnCodecs();
        var con = codecs.load(content, pageNumber);

        var srcChildren = con.get_children();

        if (srcChildren.get_count() > 0) {
            var destChildren = _activeAutomation.get_container().get_children();
            destChildren.clear();
            var scale = _imageViewer.get_backCanvasScale();
            var point = lt.LeadPointD.create(0, 0);
            for (var i = 0; i < srcChildren.get_count(); i++) {
                var child = srcChildren.get_item(i);
                // Comment out this code if you need to scale the objects after loading.
                /*if(scale!=1)
                child.scale(scale,scale, point);*/
                destChildren.add(child);
            }
        }

        for (var i = 0; i < con.get_layers().get_count(); i++) {
            _activeAutomation.get_container().get_layers().add(con.get_layers().get_item(i));
        }
        _activeAutomation.get_automationControl().automationInvalidate();
        updateToolbar();
    }
    catch ($e1) {
        alert('File does not contain valid annotation data.');
    }
}

function onFileChanged() {
    if (lt.LTHelper.device === lt.LTDevice.mobile || lt.LTHelper.device === lt.LTDevice.tablet) {
        alert('Load and Save annotations file from file system is not supported by this device. Try this demo on a desktop browser to check this feature.');
        return;
    }

    var operation = $("#File").val();

    var myselect = $("select#File");
    if (myselect[0].selectedIndex != 0) {
        myselect[0].selectedIndex = 0;
        myselect.selectmenu("refresh");
    }
    else {
        _imageViewer.set_imageUrl(operation);

        if (_activeAutomation != null)
            _activeAutomation.set_active(false);
        _activeAutomation = _automationManager.get_automations().get_item($("#File")[0].selectedIndex + 1);

        _activeAutomation.set_active(true);
    }
}

function postData(xml, ext) {
    /// <param name="xml" type="String">
    /// </param>
    var request = new XMLHttpRequest();
    var _this = this;
    var readyStateChanged = function () {
        if (request.readyState === 4) {
            if (request.status === 200) {
                if (!String.isNullOrEmpty(request.responseText)) {
                    var fileUrl = JSON.parse(request.responseText);
                    _this.downloadFile(fileUrl, ext);
                }
                else {
                    alert('Error saving the file');
                }
            }
            else {
                _this.showRequestError(request);
            }
            request.onreadystatechange = null;
            request = null;
        }
    };
    var rest = new ss.StringBuilder();
    rest.append(buildServiceUrl('Data.svc'));
    rest.append('/SaveXml');
    var url = rest.toString();
    request.onreadystatechange = readyStateChanged;
    request.open('POST', url, true);
    request.setRequestHeader('Content-type', 'text/xml');

    request.send(xml);
}

function buildServiceUrl(serviceName) {
    var serviceUrl;
    if (String.isNullOrEmpty(this._leadtoolsRESTServicesHost)) {
        serviceUrl = String.format('{0}//{1}/{2}/{3}', window.location.protocol, window.location.host, this._leadtoolsRESTServicesHostAppName, serviceName);
    }
    else {
        serviceUrl = String.format('{0}/{1}/{2}', this._leadtoolsRESTServicesHost, this._leadtoolsRESTServicesHostAppName, serviceName);
    }
    return serviceUrl;
}

function showRequestError(request) {
    /// <param name="request" type="XMLHttpRequest">
    /// </param>
    alert(String.format('Error {0}\n{1}', request.status, request.statusText));
    request = null;
}


function downloadFile(url, ext) {
    /// <param name="url" type="String">
    /// </param>
    var rest = new ss.StringBuilder();
    rest.append(buildServiceUrl('Data.svc'));
    rest.append('/Download?mime=text/xml&name=Annotations.' + ext + '&url=');
    rest.append(url);
    var oWin = openWindow(rest.toString());
    if (oWin == null || typeof oWin == 'undefined') {
        alert('Your Popup Blocker has blocked saving the annotation file to your disk. Disable the Popup Blocker for this web site and try again.');
    }
}

function burn(canvas, renderingEngine) {
    var context = canvas.getContext('2d');
    renderingEngine.attach(_activeAutomation.get_container(), context);
    if (renderingEngine != null) {
        var dpiX = _imageViewer.get_screenDpiX();
        var dpiY = _imageViewer.get_screenDpiY();
        var xRes = _imageViewer.get_imageDpiX();
        var yRes = _imageViewer.get_imageDpiY();
        renderingEngine.burnToRectWithDpi(lt.LeadRectD.get_empty(), dpiX, dpiY, xRes, yRes);
    }
}

function burnAnnotations() {
    if (_activeAutomation != null && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        var imageCanvas = _imageViewer.get_backCanvas();
        var renderingEngine = new lt.Annotations.Rendering.AnnHtml5RenderingEngine();
        renderingEngine.set_renderers(_html5RenderingEngine.get_renderers());
        burn(imageCanvas, renderingEngine);
        _imageViewer.invalidate();
    }
}


function onActionClicked(actionName) {
    if ((actionName != "designMode" && actionName != "runMode") && _activeAutomation.get_manager().get_userMode() == lt.Annotations.Core.AnnUserMode.run) {
        return;
    }

    if (actionName == 'load') {
        $('#inputFileBrowser').remove();
        $("#fileInput").append('<input type="file" id="inputFileBrowser" accept="text/xml" value="Load" change="onInputFileChange()"/>');

        $("#inputFileBrowser").bind('change', onInputFileChange);

        //$("#inputFileBrowser").val('');
        window.setTimeout(function () {
            $("#inputFileBrowser").click()
        }, 200);
    }
    else if (actionName == 'save') {
        var codecs = new lt.Annotations.Core.AnnCodecs();
        var xmlString = codecs.save(_activeAutomation.get_container(), 1, null, 1);

        postData(xmlString, 'xml');
    }
    else if (actionName == "burnAnnotations") {
        burnAnnotations()
    }
    else if (actionName == "saveImage") {
        var imageCanvas = _imageViewer.get_backCanvas();
        var myImage = imageCanvas.toDataURL("image/png");
        return myImage; // Line added by Yogeesha
        // BS - Commented by Yogeesha
        //      var oWin = window.open();
        //      if (oWin == null || typeof oWin == 'undefined') {
        //         alert('Your Popup Blocker has blocked saving the image. Disable the Popup Blocker for this web site and try again.');
        //      }
        //      else {
        //          oWin.document.write('<img src="' + myImage + '"/>');          
        //      }
        // BE - Commented by Yogeesha
    }
    else if (actionName == "undo") {
        annUndo();
    }
    else if (actionName == "redo") {
        annRedo();
    }
    else if (actionName == "unlock") {
        var editObject = _activeAutomation.get_currentEditObject();
        if (editObject != null &&
           (editObject.get_id() === lt.Annotations.Core.AnnObject.selectObjectId)) {
            alert('Cannot unlock objects in a group.');
            return;
        }

        if (_activeAutomation.get_canUnlock()) {
            _activeAutomation.unlock();
            updateToolbar();
        }
        else {
            alert("Object is already unlocked or no object is selected.");
        }
    }
    else if (actionName == "lock") {
        _activeAutomation.lock();
        updateToolbar();
    }
    else if (actionName == "delete") {
        annDelete();
    }
    else if (actionName == "properties") {
        var automation = _activeAutomation;
        if (automation.get_canShowProperties()) {
            showProperties();
        }
        else {
            alert("Select an annotation object first.");
        }
    }
    else if (actionName == "realizeRedact") {
        var editObject = _activeAutomation.get_currentEditObject();
        if (editObject != null && editObject.get_id() == lt.Annotations.Core.AnnObject.redactionObjectId && _activeAutomation.get_canRealizeRedaction()) {
            _activeAutomation.realizeRedaction();
        }
        updateToolbar();
    }
    else if (actionName == "restoreRedact") {
        var editObject = _activeAutomation.get_currentEditObject();
        if (editObject != null && editObject.get_id() == lt.Annotations.Core.AnnObject.redactionObjectId && _activeAutomation.get_canRestoreRedaction()) {
            _activeAutomation.restoreRedaction();
        }
        updateToolbar();
    }
    else if (actionName == "applyEncryptor") {
        var editObject = _activeAutomation.get_currentEditObject();
        if (editObject != null && editObject.get_id() == lt.Annotations.Core.AnnObject.encryptObjectId && _activeAutomation.get_canApplyEncryptor()) {
            _activeAutomation.applyEncryptor();
        }
        updateToolbar();
    }
    else if (actionName == "applyDecryptor") {
        var editObject = _activeAutomation.get_currentEditObject();
        if (editObject != null && editObject.get_id() == lt.Annotations.Core.AnnObject.encryptObjectId && _activeAutomation.get_canApplyDecryptor()) {
            _activeAutomation.applyDecryptor();
        }
        updateToolbar();
    }
    else if (actionName == "runMode") {
        if (_activeAutomation != null) {
            _activeAutomation.get_manager().set_userMode(lt.Annotations.Core.AnnUserMode.run);
            updateToolbar();
        }
    }
    else if (actionName == "designMode") {
        if (_activeAutomation != null) {
            _activeAutomation.get_manager().set_userMode(lt.Annotations.Core.AnnUserMode.design);
            updateToolbar();
            if (_audio != null) {
                _audio.pause();
            }
            if (_video != null) {
                _video.pause();
            }
        }
    }
}

function onImageChanged() {
    var imageId = $("#ImagesIds").val();

    var imageData = this.imagesData[imageId];

    // beginOperation("Loading image...");
    _imageViewer.set_imageUrl(imageData.imageId);
}

function loadImage() {
    if (_activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        showDialog('imageControlsDiv', true);
    }
}

function showDialog(divName, show) {
    // BS - Commented by Yogeesha to hide annotation properties
//    var overlay = document.getElementById('overlay');
//    overlay.style.visibility = (show) ? 'visible' : 'hidden';
//    var div = document.getElementById(divName);
//    div.style.visibility = (show) ? 'visible' : 'hidden';
    // BS - Commented by Yogeesha to hide annotation properties
}

function beginOperation(message) {
    var loadingText = document.getElementById("loadingText");
    loadingText.textContent = message;

    var imageLoadOverlay = document.getElementById("imageLoadOverlay");
    imageLoadOverlay.style.visibility = "visible";
    var imageLoadDiv = document.getElementById("imageLoadDiv");
    imageLoadDiv.style.visibility = "visible";
}

function endOperation(imageChanged) {
    if (imageChanged) {
        var imageId = $("#ImagesIds").val();
        var imageData = this.imagesData[imageId];

        if (lt.LTHelper.OS !== lt.LTOS.iOS) {
            _imageViewer.set_imageDpiX(imageData.xResolution);
            _imageViewer.set_imageDpiY(imageData.yResolution);
        }

        if (lt.LTHelper.device === lt.LTDevice.mobile || lt.LTHelper.device === lt.LTDevice.tablet) {
            _imageViewer.zoom(lt.Controls.ImageViewerSizeMode.fitWidth, 1, _imageViewer.get_defaultZoomOrigin());
        }

        if (_enableSvg) {
            _automationManager.set_renderingEngine(_svgRenderingEngine);
        }
        else {
            _automationManager.set_renderingEngine(_html5RenderingEngine);
        }

        if (_activeAutomation != null)
            _activeAutomation.set_active(false);
        _activeAutomation = _automationManager.get_automations().get_item($("#ImagesIds")[0].selectedIndex);
        _activeAutomation.set_active(true);

        onContentSizeChange();
    }

    var imageLoadOverlay = document.getElementById("imageLoadOverlay");
    if (imageLoadOverlay.style.visibility !== "hidden") {
        imageLoadOverlay.style.visibility = "hidden";
        var imageLoadDiv = document.getElementById("imageLoadDiv");
        imageLoadDiv.style.visibility = "hidden";
    }
    updateToolbar();
}

function automation_TransformChanged() {
    var parent = document.getElementById(_imageViewer.get_canvasId()).parentNode;
    var child = document.getElementById('textObject');
    if (child && document.activeElement != child) {
        child.userData.set_text(child.value);
        parent.removeChild(child);
        $("#Content").focus();
        child = null;
    }
}

function automation_GotFocus(sender, e) {
    var parent = document.getElementById(_imageViewer.get_canvasId()).parentNode;
    var child = document.getElementById('textObject');
    if (child) {
        child.userData.set_text(child.value);
        parent.removeChild(child);
        $("#Content").focus();
        child = null;
        updateToolbar();
    }
}

function automation_CurrentDesignerChanged(sender, e) {
    var automation = Type.safeCast(sender, lt.Annotations.Automation.AnnAutomation);
    if (automation.get_canShowProperties()) {
        var targetObject = automation.get_currentDesigner().get_targetObject();
        PropertyGrid.EditObject(targetObject);
        var enable = true;
    }
    else {
        PropertyGrid.EditObject(null);
    }
}

function onBtnProperties(event, ui) {
    var automation = _activeAutomation;
    if (automation.get_canShowProperties()) {
        showProperties();
    }
    else {
        alert("Select an annotation object first.");
    }
}

function automation_LockObject(sender, e) {

    var automation = _activeAutomation;
    if (automation.get_canLock()) {
        var element = document.createElement("a");

        element.setAttribute("id", "lockDialog");
        element.setAttribute("href", "#passwordPage");
        element.setAttribute("data-rel", "dialog");
        element.setAttribute("data-transition", "flip");

        var content = document.getElementById("Content");

        content.appendChild(element);
        e.set_cancel(true);
        $("#passwordPage").data("annObject", e.get_object());
        $("#passwordPage").data("Lock", true);
        $("#password").val(null);
        _propertiesVisible = true;
        $("#lockDialog").click();
    }
    else {
        alert("Select an annotation object first.");
    }
}

function automation_UnLockObject(sender, e) {
    var automation = _activeAutomation;
    if (automation.get_canUnlock()) {
        var element = document.createElement("a");

        element.setAttribute("id", "unlockDialog");
        element.setAttribute("href", "#passwordPage");
        element.setAttribute("data-rel", "dialog");
        element.setAttribute("data-transition", "flip");

        var content = document.getElementById("Content");

        content.appendChild(element);
        e.set_cancel(true);
        $("#passwordPage").data("annObject", e.get_object());
        $("#passwordPage").data("Lock", false);
        $("#password").val(null);
        _propertiesVisible = true;
        $("#unlockDialog").click();
    }
    else {
        alert("Select an annotation object first.");
    }
}

function automation_EditText(sender, e) {
    var textElement = document.createElement("textArea");
    textElement.id = 'textObject';
    textElement.style.left = e.get_bounds().get_left() + 'px';
    textElement.style.top = e.get_bounds().get_top() + 'px';
    textElement.style.position = 'absolute';
    textElement.style.zIndex = 100;
    textElement.style.height = String.format("{0}px", e.get_bounds().get_height());
    textElement.style.width = String.format("{0}px", e.get_bounds().get_width());
    textElement.userData = e.get_textObject();
    textElement.value = e.get_textObject().get_text();
    textElement.style.color = e.get_textObject().get_textForeground().get_color();
    textElement.style.fontFamily = e.get_textObject().get_font().get_fontFamilyName();
    textElement.style.fontSize = e.get_textObject().get_font().get_fontSize() + 'pt';
    textElement.wrap = "off";
    var parent = document.getElementById(_imageViewer.get_canvasId()).parentNode;
    parent.appendChild(textElement);
    imageViewerAutomationControl.handleLostFocus();
    textElement.focus();
    textElement.onfocusout = function () {
        var parent = document.getElementById(_imageViewer.get_canvasId()).parentNode;
        var child = document.getElementById('textObject');
        if (child) {
            child.userData.set_text(child.value);
            if (_enableSvg) {
                imageViewerAutomationControl.automationInvalidate();
            }
            parent.removeChild(child);
            $("#Content").focus();
            child = null;
            updateToolbar();
        }
    };

    moveCaretToEnd(textElement);
    updateToolbar();

    // Work around for Chrome
    window.setTimeout(function () {
        moveCaretToEnd(textElement);
    }, 1);
}

function moveCaretToEnd(element) {
    if (typeof element.selectionStart == "number") {
        element.selectionStart = element.selectionEnd = element.value.length;
    } else if (typeof element.createTextRange != "undefined") {
        element.focus();
        var range = element.createTextRange();
        range.collapse(false);
        range.select();
    }
}

function AnnFontToString(annFont) {
    var fontStyle = '';
    switch (annFont.get_fontWeight()) {
        case lt.Annotations.Core.AnnFontWeight.bold:
        case lt.Annotations.Core.AnnFontWeight.extraBold:
        case lt.Annotations.Core.AnnFontWeight.semiBold:
            fontStyle = 'bold';
            break;
        default:
            fontStyle = '';
            break;
    }
    switch (annFont.get_fontStyle()) {
        case lt.Annotations.Core.AnnFontStyle.italic:
            fontStyle += 'italic';
            break;
        default:
            break;
    }

    return String.format('{0} {1}pt {2}', fontStyle, annFont.get_fontSize(), annFont.get_fontFamilyName());
}

function openWindow(url) {
    if (!url || 0 === url.length)
        return;

    return window.open(url.toString());
}

function isEmptyString(str) {
    return (str == "" || str == null);
}

function onRun(sender, args) {
    if (args.get_operationStatus() == lt.Annotations.Core.AnnDesignerOperationStatus.start) {
        var hyperlink = args.get_object().get_hyperlink();

        var id = args.get_object().get_id();
        if (id == lt.Annotations.Core.AnnObject.mediaObjectId) {
            var videoDiv = document.getElementById("videoDiv");
            if (_video != null) {
                videoDiv.removeChild(_video);
            }
            _video = document.createElement('video');
            _video.controls = "controls";
            _video.style.display = "block";
            _video.style.marginLeft = "auto";
            _video.style.marginRight = "auto";
            _video.style.width = "100%";

            videoDiv.appendChild(_video);

            var uri1 = args.get_object().get_media().get_source1();
            var uri2 = args.get_object().get_media().get_source2();
            var uri3 = args.get_object().get_media().get_source3();

            var noURI = String.isNullOrEmpty(uri1) && String.isNullOrEmpty(uri2) && String.isNullOrEmpty(uri3);

            if (typeof (_video.play) != "function" && !noURI)//safari cannot play if QuickTime Player not installed
            {
                _video.innerHTML = "Your browser does not support HTML5 video.";

                var element = document.createElement("a");

                element.setAttribute("id", "openMediaDialog");
                element.setAttribute("href", "#mediaPlayerPage");
                element.setAttribute("data-rel", "dialog");
                element.setAttribute("data-transition", "flip");

                var content = document.getElementById("Content");

                content.appendChild(element);
                $("#openMediaDialog").click();
            } else if (!noURI) {
                if (!String.isNullOrEmpty(uri1)) {
                    var source = document.createElement("source");
                    source.src = uri1;
                    _video.appendChild(source);
                }

                if (!String.isNullOrEmpty(uri2)) {
                    var source = document.createElement("source");
                    source.src = uri2;
                    _video.appendChild(source);
                }

                if (!String.isNullOrEmpty(uri3)) {
                    var source = document.createElement("source");
                    source.src = uri3;
                    _video.appendChild(source);
                }

                _video.load();

                var element = document.createElement("a");

                element.setAttribute("id", "openMediaDialog");
                element.setAttribute("href", "#mediaPlayerPage");
                element.setAttribute("data-rel", "dialog");
                element.setAttribute("data-transition", "flip");

                var content = document.getElementById("Content");

                content.appendChild(element);
                $("#openMediaDialog").click();

                $("#mediaPlayerPage").live('pageshow', function () {
                    $("#mediaPlayerPage").die('pageshow');
                    _imageViewer.get_interactiveService().stopListening();
                    setTimeout(function () {
                        _video.play();
                        _video.focus();
                    }, 1000); //to fix bug #20321 on opera
                });

                $("#mediaPlayerPage").live('pagehide', function () {
                    $("#mediaPlayerPage").die('pagehide');
                    _video.pause();
                    try {
                        _video.currentTime = 0;
                    } catch (ex) { }
                    _imageViewer.get_interactiveService().startListening();
                });
            }
        }
        else if (id == lt.Annotations.Core.AnnObject.audioObjectId) {
            if (_audio != null && !(_audio.paused || _audio.ended)) {
                _audio.pause();
            } else {
                _audio = document.createElement("audio");
                if (!(_audio.play) ? true : false) {
                    _audio = null;
                    //safari cannot play if QuickTime Player not installed
                    alert("Your browser does not support HTML5 audio.");
                }

                if (_audio != null) {
                    while (_audio.firstChild) {
                        _audio.removeChild(_audio.firstChild);
                    }

                    var uri = args.get_object().get_media().get_source1();
                    if (!String.isNullOrEmpty(uri)) {
                        var source = document.createElement("source");
                        source.src = uri;
                        _audio.appendChild(source);
                    }

                    var uri = args.get_object().get_media().get_source2();
                    if (!String.isNullOrEmpty(uri)) {
                        var source = document.createElement("source");
                        source.src = uri;
                        _audio.appendChild(source);
                    }

                    var uri = args.get_object().get_media().get_source3();
                    if (!String.isNullOrEmpty(uri)) {
                        var source = document.createElement("source");
                        source.src = uri;
                        _audio.appendChild(source);
                    }
                    _audio.play();
                }
            }
        }

        else if (!isEmptyString(hyperlink)) {
            var strings = hyperlink.split("//");
            if (strings != null && strings.length < 2) {
                hyperlink = "http://" + hyperlink;
            }

            window.setTimeout(function () {
                var oWin = openWindow(hyperlink);
                if (oWin == null || typeof oWin == 'undefined') {
                    alert('Your Popup Blocker has blocked opening this hyperlink. Disable the Popup Blocker for this web site and try again.');
                }
            }, 200);
        }
    }
}

function createAutomation() {
    var automation = new lt.Annotations.Automation.AnnAutomation(_automationManager, imageViewerAutomationControl);
    automation.add_editText(automation_EditText);
    automation.add_lockObject(automation_LockObject);
    automation.add_run(onRun);
    automation.add_unlockObject(automation_UnLockObject);
    automation.add_currentDesignerChanged(automation_CurrentDesignerChanged);
    automation.add_selectedObjectsChanged(automation_SelectedObjectsChanged);

    with (lt.Annotations.Core) {
        var resources = new lt.Annotations.Core.AnnResources();
        automation.get_container().set_resources(resources);
        var rubberStampsResources = resources.get_rubberStamps();
        var imagesResources = resources.get_images();
        var host = String.format('{0}//{1}', window.location.protocol, window.location.host);

        rubberStampsResources[AnnRubberStampType.stampApproved] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/approved.png");
        rubberStampsResources[AnnRubberStampType.stampAssigned] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Assigned.png");
        rubberStampsResources[AnnRubberStampType.stampClient] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Client.png");
        rubberStampsResources[AnnRubberStampType.stampChecked] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/checked.png");
        rubberStampsResources[AnnRubberStampType.stampCopy] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Copy.png");
        rubberStampsResources[AnnRubberStampType.stampDraft] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Draft.png");
        rubberStampsResources[AnnRubberStampType.stampExtended] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Extended.png");
        rubberStampsResources[AnnRubberStampType.stampFax] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Fax.png");
        rubberStampsResources[AnnRubberStampType.stampFaxed] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Faxed.png");
        rubberStampsResources[AnnRubberStampType.stampImportant] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Important.png");
        rubberStampsResources[AnnRubberStampType.stampInvoice] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Invoice.png");
        rubberStampsResources[AnnRubberStampType.stampNotice] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Notice.png");
        rubberStampsResources[AnnRubberStampType.stampPaid] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Paid.png");

        rubberStampsResources[AnnRubberStampType.stampOfficial] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Official.png");
        rubberStampsResources[AnnRubberStampType.stampOnFile] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Onfile.png");
        rubberStampsResources[AnnRubberStampType.stampPassed] = new AnnPicture(host + "host + /LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Passed.png");
        rubberStampsResources[AnnRubberStampType.stampPending] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Pending.png");
        rubberStampsResources[AnnRubberStampType.stampProcessed] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Processed.png");
        rubberStampsResources[AnnRubberStampType.stampReceived] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Received.png");
        rubberStampsResources[AnnRubberStampType.stampRejected] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/rejected.png");
        rubberStampsResources[AnnRubberStampType.stampRelease] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Release.png");
        rubberStampsResources[AnnRubberStampType.stampSent] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Sent.png");
        rubberStampsResources[AnnRubberStampType.stampShipped] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/Shipped.png");
        rubberStampsResources[AnnRubberStampType.stampTopSecret] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/topsecret.png");
        rubberStampsResources[AnnRubberStampType.stampUrgent] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/urgent.png");
        rubberStampsResources[AnnRubberStampType.stampVoid] = new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/RubberStamps/void.png");

        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/Point.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/lock.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/hotspot.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/audio.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/video.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/EncryptPrimary.png"));
        imagesResources.add(new AnnPicture(host + "/LeadtoolsHTML5Demos/Resources/Objects/EncryptSecondary.png"));
    }

    return automation;
}

function encode_utf8(s) { return unescape(encodeURIComponent(s)); }

function decode_utf8(s) { return decodeURIComponent(escape(s)); }

function onObjectPropertiesChanged(value, userData) {
    userData.set_value(value);
    updateToolbar();
    //update PolyRulers tick marks stroke thickness to same as object stroke thickness
    var annObject = _activeAutomation.get_currentEditObject();
    if (Type.canCast(annObject, lt.Annotations.Core.AnnPolyRulerObject) && userData.get_category() == "Stroke") {
        annObject.get_tickMarksStroke().get_strokeThickness().set_value(value);
    }
}

// Toolbar stuff
function canEditObject(annObject) {
    if (_activeAutomation == null) {
        return false;
    }
    if (annObject == null) {
        return false;
    }
    if (annObject.get_isLocked()) {
        return false;
    }
    if ((annObject.get_id() === lt.Annotations.Core.AnnObject.groupObjectId) ||
       (annObject.get_id() === lt.Annotations.Core.AnnObject.selectObjectId)) {
        return false;
    }
    return _activeAutomation.get_canShowProperties();
}

function annNoFill() {
    var editObject = _activeAutomation.get_currentEditObject();
    if (!canEditObject(editObject)) {
        return;
    }
    if (editObject.get_supportsFill()) {
        editObject.set_fill(lt.Annotations.Core.AnnSolidColorBrush.create('Transparent'));
        _activeAutomation.get_automationControl().automationInvalidate();
    }
}

function annSolidFill() {
    var editObject = _activeAutomation.get_currentEditObject();
    if (!canEditObject(editObject)) {
        return;
    }
    if (editObject.get_supportsFill()) {
        editObject.set_fill(lt.Annotations.Core.AnnSolidColorBrush.create('Blue'));
        _activeAutomation.get_automationControl().automationInvalidate();
    }
}

function annNoStroke() {
    var editObject = _activeAutomation.get_currentEditObject();
    if (!canEditObject(editObject)) {
        return;
    }
    if (editObject.get_supportsStroke()) {
        editObject.set_stroke(lt.Annotations.Core.AnnStroke.create(lt.Annotations.Core.AnnSolidColorBrush.create('Transparent'), lt.LeadLengthD.create(1)));
        _activeAutomation.get_automationControl().automationInvalidate();
    }
}

function annSolidStroke() {
    var editObject = _activeAutomation.get_currentEditObject();
    if (!canEditObject(editObject)) {
        return;
    }
    if (editObject.get_supportsStroke()) {
        editObject.set_stroke(lt.Annotations.Core.AnnStroke.create(lt.Annotations.Core.AnnSolidColorBrush.create('Red'), lt.LeadLengthD.create(1)));
        _activeAutomation.get_automationControl().automationInvalidate();
    }
}

function annEditObjectProperties() {
    var editObject = _activeAutomation.get_currentEditObject();
    if (!canEditObject(editObject)) {
        return;
    }
    showProperties();
}

function annLockObject() {
    if (_activeAutomation.get_canLock()) {
        var element = document.createElement("a");
        var editObject = _activeAutomation.get_currentEditObject();
        element.setAttribute("id", "lockDialog");
        element.setAttribute("href", "#passwordPage");
        element.setAttribute("data-rel", "dialog");
        element.setAttribute("data-transition", "flip");
        var content = document.getElementById("Content");
        content.appendChild(element);
        //e.set_cancel(true);
        $("#passwordPage").data("annObject", editObject);
        $("#passwordPage").data("Lock", true);
        $("#password").val(null);
        _propertiesVisible = true;
        $("#lockDialog").click();
    }
}

function annUnlockObject() {
    if (_activeAutomation.get_canUnlock()) {
        var element = document.createElement("a");
        var editObject = _activeAutomation.get_currentEditObject();
        element.setAttribute("id", "unlockDialog");
        element.setAttribute("href", "#passwordPage");
        element.setAttribute("data-rel", "dialog");
        element.setAttribute("data-transition", "flip");
        var content = document.getElementById("Content");
        content.appendChild(element);
        //e.set_cancel(true);
        $("#passwordPage").data("annObject", editObject);
        $("#passwordPage").data("Lock", false);
        $("#password").val(null);
        _propertiesVisible = true;
        $("#unlockDialog").click();
    }
}

function annUndo() {
    if (_activeAutomation != null && _activeAutomation.get_canUndo() && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        _activeAutomation.undo();
        updateToolbar();
    }
}

function annRedo() {
    if (_activeAutomation != null && _activeAutomation.get_canRedo() && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        _activeAutomation.redo();
        updateToolbar();
    }
}

function annBurn() {
    burnAnnotations()
}

function annSave() {
    if (_activeAutomation != null && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        var codecs = new lt.Annotations.Core.AnnCodecs();
        var xmlString = codecs.save(_activeAutomation.get_container(), 1, null, 1);
        return xmlString; // Line added by Yogeesha
        // postData(xmlString, 'xml');  // Line commented by Yogeesha
    }
}

function annSaveSVG() {
    if (_activeAutomation != null && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        var svg = lt.Annotations.Rendering.AnnSvgRenderingEngine.exportSvg(_activeAutomation.get_container(), null, _svgRenderingEngine.get_renderers());
        postData(svg, 'svg');
    }
}



function annLoad() {
    if (_activeAutomation != null && _activeAutomation.get_manager().get_userMode() === lt.Annotations.Core.AnnUserMode.design) {
        $('#inputFileBrowser').remove();
        $("#fileInput").append('<input type="file" id="inputFileBrowser" accept="text/xml" value="Load" change="onInputFileChange()" style="visibility:collapse"/>');

        $("#inputFileBrowser").bind('change', onInputFileChange);
        if (lt.LTHelper.OS == lt.LTOS.android && lt.LTHelper.browser != lt.LTBrowser.opera) {
            $("#inputFileBrowser").click()
        }
        else {
            window.setTimeout(function () {
                $("#inputFileBrowser").click()
            }, 200);
        }
    }
}

function annRunMode() {
    if (_activeAutomation != null) {
        _activeAutomation.get_manager().set_userMode(lt.Annotations.Core.AnnUserMode.run);
        updateToolbar();
    }
}

function annDesignMode() {
    if (_activeAutomation != null) {
        _activeAutomation.get_manager().set_userMode(lt.Annotations.Core.AnnUserMode.design);
        updateToolbar();
        if (_audio != null) {
            _audio.pause();
        }
        if (_video != null) {
            _video.pause();
        }
    }
}

function canDeleteObject() {
    var canDelete = _activeAutomation != null && _activeAutomation.get_canDeleteObjects();
    if (canDelete) {
        // Check if it is the text box in edit mode, then we should not delete it
        var textElement = document.getElementById("textObject");
        if (textElement != null) {
            canDelete = false;
        }
    }

    return canDelete;
}

function clickEffect(item) {
    if (_isTouchDevice && $(item).hasClass(item.id)) {
        $(item).removeClass(item.id);
        $(item).addClass("Click" + item.id);
        setTimeout(function () {
            $(item).removeClass("Click" + item.id);
            $(item).addClass(item.id);
        }, 200);
    }
}

function annDelete() {
    if (canDeleteObject()) {
        _activeAutomation.deleteSelectedObjects();
    }
}

function updateToolbar() {
    var toolbar = document.getElementById('toolRibbon1');
    if (toolbar.style.visibility === "hidden") {
        return;
    }

    var userMode = 0;
    if (_activeAutomation != null) {
        userMode = _activeAutomation.get_manager().get_userMode();
    }

    //    if (_activeAutomation != null && userMode === lt.Annotations.Core.AnnUserMode.run) {
    //        document.getElementById('DesignMode').setAttribute('class', 'toolBarItem DesignMode');
    //        document.getElementById('RunMode').setAttribute('class', 'toolBarItem DisableRunMode');
    //    } else {
    //        document.getElementById('DesignMode').setAttribute('class', 'toolBarItem DisableDesignMode');
    //        document.getElementById('RunMode').setAttribute('class', 'toolBarItem RunMode');
    //    }

    //    if (_activeAutomation != null && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('SaveAnn').setAttribute('class', 'toolBarItem SaveAnn');
    //        document.getElementById('LoadAnn').setAttribute('class', 'toolBarItem LoadAnn');
    //        document.getElementById('Burn').setAttribute('class', 'toolBarItem Burn');
    //        //for version 19 release
    //        //document.getElementById('SaveAnnSVG').setAttribute('class', 'toolBarItem SaveAnnSVG');

    //    } else {
    //        document.getElementById('SaveAnn').setAttribute('class', 'toolBarItem DisableSaveAnn');
    //        document.getElementById('LoadAnn').setAttribute('class', 'toolBarItem DisableLoadAnn');
    //        document.getElementById('Burn').setAttribute('class', 'toolBarItem DisableBurn');
    //        //for version 19 release
    //        //document.getElementById('SaveAnnSVG').setAttribute('class', 'toolBarItem DisableSaveAnnSVG');
    //    }

    //    document.getElementById('Undo').setAttribute('class', 'toolBarItem DisableUndo');
    //    document.getElementById('Redo').setAttribute('class', 'toolBarItem DisableRedo');
    //    if (_activeAutomation != null && _activeAutomation.get_canUndo() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('Undo').setAttribute('class', 'toolBarItem Undo');
    //    }
    //    if (_activeAutomation != null && _activeAutomation.get_canRedo() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('Redo').setAttribute('class', 'toolBarItem Redo');
    //    }

    //    document.getElementById('Lock').setAttribute('class', 'toolBarItem DisableLock');
    //    document.getElementById('Unlock').setAttribute('class', 'toolBarItem DisableUnlock');
    //    if (_activeAutomation != null && _activeAutomation.get_canLock() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('Lock').setAttribute('class', 'toolBarItem Lock');
    //    }
    //    if (_activeAutomation != null && _activeAutomation.get_canUnlock() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('Unlock').setAttribute('class', 'toolBarItem Unlock');
    //    }

    //    document.getElementById('ApplyEncryptor').setAttribute('class', 'toolBarItem DisableApplyEncryptor');
    //    document.getElementById('ApplyDecryptor').setAttribute('class', 'toolBarItem DisableApplyDecryptor');
    //    if (_activeAutomation != null && _activeAutomation.get_canApplyEncryptor() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('ApplyEncryptor').setAttribute('class', 'toolBarItem ApplyEncryptor');
    //    }
    //    if (_activeAutomation != null && _activeAutomation.get_canApplyDecryptor() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('ApplyDecryptor').setAttribute('class', 'toolBarItem ApplyDecryptor');
    //    }

    //    document.getElementById('RedactionRealize').setAttribute('class', 'toolBarItem DisableRedactionRealize');
    //    document.getElementById('RedactionRestore').setAttribute('class', 'toolBarItem DisableRedactionRestore');
    //    if (_activeAutomation != null && _activeAutomation.get_canRealizeRedaction() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('RedactionRealize').setAttribute('class', 'toolBarItem RedactionRealize');
    //    }
    //    if (_activeAutomation != null && _activeAutomation.get_canRestoreRedaction() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('RedactionRestore').setAttribute('class', 'toolBarItem RedactionRestore');
    //    }

    //    if (canDeleteObject() && userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('Delete').setAttribute('class', 'toolBarItem Delete');
    //    }
    //    else {
    //        document.getElementById('Delete').setAttribute('class', 'toolBarItem DisableDelete');
    //    }

    //    if (userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('SaveImage').setAttribute('class', 'toolBarItem SaveImage');
    //    }
    //    else {
    //        document.getElementById('SaveImage').setAttribute('class', 'toolBarItem DisableSaveImage');
    //    }

    //    if (userMode === lt.Annotations.Core.AnnUserMode.design) {
    //        document.getElementById('LoadImage').setAttribute('class', 'toolBarItem LoadImage');
    //    }
    //    else {
    //        document.getElementById('LoadImage').setAttribute('class', 'toolBarItem DisableLoadImage');
    //    }

    //    //   document.getElementById('nofill').src="images/nofill_.png";
    //    //   document.getElementById('solidfill').src="images/solidfill_.png";
    //    //   document.getElementById('nostroke').src="images/nostroke_.png";
    //    //   document.getElementById('solidstroke').src="images/solidstroke_.png";
    //    document.getElementById('Properties').setAttribute('class', 'toolBarItem DisableProperties');
    //    if (_activeAutomation != null) {
    //        var editObject = _activeAutomation.get_currentEditObject();
    //        if (editObject != null) {
    //            if (editObject.get_id() !== lt.Annotations.Core.AnnObject.groupObjectId && editObject.get_id() !== lt.Annotations.Core.AnnObject.selectObjectId) {
    //                if (_activeAutomation.get_canShowProperties()) {
    //                    document.getElementById('Properties').setAttribute('class', 'toolBarItem Properties');
    //                    //               if(editObject.get_supportsFill()) {
    //                    //                  document.getElementById('nofill').src="images/nofill.png";
    //                    //                  document.getElementById('solidfill').src="images/solidfill.png";
    //                    //               }
    //                    //               if(editObject.get_supportsStroke()) {
    //                    //                  document.getElementById('nostroke').src="images/nostroke.png";
    //                    //                  document.getElementById('solidstroke').src="images/solidstroke.png";
    //                    //               }
    //                }
    //            }
    //        }
    //    }
}

function automation_SelectedObjectsChanged() {
    updateToolbar();
}
