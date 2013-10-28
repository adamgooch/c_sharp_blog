$(document).ready(function () {
  $('#Email').focusout(function () {
    validateEmail($(this));
  });

  function validateEmail(emailField) {
    $.ajax({
      url: '/account/usernameexists',
      type: 'get',
      data: { email: emailField.val() },
      success: function (data) {
        if (data == 'True') {
          var emailError = emailField.siblings('.field-validation-valid');
          emailField.addClass('input-validation-error');
          emailError.addClass('field-validation-error').text('That email has already been registered');
        } else {
          var emailError = emailField.siblings('.field-validation-valid');
          emailField.removeClass('input-validation-error');
          emailError.removeClass('field-validation-error').text('');
        }
      },
      error: function () {
        toastr.error("Error occurred");
      }
    });
  }
});