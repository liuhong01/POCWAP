/*globals window,jQuery,cdm,HelloWorldTenantExtension,waz,Exp*/
(function ($, global, undefined) {
    "use strict";

    var baseUrl = "/HelloWorldTenant",
        listFileSharesUrl = baseUrl + "/ListFileShares",
        domainType = "HelloWorld";

    function navigateToListView() {
        Shell.UI.Navigation.navigate("#Workspaces/{0}/helloworld".format(HelloWorldTenantExtension.name));
    }

    function getFileShares(subscriptionIds) {
        return makeAjaxCall(listFileSharesUrl, { subscriptionIds: subscriptionIds }).data;
    }

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }

    function getLocalPlanDataSet() {
        return Exp.Data.getLocalDataSet(planListUrl);
    }

    function getfileSharesData(subscriptionId) {
        return Exp.Data.getData("fileshare{0}".format(subscriptionId), {
            ajaxData: {
                subscriptionIds: subscriptionId
            },
            url: listFileSharesUrl,
            forceCacheRefresh: true
        });
    }

    // TODO: Can we use the waz.dataWrapper in the sample?
    function createFileShare(subscriptionId, fileShareName, size, fileServerName) {
        return new waz.dataWrapper(Exp.Data.getLocalDataSet(listFileSharesUrl))
            .add(
            {
                Name: fileShareName,
                SubscriptionId: subscriptionId,
                Size: size,
                FileServerName: fileServerName
            },
            Shell.Net.ajaxPost({
                data: {
                    subscriptionId: subscriptionId,
                    Name: fileShareName,
                    Size: size,
                    FileServerName: fileServerName
                },
                url: baseUrl + "/CreateFileShare"
            })
        );
    }

    global.HelloWorldTenantExtension = global.HelloWorldTenantExtension || {};
    global.HelloWorldTenantExtension.Controller = {
        createFileShare: createFileShare,
        listFileSharesUrl: listFileSharesUrl,
        getFileShares: getFileShares,
        getLocalPlanDataSet: getLocalPlanDataSet,
        getfileSharesData: getfileSharesData,
        navigateToListView: navigateToListView
    };
})(jQuery, this);
