/// <reference path="scripts/helloWorldTenant.createwizard.js" />
/// <reference path="scripts/helloWorldTenant.controller.js" />
/*globals window,jQuery,Shell, HelloWorldTenantExtension, Exp*/

(function ($, global, undefined) {
    "use strict";

    var resources = [],
        HelloWorldTenantExtensionActivationInit,
        navigation,
        serviceName = "helloWorld";

    function onNavigateAway() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    function loadSettingsTab(extension, renderArea, renderData) {
        global.HelloWorldTenantExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    function fileSharesTab(extension, renderArea, renderData) {
        global.HelloWorldTenantExtension.FileSharesTab.loadTab(renderData, renderArea);
    }

    global.HelloWorldTenantExtension = global.HelloWorldTenantExtension || {};

    navigation = {
        tabs: [
            {
                id: "fileShares",
                displayName: "File Shares",
                template: "fileSharesTab",
                activated: fileSharesTab
            }            
        ],
        types: [
        ]
    };

    HelloWorldTenantExtensionActivationInit = function () {
        var subs = Exp.Rdfe.getSubscriptionList(),
            subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("helloworld"),
            helloWorldExtension = $.extend(this, global.HelloWorldTenantExtension);

        // Don't activate the extension if user doesn't have a plan that includes the service.
        if (subscriptionRegisteredToService.length === 0) {
            return false; // Don't want to activate? Just bail
        }

        $.extend(helloWorldExtension, {
            viewModelUris: [helloWorldExtension.Controller.userInfoUrl],
            displayName: "Hello World",
            navigationalViewModelUri: {
                uri: helloWorldExtension.Controller.listFileSharesUrl,
                ajaxData: function () {
                    return global.Exp.Rdfe.getSubscriptionIdsRegisteredToService(serviceName);
                }
            },
            displayStatus: global.waz.interaction.statusIconHelper(global.HelloWorldTenantExtension.FileSharesTab.statusIcons, "Status"),
            menuItems: [
                {
                    name: "FileShares",
                    displayName: "Hello World",
                    url: "#Workspaces/HelloWorldTenantExtension",
                    preview: "createPreview",
                    subMenu: [
                        {
                            name: "Create",
                            displayName: "Create File Share",
                            description: "Quickly Create File Share on a File Server",
                            template: "CreateFileShare",
                            label: "Create",
                            subMenu: [
                                {
                                    name: "QuickCreate",
                                    displayName: "FileFile",
                                    template: "CreateFileShare"                                    
                                }
                            ]
                        }
                    ]
                }
            ],
            getResources: function () {
                return resources;
            }
        });

        helloWorldExtension.onNavigateAway = onNavigateAway;
        helloWorldExtension.navigation = navigation;

        Shell.UI.Pivots.registerExtension(helloWorldExtension, function () {
            Exp.Navigation.initializePivots(this, this.navigation);
        });

        // Finally activate and give "the" helloWorldExtension the activated extension since a good bit of code depends on it
        $.extend(global.HelloWorldTenantExtension, Shell.Extensions.activate(helloWorldExtension));
    };

    function getQuickCreateFileShareMenuItem() {
        return {
            name: "QuickCreate",
            displayName: "Create File Share",
            description: "Create new file share",
            template: "quickCreateWithRdfe",
            label: resources.CreateMenuItem,

            opening: function () {
                AccountsAdminExtension.AccountsTab.renderListOfHostingOffers(offersListSelector);
            },

            open: function () {
                // Enables As-You-Type validation experience on a container specified
                Shell.UI.Validation.setValidationContainer(valContainerSelector);
                // Enables password complexity feedback experience on a container specified
                Shell.UI.PasswordComplexity.parse(valContainerSelector);
            },

            ok: function (object) {
                var dialogFields = object.fields,
                    isValid = validateAccount();

                if (isValid) {
                    createAccountWithRdfeCore(dialogFields);
                }
                return false;
            },

            cancel: function (dialogFields) {
                // you can return false to cancel the closing
            }
        };
    }

    Shell.Namespace.define("HelloWorldTenantExtension", {
        serviceName: serviceName,
        init: HelloWorldTenantExtensionActivationInit,
        getQuickCreateFileShareMenuItem: getQuickCreateFileShareMenuItem
    });
})(jQuery, this);
