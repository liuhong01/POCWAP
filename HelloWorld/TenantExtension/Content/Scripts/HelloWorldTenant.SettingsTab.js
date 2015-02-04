/// <reference path="domaintenant.controller.js" />
/// <reference path="domaintenant.domainstab.js" />
/// <disable>JS2076.IdentifierIsMiscased</disable>
/*global,jQuery,trace,cdm, waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var commandsEnabled,
        tabContainer,
        passwordChanged = false;

    function renderPage(userInfo, container) {        
    }

    function onSettingChanged() {
        updateContextualCommands(true);
    }

    function updateContextualCommands(hasPendingChanges) {
        if (commandsEnabled !== hasPendingChanges) {
            Exp.UI.Commands.Contextual.clear();
            if (hasPendingChanges) {
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("saveSettings", "Save", Exp.UI.CommandIconDescriptor.getWellKnown("save"), true, null, onSaveSettings));
                Exp.UI.Commands.Contextual.add(new Exp.UI.Command("discardSettings", "Discard", Exp.UI.CommandIconDescriptor.getWellKnown("reset"), true, null, onDiscardSettings));
                Shell.UI.Navigation.setConfirmNavigateAway("If you leave this page then your unsaved changes will be lost.");
                commandsEnabled = true;
            } else {
                Shell.UI.Navigation.removeConfirmNavigateAway();
                commandsEnabled = false;
            }
            Exp.UI.Commands.update();
        }
    }

    function onSaveSettings() {        
    }

    function onDiscardSettings() {
    }

    // Public
    function loadTab(renderData, container) {
        commandsEnabled = false;

        Shell.UI.Validation.setValidationContainer("#hw-settings");  // Initialize validation container for subsequent calls to Shell.UI.Validation.validateContainer.
        $("#hw-settings").on("change.fxcontrol", onSettingChanged);

        $("#hw-password").on("keyup change", function () {
            passwordChanged = true;
        });
    }

    function cleanup() {
    }

    global.HelloWorldTenantExtension = global.HelloWorldTenantExtension || {};
    global.HelloWorldTenantExtension.SettingsTab = {
        loadTab: loadTab,
        cleanup: cleanup
    };
})(jQuery, this, this.Shell, this.Exp);