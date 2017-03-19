(function () {
    "use strict";
    angular.module(APPNAME)
           .factory("$referralService", referralServiceFactory);
		   
	// manually identify dependencies for injection: https://github.com/johnpapa/angular-styleguide#style-y091
    referralServiceFactory.$inject = ["$baseService"];

    function referralServiceFactory($baseService) {
		
        var abringproServiceObject = bringpro.services.referrals;
		
		//  merge the jQuery object with the angular base service to simulate inheritance
        var newService = $baseService.merge(true, {}, abringproServiceObject, $baseService);
        
		return newService;
    }
})();