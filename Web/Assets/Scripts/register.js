$(document).ready(function () {
  $('#Email').focusout(function () {
    verifyEmailField($(this));
  });

  function verifyEmailField(emailField) {
    var emailError = emailField.siblings('.field-validation-valid');
    emailField.addClass('invalid-field');
    emailError.addClass('invalid-field-label').text('Awesome');
  }
});