function Login() {

    var email = jQuery('#inputEmail').val();
    var password = jQuery('#inputPassword').val();
    var params = "?email=" + email + "&password=" + password;
    $.ajax({
        type: "POST",
        url: "http://localhost:50970/Login/Login" + params,
        cache: false,
        success: function (data) {

            if (data !== null && data !== undefined && data !== "undefined" && data !== "") {

                window.location = 'http://localhost:50970/' + data;
            } else {
                alert("Error accured while Login.");
            }
        },
        error: function (xhr, txtStatus, errorThrown) {
            alert("Error Code : " + xhr.status + " " + txtStatus + "\n" + errorThrown);

        }


    });
}