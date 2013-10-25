$(document).ready(function () {
  toastr.options = { "timeOut": "2000", "closeButton": true };
  var currentAccountRoles;

  $('.delete-account').click(function (event) {
    var accountId = $(this).closest('tr').children('td.account-id').text();
    $.ajax({
      url: '/account/delete',
      type: 'post',
      data: { accountId: accountId },
      success: function (data) {
        if (data == 'True') {
          $(event.target).closest('tr').remove();
          toastr.success("Deleted user");
        } else
          toastr.error("Failed to delete user");
      },
      error: function () {
        toastr.error("Failed to delete user");
      }
    });
    return false;
  })

  $('.edit-roles').click(function () {
    var accountEmail = $(this).closest('tr').children('td.account-email').text();
    var accountId = $(this).closest('tr').children('td.account-id').text();
    currentAccountRoles = $(this).closest('tr').children('td.account-roles').children('select');
    populateCurrentUserRoles();
    $('.modal-dialog').show();
    $('#selected-user').val(accountId);
    $('#edit-account-name').text(accountEmail);
    return false;
  });

  function populateCurrentUserRoles() {
    $('#role-selection :input').prop('checked', false);
    var currentRoles = [];
    currentAccountRoles.children().each(function () {
      currentRoles.push($(this).val().replace(" ", ""));
    });
    for (i = 0; i < currentRoles.length; i++) {
      $('#role-' + currentRoles[i]).prop('checked', true);
    }
  }

  $('#close-modal').click(function () {
    $('.modal-dialog').hide()
    return false;
  });

  $('.role-selector').change(function (event) {
    var role = $(event.target).parent('label').text();
    var userId = $('#selected-user').val();
    if ($(event.target).prop('checked'))
      addRole(userId, role);
    else
      removeRole(userId, role);
  });

  function removeRole(userId, role) {
    $.ajax({
      url: '/account/removeRole',
      type: 'post',
      data: { accountId: userId, roleToRemove: role },
      success: function (data) {
        if (data == 'True') {
          toastr.success("Removed role");
          removeRoleFromList(role, currentAccountRoles);
          $("#current-user-roles option:selected").remove();
        } else {
          toastr.error("Failed to remove role");
        }
      },
      error: function () {
        toastr.error("Failed to remove role");
      }
    });
  }

  function addRole(userId, role) {
    $.ajax({
      url: '/account/addRole',
      type: 'post',
      data: { accountId: userId, newRole: role },
      success: function (data) {
        if (data == 'True') {
          toastr.success("Added role");
          $('#current-user-roles').append('<option>' + role + '</option>');
          currentAccountRoles.append('<option>' + role + '</option>');
        } else {
          toastr.error("Failed to add role");
        }
      },
      error: function () {
        toastr.error("Failed to add role");
      }
    });
  }

  function removeRoleFromList(role, listOfRoles) {
    currentAccountRoles.children().each(function () {
      if ($(this).val() == role)
        $(this).remove();
    });
  }

  $('.account-active').change(function () {
    var accountId = $(this).closest('tr').children('td.account-id').text();
    if ($(this).prop('checked'))
      activateUser(accountId);
    else
      deactivateUser(accountId);
  });

  function activateUser(accountId) {
    $.ajax({
      url: '/account/activate',
      type: 'post',
      data: { accountId: accountId },
      success: function (data) {
        if (data == 'True') {
          toastr.success("Activated user");
        } else {
          toastr.error("Failed to activate user");
        }
      },
      error: function () {
        toastr.error("Failed to activate user");
      }
    });
  }

  function deactivateUser(accountId) {
    $.ajax({
      url: '/account/deactivate',
      type: 'post',
      data: { accountId: accountId },
      success: function (data) {
        if (data == 'True') {
          toastr.info("Deactivated user");
        } else {
          toastr.error("Failed to deactivate user");
        }
      },
      error: function () {
        toastr.error("Failed to deactivate user");
      }
    });
  }
});
