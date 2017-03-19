//Angular controller factory for listing table:
//create a controller and add properties to the controller
//to represent the different objects on the page;
//also inject the service from step one which will allow the controller to make ajax calls
(function () {
    "use strict";

    angular.module(APPNAME)
        .controller("referralController", referralController);
    referralController.$inject = ["$scope", "$baseController", "$referralService", "toastr", "$uibModal"];

    function referralController(
        $scope
        , $baseController
        , $referralService
        , toastr
        , $uibModal) {

        var vm = this;
        vm.referralEmail = null;
        vm.referralForm = null;
        vm.referrals = null;
        vm.selectedReferral = null;
        vm.toggleForm = false;
        vm.inputs = [];

        vm.removeLine = true;
        vm.getsResultsPage = 1;


        vm.payload = {
            "itemsPerPage": 10
            , "currentPage": 1
            , "totalItems": 0

        }

        vm.$referralService = $referralService;
        vm.$scope = $scope;

        vm.$uibModal = $uibModal;

        //  modal directive
        vm.modalSelected = null;

        //hoisting all the functions
        vm.selectReferral = _selectReferral;
        vm.onGetReferralsSuccess = _onGetReferralsSuccess;
        vm.onInsertReferralSuccess = _onInsertReferralSuccess;
        vm.submitForm = _submitForm;
        vm.insertreferral = _insertreferral;
        vm.onInsertReferralError = _onInsertReferralError;
        vm.addEmail = _addEmail;
        vm.removeEmail = _removeEmail;
        vm.openModal = _openModal;
        vm.resetForm = _resetForm;
        vm.pageChanged = _pageChanged;
        vm.showForm = _showForm;



        $baseController.merge(vm, $baseController);
        vm.notify = vm.$referralService.getNotifier($scope);

        //similar to startup function in jqurey
        render();
        vm.addEmail();

        function render() {
            console.log('vm.payload', vm.payload);

            //Get with pagination:
            vm.$referralService.getReferralsByUserIdAndCouponTypePagination(vm.payload, vm.onGetReferralsSuccess, vm.onGetReferralError);

            //console.log("new referral:" , vm.newReferral);

        }

        function _showForm() {
            console.log("Show the invite form!");
            vm.toggleForm = !vm.toggleForm;
        }

        //opening a modal on click of button
        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: 'modalContent.html',
                controller: 'modalController as mc',
                size: 'sm',
                resolve: {
                    items: function () {
                        return vm.modalItems;
                    }
                }
            });
        }

        //add and remove buttons: showning and hiding the delete button depending on the
        //length of the inputs
        function _addEmail() {

            vm.inputs.push({})
            if (vm.inputs.length > 1) {
                vm.removeLine = false;
            }
        }

        function _removeEmail(index) {
            console.log(vm.inputs.length);
            if (vm.inputs.length <= 2) {
                vm.removeLine = true;
            }
            vm.inputs.splice(index, 1);
        }

        //adding new email to send invitation
        function _insertreferral() {
            var data = {
                "inviteEmail": vm.inputs
            } // I need to make this data a local variable because if this is hoisted up as vm.data, it will
            //keep a copy of of vm.inputs, so that when i try to submit another email after an initial submit, 
            // it won't keep the original inputs. 

            console.log("insert starting", data);

            vm.$referralService.insertReferralRequest(data, vm.onInsertReferralSuccess, vm.onInsertReferralError);

        }

        //gets the info from DB and renders on the page
        function _onInsertReferralSuccess(data) {
            console.log(data);
            console.log("Insert Referral successful");
            toastr.success("Email(s) sent successfully!");
            render();
            vm.resetForm();
        }

		//display the data using pagination
        function _onGetReferralsSuccess(data) {
            console.log("referral data:", data);
            vm.notify(function () {
                vm.referrals = data.items;
                vm.payload.totalItems = data.totalItems;
                console.log("THIS IS MY PAYLOAD", vm.payload);
                var firstNumber = ((vm.payload.currentPage - 1) * vm.payload.itemsPerPage) + 1;

                if (vm.payload.currentPage * 10 < vm.payload.totalItems) {
                    var secondNumber = (vm.payload.currentPage * vm.payload.itemsPerPage);
                    $('.infos').text('Showing ' + firstNumber + ' ' + 'to ' + secondNumber + ' of ' + vm.payload.totalItems);
                } else {
                    var secondNumber = vm.payload.totalItems;
                    $('.infos').text('Showing ' + firstNumber + ' to ' + secondNumber);
                }

                window.scrollTo(0, 0);
                console.log("referral items: ", vm.referrals);
            });
        }

        function _pageChanged(newPage) {
            vm.payload.currentPage = newPage;
            render();
        }

        function _selectReferral(aReferral) {
            console.log(aReferral);
            vm.selectedReferral = aReferral;
        }

        function _onInsertReferralError(jqXhr, error) {
            toastr.error("Email(s) could not be sent!");

            vm.resetForm();

            console.error("error with get all", error);
        }

        function _onGetReferralError(jqXhr, error) {
            console.error("error with get all", error);
        }

        function _resetForm() {
            console.log("reset form");
            vm.inputs = [];
            vm.inputs.push({});

        }

        //form validation:

        function _submitForm(isValid) {
            if (isValid) {
                console.log("data is valid! go save this object -> ");
                vm.insertreferral();
                vm.resetForm();

            } else {
                console.log("form submitted with invalid data :(");
                toastr.error("Email(s) could not be sent!");
                vm.resetForm();

            }
        };

    }
})();