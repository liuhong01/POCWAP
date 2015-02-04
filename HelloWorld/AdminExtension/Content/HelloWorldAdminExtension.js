/*globals window,jQuery,Shell,Exp,waz*/

(function (global, $, undefined) {
    "use strict";

    var resources = [],
        helloWorldExtensionActivationInit,
        navigation;   

    function clearCommandBar() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    function onApplicationStart() {
        Exp.UserSettings.getGlobalUserSetting("Admin-skipQuickStart").then(function (results) {
            var setting = results ? results[0] : null;
            if (setting && setting.Value) {
                global.HelloWorldAdminExtension.settings.skipQuickStart = JSON.parse(setting.Value);
            }
        });
                
        global.HelloWorldAdminExtension.settings.skipQuickStart = false;
    }

    function loadQuickStart(extension, renderArea, renderData) {
        clearCommandBar();
        global.HelloWorldAdminExtension.QuickStartTab.loadTab(renderData, renderArea);
    }

    function loadFileServersTab(extension, renderArea, renderData) {
        global.HelloWorldAdminExtension.FileServersTab.loadTab(renderData, renderArea);
    }

    function loadProductsTab(extension, renderArea, renderData) {
        global.HelloWorldAdminExtension.ProductsTab.loadTab(renderData, renderArea);
    }

    function loadSettingsTab(extension, renderArea, renderData) {
        global.HelloWorldAdminExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    function loadControlsTab(extension, renderArea, renderData) {
        global.HelloWorldAdminExtension.ControlsTab.loadTab(renderData, renderArea);
    }

    global.helloWorldExtension = global.HelloWorldAdminExtension || {};

    navigation = {
        tabs: [
                {
                    id: "quickStart",
                    displayName: "quickStart",
                    template: "quickStartTab",
                    activated: loadQuickStart
                },
                {
                    id: "fileServers",
                    displayName: "file Servers",
                    template: "fileServersTab",
                    activated: loadFileServersTab
                },
                 {
                     id: "products",
                     displayName: "products",
                     template: "productsTab",
                     activated: loadProductsTab
                 },
                {
                    id: "settings",
                    displayName: "settings",
                    template: "settingsTab",
                    activated: loadSettingsTab
                },
                {
                    id: "controls",
                    displayName: "controls",
                    template: "controlsTab",
                    activated: loadControlsTab
                }
        ],
        types: [
        ]
    };

    helloWorldExtensionActivationInit = function () {
        var helloWorldExtension = $.extend(this, global.HelloWorldAdminExtension);

        $.extend(helloWorldExtension, {
            displayName: "Hello World",
            viewModelUris: [
                global.HelloWorldAdminExtension.Controller.adminSettingsUrl,
                global.HelloWorldAdminExtension.Controller.adminProductsUrl,
            ],
            menuItems: [],
            settings: {
                skipQuickStart: true
            },
            getResources: function () {
                return resources;
            }
        });

        helloWorldExtension.onApplicationStart = onApplicationStart;        
        helloWorldExtension.setCommands = clearCommandBar();

        Shell.UI.Pivots.registerExtension(helloWorldExtension, function () {
            Exp.Navigation.initializePivots(this, navigation);
        });

        // Finally activate helloWorldExtension 
        $.extend(global.HelloWorldAdminExtension, Shell.Extensions.activate(helloWorldExtension));
    };

    Shell.Namespace.define("HelloWorldAdminExtension", {
        init: helloWorldExtensionActivationInit
    });

})(this, jQuery, Shell, Exp);