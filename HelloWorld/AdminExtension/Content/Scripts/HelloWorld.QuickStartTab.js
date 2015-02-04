/// <reference path="HelloWorldadmin.controller.js" />
(function ($, global, fx, Exp, undefined) {
    "use strict";

    var holder,
        htmlResources = {
            quickStartImage: fx.resources.getContentUrl("Content/HelloWorldAdmin/Images/quickstart.png"),
            registerResourceProviderInstruction: "Connect the portal to your Hello World resource provider.",
            registerResellerAccountInstruction: "This is used to talk to HelloWorld API.",
        },
        steps;

    // Input Dialogs
    function registerResourceProvider() {
        var htmlResources = {
            registerEndpointTitle: "Register your Hello World REST endpoint",
            registerEndpointSubTitle: "Specify the endpoint URL to the Hello World REST API that the Portal will connect to.",
            endpointName: "REST API endpoint"
        };

        registerEndpoint(htmlResources, false, function (newEndpointUrl, newUsername, newPassword) {
            var newSettings = $.extend(true, {}, global.HelloWorldAdminExtension.Controller.getCurrentAdminSettings());
            newSettings.EndpointAddress = newEndpointUrl;
            newSettings.Username = newUsername;
            newSettings.Password = newPassword;

            return global.HelloWorldAdminExtension.Controller.updateAdminSettings(newSettings);
        });
    }

    function registerEndpoint(htmlResources, registerReseller, callback) {
        var promise,
            wizardContainerSelector = ".hw-registerEndpoint";

        cdm.stepWizard({
            extension: "HelloWorldAdminExtension",
            steps: [
                {
                    template: "registerEndpoint",
                    htmlResources: htmlResources,
                    data: { registerReseller: registerReseller },
                    onStepActivate: function () {
                        Shell.UI.Validation.setValidationContainer(wizardContainerSelector);
                    }
                }
            ],

            onComplete: function () {
                var newEndpointUrl, newUserName, newPassword, newResellerPortalUrl;

                if (!Shell.UI.Validation.validateContainer(wizardContainerSelector)) {
                    return false;
                }

                newEndpointUrl = $("#hw-endpointUrl").val();
                newUserName = $("#hw-username").val();
                newPassword = $("#hw-password").val();
                newResellerPortalUrl = registerReseller ? $("#dm-portalUrl").val() : null;

                promise = callback(newEndpointUrl, newUserName, newPassword, newResellerPortalUrl);

                global.waz.interaction.showProgress(
                    promise,
                    {
                        initialText: "Registering endpoint...",
                        successText: "Successfully registered the endpoint.",
                        failureText: "Failed to register the endpoint."
                    }
                );

                promise.done(function () {
                    global.HelloWorldAdminExtension.Controller.invalidateAdminSettingsCache();
                    global.HelloWorldAdminExtension.Controller.getCurrentAdminSettings().done(function (data) {
                        updateSteps(data.data);
                    });
                });
            }
        });
    }

    steps = {
        resourceProvider: {
            state: false,
            text: htmlResources.registerResourceProviderInstruction,
            cssClass: ".quickstart-step1",
            handler: registerResourceProvider
        }
    };

    function updateSteps(adminSettings) {
        var step, i;

        steps.resourceProvider.state = !!adminSettings.EndpointAddress;

        for (i in steps) {
            if (steps.hasOwnProperty(i)) {
                step = steps[i];

                if (step.state) {
                    holder.find(step.cssClass).addClass("completed");
                    removeAction(
                        holder.find(step.cssClass + " .detail .detailDescription"),
                        step.text
                    );
                } else {
                    holder.find(step.cssClass).removeClass("completed");
                    addAction(
                        holder.find(step.cssClass + " .detail .detailDescription"),
                        step.text,
                        step.handler
                    );
                }
            }
        }
    }

    function addAction(holder, name, callback) {
        holder.quickstartItems(
            {
                actions: [
                    {
                        name: name,
                        callback: function (event) {
                            event.preventDefault();
                            callback();
                            return false;
                        }
                    }
                ]
            });
    }

    function removeAction(holder, name) {
        holder.quickstartItems("destroy").quickstartItems(
            {
                actions: [{
                    name: name,
                    disabled: true
                }]
            })
            .find("a").removeAttr("href").on("click", function (e) {
                e.preventDefault();
                return false;
            });
    }

    function getSkipQuickStart() {
        var skipQuickStart = global.HelloWorldAdminExtension.settings.skipQuickStart;

        if (skipQuickStart === null || skipQuickStart === undefined) {
            setSkipQuickStart(true);
            return true;
        }

        return skipQuickStart;
    }

    function setSkipQuickStart(value) {
        global.HelloWorldAdminExtension.settings.skipQuickStart = value;
        Exp.UserSettings.updateGlobalUserSetting("HelloWorld-skipQuickStart", JSON.stringify(value));
    }

    // Public
    function loadTab(renderData, container) {
        holder = container.find(".adminQuickStart")
                .html(global.HelloWorldAdminExtension.templates.quickStartTabContent.render(htmlResources));

        // Intialize the local data update event handler
        global.HelloWorldAdminExtension.Controller.invalidateAdminSettingsCache()
            .done(function (data) {
                $(data.data).off("propertyChange").on("propertyChange", function () {
                    updateSteps(data.data);
                });
                $(data.data).trigger("propertyChange");
            });

        Exp.Data.setFastPolling(global.HelloWorldAdminExtension.Controller.adminSettingsUrl, true);

        $(".quickstartCheckbox").fxCheckBox({
            value: getSkipQuickStart(),
            change: function (event, data) {
                setSkipQuickStart(data.value);
            }
        });

        global.HelloWorldAdminExtension.Controller.getCurrentAdminSettings()
        .done(function (data) {
            updateSteps(data.data);
        });
    }

    function cleanup() {
    }

    function executeCommand(commandId) {
        return false;
    }

    global.HelloWorldAdminExtension = global.HelloWorldAdminExtension || {};
    global.HelloWorldAdminExtension.QuickStartTab = {
        loadTab: loadTab,
        cleanup: cleanup,
        executeCommand: executeCommand
    };
})(jQuery, this, this.fx, this.Exp);