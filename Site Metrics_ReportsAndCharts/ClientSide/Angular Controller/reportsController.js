
(function () {
    "use strict";
    angular.module(APPNAME)
        .controller('reportsController', ReportsController)
        .filter('utcToLocal', Filter);


    ReportsController.$inject = ['$scope', '$baseController', "$reportsService", '$uibModal', "$utilityService", "$filter", "$websiteService", "toastr"];

    function ReportsController(
       $scope
       , $baseController
       , $reportsService
       , $uibModal
       , $utilityService
       , $filter
       , $websiteService
       , toastr) {

        var vm = this;//this points to a new {}
        vm.filters = {
            queryWebsiteId: null
           , queryStartDate: null
           , queryEndDate: null
        };
        vm.popup1 = {
            opened: false
        }
        vm.popup2 = {
            opened: false
        }
        vm.customDates = {
            'visibility': 'hidden'
        }
        vm.url = null;
        vm.reportData = null;
        vm.websites = {};
        vm.dateRanges = ["Today", "Past Week", "Past Month", "Custom"]; //this is for range date filter

        vm.$reportsService = $reportsService;
        vm.$scope = $scope;
        vm.$uibModal = $uibModal;
        vm.$utilityService = $utilityService;
        vm.$filter = $filter;
        vm.$websiteService = $websiteService;

        vm.getSuccess = _getSuccess;
        vm.getError = _getError;
        vm.useFilter = _useFilter;
        vm.clearFilter = _clearFilter;
        vm.getByDateAndWebIdError = _getByDateAndWebIdError;
        vm.open1 = _open1;
        vm.open2 = _open2;
        vm.checkClick = _checkClick;

        //-- this line to simulate inheritance
        $baseController.merge(vm, $baseController);

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$reportsService.getNotifier($scope);

        render();

        function render() {

            var d = new Date();
            vm.startDate = new Date(d.setDate(d.getDate() - 7));
            vm.endDate = new Date();
            vm.filters.queryStartDate = _convertDateTime(vm.startDate);
            vm.filters.queryEndDate = _convertDateTime(vm.endDate);

            _getWebsiteAccounts();
            _getByDateAndWebId();

        }

        function _checkClick() {
            console.log("CLICK HANDLER", vm.filters.queryWebsiteId, vm.filters.queryStartDate, vm.filters.queryEndDate);
            console.log("NOT THE VM.FILTER STUFF", vm.startDate, vm.endDate, vm.websiteId);

        }

        function _open1() {
            vm.popup1.opened = true;
        }
        function _open2() {
            vm.popup2.opened = true;
        }

        //getting the data for all charts
        function _getSuccess(data) {
            vm.notify(function () {
                vm.reportData = data.items;
                console.log("I got the data!", vm.reportData);
                vm.totalRegistered = 0;
                vm.totalReferrals = 0;
                vm.totalRegisteredBarChart = 0;

                if (vm.reportData == null) { //------------------------------------------------

                    toastr.error("Sorry, no data to show for the selected filters.");

                }

                //for the bar chart:
                vm.seriesBar = [];
                vm.labelsBar = [];
                vm.dataBar = [];
                vm.webIdGroup = {};
                console.log("before obj", vm.webIdGroup);

                for (var i = 0; i < vm.reportData.length; i++) {
                    var obj = vm.reportData[i].date;
                    vm.totalRegistered += vm.reportData[i].totalRegistered;
                    vm.totalReferrals += vm.reportData[i].totalReferrals;
                    var webId = vm.reportData[i].websiteId
                    vm.seriesBar.push(webId);
                    var date = new Date(vm.reportData[i].date);

                    //the next two lines will set the date with the day of the week to render on the bar chart
                    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
                    var dayName = days[date.getDay()];

                    if (!vm.webIdGroup[obj]) {
                        vm.webIdGroup[obj] = [];
                    }
                    vm.webIdGroup[obj].push(vm.reportData[i].websiteId);

                    vm.labelsBar.push(dayName + " " + (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear());

                    function onlyUnique(value, index, self) {
                        return self.indexOf(value) === index;
                    }
                    var uniqueWebId = vm.seriesBar.filter(onlyUnique);
                    var uniqueDate = vm.labelsBar.filter(onlyUnique);
                    vm.seriesBar = uniqueWebId;
                    vm.labelsBar = uniqueDate;

                }
                console.log("These are the label bars:", vm.labelsBar);
                console.log("These are the seriesBar", vm.seriesBar, vm.seriesBar.length);
                console.log("These are the webGroupIds", vm.webIdGroup);
                for (var i = 0; i < vm.seriesBar.length; i++) {
                    vm.dataBar.push([]); //push the same amount of empty arrays into the databar for the length of seriesBar
                }

                $.each(vm.reportData, function (key, value) {

                    var myIndex = vm.seriesBar.indexOf(value.websiteId);
                    vm.dataBar[myIndex].push(value.totalRegistered);


                });

                //for loop to get the total registered for the bar chart
                for (var i = 0; i < vm.reportData.length; i++) {
                    vm.totalRegisteredBarChart += vm.reportData[i].totalRegistered;
                }

                //for the doughnut Chart:
                vm.labels = ["New Accounts", "Referrals"];
                vm.data = [vm.totalRegistered, vm.totalReferrals];
            });


        }

        //getting website for the dropdown option
        function _getWebsiteAccounts() {
            vm.$websiteService.get(_getWebsiteAccountsSuccess, _getWebsiteAccountsError);
        }

        function _getWebsiteAccountsSuccess(data) {
            vm.notify(function () {
                vm.websites = data.items;
                console.log("websites:", vm.websites);
            });
        }

        function _useFilter() {
            console.log("Website Filter: ", vm.websiteId);

            var start = vm.startDate;
            var end = vm.endDate;


            if (vm.pickDateRange == "Custom") {

                vm.customDates = { 'visibility': 'visible' };
                vm.filters.queryStartDate = null;
                vm.filters.queryEndDate = null;

            }
            else if (vm.pickDateRange == "Today") {
                console.log("what is the dateRange picked?", vm.dateRanges);
                vm.customDates = { 'visibility': 'hidden' };
                vm.endDate = new Date(); //both start and end date is now
                vm.startDate = new Date();

            }
            else if (vm.pickDateRange == "Past Week") {
                vm.customDates = { 'visibility': 'hidden' };

                var d = new Date();
                vm.endDate = new Date();
                vm.startDate = new Date(d.setDate(d.getDate() - 7));



            } else if (vm.pickDateRange == "Past Month") {
                vm.customDates = { 'visibility': 'hidden' };
                var d = new Date();
                vm.currentDay = Date.now();
                vm.endDate = new Date();
                vm.startDate = new Date(d.setDate(d.getDate() - 30));

            }

            vm.filters.queryStartDate = _convertDateTime(vm.startDate);
            vm.filters.queryEndDate = _convertDateTime(vm.endDate);
            vm.filters.queryWebsiteId = vm.websiteId

            console.log("QUERY START DATE", vm.filters.queryStartDate);
            console.log("QUERY END DATE", vm.filters.queryEndDate);
            console.log("Website Id", vm.websiteId);

            vm.filters.websiteId = vm.websiteId;

            _getByDateAndWebId(vm.filters);

        }

        function _getByDateAndWebId() {
            vm.$reportsService.getByDateAndWebId(vm.filters, _getSuccess, _getByDateAndWebIdError);

            console.log("filter", vm.filters);
            vm.filters.websiteId = vm.websiteId;
        }

        function _convertDateTime(date) {
            date = vm.$filter('date')(date, 'yyyy/MM/dd HH:mm:ss', 'UTC');
            console.log("C# Format Date: ", date);
            return date;
        }

        function _clearFilter() {
            console.log("click is working");
            vm.startDate = null;
            vm.endDate = null;
            vm.filters.queryWebsiteId = null;
            vm.filters.queryEndDate = null;
            vm.filters.queryStartDate = null;
            vm.websiteId = null;
            vm.pickDateRange = "";
            vm.customDates = { 'visibility': 'hidden' };

            _getByDateAndWebId();
        }

        function _getWebsiteAccountsError(jqXhr, error) {
            console.error("error with get website", error);
        }

        function _getError(jqXhr, error) {
            console.log("Error on getting data", error);
        }

        function _getByDateAndWebIdError(jqXhr, error) {
            console.log("Error on getting data", error);
            toastr.error("Sorry, no data to show for the selected filters.");

        }
    }
})();