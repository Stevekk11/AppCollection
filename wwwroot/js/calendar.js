$(document).ready(function () {
    // Initialize FullCalendar
    $('#calendar').fullCalendar({
        locale: navigator.language || navigator.userLanguage,
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        buttonText: {
            today:    'Dnes',
            month:    'Měsíc',
            week:     'Týden',
            day:      'Den',
            list:     'Agenda',
            prev:     'Předchozí',
            next:     'Další'
        },
        selectable: true,
        selectHelper: true,
        editable: true,
        events: {
            url: '/Events/UserEvents',
            type: 'GET',
            error: function () {
                alert('There was an error while fetching events!');
            }
        },
        select: function (start, end) {
            $('#eventId').val('');
            $('#eventName').val('');
            $('#eventStart').val(moment(start).format('YYYY-MM-DDTHH:mm'));
            $('#eventEnd').val(moment(end).format('YYYY-MM-DDTHH:mm'));
            $('#deleteEventBtn').hide();
            $('#eventModal').modal('show');
        },
        eventClick: function (event) {
            $('#eventId').val(event.id);
            $('#eventName').val(event.title);
            $('#eventStart').val(moment(event.start).format('YYYY-MM-DDTHH:mm'));
            $('#eventEnd').val(moment(event.end).format('YYYY-MM-DDTHH:mm'));
            $('#deleteEventBtn').show();
            $('#eventModal').modal('show');
        },
        eventDrop: function (event, delta, revertFunc) {
            // Update event time after drag & drop
            updateEvent(event);
        },
        eventResize: function (event, delta, revertFunc) {
            // Update event time after resize
            updateEvent(event);
        }
    });

    // Save (Add/Edit) Event
    $('#eventForm').submit(function (e) {
        e.preventDefault();
        var id = $('#eventId').val();
        var eventData = {
            Name: $('#eventName').val(),
            StartDate: $('#eventStart').val(),
            EndDate: $('#eventEnd').val()
        };

        if (id) {
            // Edit
            $.ajax({
                url: '/Events/Edit/' + id,
                type: 'PUT',
                data: JSON.stringify(eventData),
                contentType: 'application/json',
                success: function () {
                    $('#calendar').fullCalendar('refetchEvents');
                    $('#eventModal').modal('hide');
                }
            });
        } else {
            // Add
            $.ajax({
                url: '/Events/Add',
                type: 'POST',
                data: JSON.stringify(eventData),
                contentType: 'application/json',
                success: function () {
                    $('#calendar').fullCalendar('refetchEvents');
                    $('#eventModal').modal('hide');
                }
            });
        }
    });

    // Delete Event
    $('#deleteEventBtn').click(function () {
        var id = $('#eventId').val();
        if (id && confirm('Smazat událost?')) {
            $.ajax({
                url: '/Events/Delete/' + id,
                type: 'DELETE',
                success: function () {
                    $('#calendar').fullCalendar('refetchEvents');
                    $('#eventModal').modal('hide');
                }
            });
        }
    });

    // Helper for drag/drop and resize
    function updateEvent(event) {
        var eventData = {
            Name: event.title,
            StartDate: event.start.format(),
            EndDate: event.end ? event.end.format() : event.start.format()
        };
        $.ajax({
            url: '/Events/Edit/' + event.id,
            type: 'PUT',
            data: JSON.stringify(eventData),
            contentType: 'application/json',
            success: function () {
                $('#calendar').fullCalendar('refetchEvents');
            }
        });
    }
});
