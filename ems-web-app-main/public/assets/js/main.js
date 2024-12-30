jQuery(document).ready(function () {
  "use strict";
  $("#menuToggle").on("click", function () {
    $("body").toggleClass("open");
  });

  $("#searchTrigger").click(function () {
    $("header .form-inline").css("display", "block");
  });
  $(function () {
    $('[data-toggle="tooltip"]').tooltip();
  });
});
