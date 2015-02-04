/// <reference path="HelloWorldtenant.createwizard.js" />
/// <reference path="HelloWorldtenant.controller.js" />
/*globals window,jQuery,HelloWorldTenantExtension,Exp,waz,cdm*/
(function ($, global, undefined) {
    "use strict";

    var grid,
        selectedRow,
        statusIcons = {
            Registered: {
                text: "Registered",
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        };

    function dateFormatter(value) {
        try {
            if (value) {
                return $.datepicker.formatDate("m/d/yy", value);
            }
        }
        catch (err) { }  // Display "-" if the date is in an unrecoginzed format.

        return "-";
    }

    function onRowSelected(row) {
        if (row) {
            selectedRow = row;
            updateContextualCommands(row);
        }
    }

    function updateContextualCommands(domain) {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("showDnsManager", "Connect", Exp.UI.CommandIconDescriptor.getWellKnown("browse"), true, null, onShowDnsManager));
        Exp.UI.Commands.Contextual.add(new Exp.UI.Command("viewDomainInfo", "Info", Exp.UI.CommandIconDescriptor.getWellKnown("viewinfo"), true, null, onViewInfo));
        Exp.UI.Commands.update();
    }

    // Command handlers
    function onShowDnsManager() {
        var userInfo = global.DomainTenantExtension.Controller.getCurrentUserInfo(),
            portalUrl;

        if (!userInfo.GoDaddyShopperPasswordChanged) {
            changePassword(userInfo);
        } else {
            portalUrl = global.DomainTenantExtension.Controller.getCurrentUserInfo().GoDaddyCustomerPortalUrl;
            window.open(portalUrl, "_blank");
        }
    }

    function onViewInfo(item) {
        cdm.stepWizard({
            extension: "DomainTenantExtension",
            steps: [
                {
                    template: "viewInfo",
                    contactInfo: global.DomainTenantExtension.Controller.getCurrentUserInfo(),
                    domain: selectedRow
                }
            ]
        },
        { size: "mediumplus" });
    }

    function changePassword(currentUserInfo) {
        var promise,
            wizardContainerSelector = ".dm-selectPassword";

        cdm.stepWizard({
            extension: "DomainTenantExtension",
            steps: [
                {
                    template: "selectPassword",
                    data: {
                        customerId: currentUserInfo.GoDaddyShopperId
                    },
                    onStepActivate: function () {
                        Shell.UI.Validation.setValidationContainer(wizardContainerSelector);
                    }
                }
            ],

            onComplete: function () {
                if (!Shell.UI.Validation.validateContainer(wizardContainerSelector)) {
                    return false;
                }

                currentUserInfo.GoDaddyShopperPassword = $("#dm-password").val();
                currentUserInfo.GoDaddyShopperPasswordChanged = true;
                promise = global.DomainTenantExtension.Controller.updateUserInfo(currentUserInfo);

                global.waz.interaction.showProgress(
                    promise,
                    {
                        initialText: "Reseting password...",
                        successText: "Successfully reset the password.",
                        failureText: "Failed to reset the password."
                    }
                );

                promise.done(function () {
                    global.DomainTenantExtension.Controller.invalidateUserInfoCache();
                    var portalUrl = global.DomainTenantExtension.Controller.getCurrentUserInfo().GoDaddyCustomerPortalUrl;
                    window.open(portalUrl, "_blank");
                });
            }
        },
        { size: "small" });
    }

    function openQuickCreate() {
        Exp.Drawer.openMenu("AccountsAdminMenuItem/CreateFileShare");
    }

    // Public
    function loadTab(extension, renderArea, initData) {
        var subs = Exp.Rdfe.getSubscriptionList(),
           subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("helloworld"),
        localDataSet = {            
            dataSetName: global.HelloWorldTenantExtension.Controller.listFileSharesUrl,
            ajaxData: { 
                subscriptionIds: subscriptionRegisteredToService[0].id
                },
            url: global.HelloWorldTenantExtension.Controller.listFileSharesUrl
        },
        columns = [
                { name: "Share Name", field: "Name", sortable: false },
                { name: "Server Name", field: "FileServerName", filterable: false, sortable: false },
                { name: "subscriptionId", field: "SubscriptionId", filterable: false, sortable: false },
                { name: "Size", field: "Size", filterable: false, sortable: false }
            ];

        grid = renderArea.find(".gridContainer")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: localDataSet,
                keyField: "name",
                columns: columns,
                gridOptions: {
                    rowSelect: onRowSelected
                },
                emptyListOptions: {
                    extensionName: "HelloWorldTenantExtension",
                    templateName: "FileSharesTabEmpty"
                }
            });
    }
    
    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.HelloWorldTenantExtension = global.HelloWorldTenantExtension || {};
    global.HelloWorldTenantExtension.FileSharesTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        statusIcons: statusIcons
    };
})(jQuery, this);
