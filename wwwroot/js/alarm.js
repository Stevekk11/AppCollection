function pad(num) {
    return num < 10 ? '0'+num : num;
}

function getCurrentTime() {
    const now = new Date();
    return pad(now.getHours()) + ":" + pad(now.getMinutes());
}

let alarmTriggered = {};

setInterval(() => {
    const currentTime = getCurrentTime();
    console.log("Updating alarm")
    document.querySelectorAll("#alarmTable tbody tr").forEach(row => {
        const alarmTime = row.dataset.alarm;
        const isOn = row.dataset.isOn === "true";
        console.log(currentTime)
        console.log(alarmTime)
        if (isOn && alarmTime === currentTime) {
            if (!alarmTriggered[alarmTime]) { // aby se alarm spustil jen jednou
                alarmTriggered[alarmTime] = true;

                const type = row.dataset.type;
                const sound = row.dataset.sound;
                const audio = document.getElementById("alarmAudio");
                if (type === "Beep") {
                    console.log("playing beep")
                    // Hraní vestavěného beep (krátký zvuk)
                    audio.src = "/media/beep.mp3";
                    audio.loop = true;
                    audio.play();
                } else if (type === "Nature" || type === "Radio") {
                    // Nature = link na mp3, Radio = webstream url
                    audio.src = "/media/"+sound+".mp3";
                    audio.loop = type === "Nature"; // příroda může loopovat, rádio ne
                    audio.play();
                }

                document.querySelector('#alrm').innerHTML= "🔔Zvoní budík🔔";

                // Zastavení alarmu po 60 sekundách, případně doplň o snooze
                setTimeout(() => {
                    audio.pause();
                    audio.currentTime = 0;
                    document.querySelector('#alrm').innerHTML= "";
                }, 60000);
            }
        } else if (alarmTime !== currentTime && alarmTriggered[alarmTime]) {
            alarmTriggered[alarmTime] = false;
        }
    });
}, 1000);
