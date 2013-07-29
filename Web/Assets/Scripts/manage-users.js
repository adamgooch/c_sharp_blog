$(".role-selector").change(function( event ) {
    var target = $(event.target);
    var id = target.closest('tr').find('.userId').text();
    var newRole = { id: id, role: target[0].selectedIndex };
    $.ajax({
        url: "/account/editrole",
        type: "post",
        data: newRole,
        success: function() {
            
        },
        error: function() {
            alert("Role Did Not Save");
        }
    });
})