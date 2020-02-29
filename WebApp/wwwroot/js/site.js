const divLoader = $('.div-loader');

function PopulateStatusMessage(element, messageType, message) {
    if (messageType && message) {
        if (messageType === 'danger' || messageType === 'error') {
            messageType = 'danger'
        }

        $(element).attr('class', `alert alert-${messageType}`).text(message).show();
    }
    else {
        $(element).attr('class', '').text('').hide();
    }
}
