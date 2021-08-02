import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class StatusColor {

    statusColor() {
        return {
            manualAction: "#99a8ff",
            deleted: "#BDBDBD",
            systemAction: "#c299ff",
            highPriority: "#d68383",
            success: "#cafc90",
            canceled: "#fcb490",
            lowPriority: "#fce690",
            waiting: "#fce690",
            inProcess: "#fce690",
            rejected: "#ffcc99",
            mediumPriority: "#FFCA28",
            trueValue: "#cafc90",
            falseValue: "#d68383",
            credit: "#bffdfd"

        };
    }

}