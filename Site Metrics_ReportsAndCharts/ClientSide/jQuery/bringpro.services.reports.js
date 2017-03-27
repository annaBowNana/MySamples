sabio.services.reports = sabio.services.reports || {};

sabio.services.reports.getByDateAndWebId = function (data, onSuccess, onError) {
    var url = "/api/reports/getFilter";
    var settings = {
        //c2d2set
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

sabio.services.reports.getCsvExport = function (data, onSuccess, onError) {
    var url = "/api/reports/getCsv";
    var settings = {
        //c2d2set
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