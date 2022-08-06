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

function Register() {

    var name = jQuery('#inputNameRegister').val();
    var email = jQuery('#inputEmailRegister').val();
    var password = jQuery('#inputPhoneRegister').val();
    var phone = jQuery('#inputPasswordRegister').val();
    var passwordAgain = jQuery('#inputPasswordAgainRegister').val();


    if (password != passwordAgain) {
        alert("Passwords are not the same");

    } else {

        var params = "?name=" + name + "?email=" + email + "&password=" + password  "&phone=" + phone;
        $.ajax({
            type: "POST",
            url: "http://localhost:50706/api/Authanticate/Register" + params,
            cache: false,
            success: function (data) {

                if (data !== null && data !== undefined && data !== "undefined" && data !== "") {
                    alert("Register Successfull.")
                } else {

                    alert("Error accured while Process.");
                }
            },
            error: function (xhr, txtStatus, errorThrown) {
                alert("Error Code : " + xhr.status + " " + txtStatus + "\n" + errorThrown);

            }


        });
    }

}