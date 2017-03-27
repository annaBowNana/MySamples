(function () {
    "use strict";

    angular.module(APPNAME)
        .factory('$reportsService', reportsFactory);

    reportsFactory.$inject = ['$baseService'];

    function reportsFactory($baseService) {

        var aServiceObject = bringpro.services.reports;

        var newService = $baseService.merge(true, {}, aServiceObject, $baseService);

        return newService;
    }

})();