"use strict";
$(async function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/batchPublisher").build();
    await connection.start();
    connection.stream("Previous", 10, 500)
        .subscribe({
            next: (batchInfo) => {
                console.log(batchInfo);
                $('#batches').append('<tr><td>' + batchInfo.batchId + '</td><td>' + batchInfo.pressure.mean + '</td><td>' + batchInfo.pressure.deviation + '</td><td>' + batchInfo.temperature.mean + '</td><td>' + batchInfo.temperature.deviation + '</td></tr>');
            }
        });
    connection.on("OnBatchInfoPublished", function (batchInfo) {
        console.log(batchInfo);
        $('#batches').append('<tr><td>' + batchInfo.batchId + '</td><td>' + batchInfo.pressure.mean + '</td><td>' + batchInfo.pressure.deviation + '</td><td>' + batchInfo.temperature.mean + '</td><td>' + batchInfo.temperature.deviation + '</td></tr>');
    });
});