
$(function () {

    $(".newSubscription").change(function () {
        calculateTotalAmount();
    });

    $(".existingSubscription").change(function () {
        calculateTotalAmount();
    });

    function calculateTotalAmount() {
        var total = 0;

        $.each($('.newSubscription'), function (index, value) {
            if ($(this).is(':checked')) {
                var price = parseInt($(this).parent().attr('data-price'));
                total += price;
            }
        });

        $.each($('.existingSubscription'), function (index, value) {
            if ($(this).is(':checked')) {
                var price = parseInt($(this).parent().attr('data-price'));
                total += price;
            }
        });

        $(".totalAmount").text(total);
    }
});