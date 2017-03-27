
sabio.services.referrals = sabio.services.referrals || {};

//getting all the sent referrals
sabio.services.referrals.getAllReferralsByUserId = function (Id, onSuccess, onError) {
    var url = "/api/referral/get" + Id;
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , success: onSuccess
        , error: onError
        ,type: "GET"
    };
    $.ajax(url, settings);
}

sabio.services.referrals.insertReferralRequest = function (data, onSuccess, onError) {
    var url = "/api/referral/post";
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , data: data
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "POST"

    };
    $.ajax(url, settings);
}

sabio.services.referrals.getAllReferrals = function (onSuccess, onError) {
    var url = "/api/referral/get";
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , success: onSuccess
        , error: onError
        ,type: "GET"
    };
    $.ajax(url, settings);
}

//get the token information for the logged in user and only for the coupon type 3:
sabio.services.referrals.getReferralsByUserIdAndCouponTypePagination = function (data, onSuccess, onError) {
    var url = "/api/referral/getPag";
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , data: data 
        , success: onSuccess
        , error: onError
        , type: "GET"
    };
    $.ajax(url, settings);
}


sabio.services.referrals.getReferralsByUserIdAndCouponType = function (onSuccess, onError) {
    var url = "/api/referral/getUserId";
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "GET"
    };
    $.ajax(url, settings);
}
//Delete:
sabio.services.referrals.deleteReferralById = function (id, onSuccess, onError) {
    var url = "/api/referral/delete" + id;
    var settings = {
        cache: false
        , contentType: "application/x-www-form-urlencoded; charset=UTF-8"
        , dataType: "json"
        , success: onSuccess
        , error: onError
        , type: "DELETE"
    };
    $.ajax(url, settings);
}