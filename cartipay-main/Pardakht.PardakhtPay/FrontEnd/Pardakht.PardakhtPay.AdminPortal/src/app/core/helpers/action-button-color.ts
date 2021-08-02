import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ActionButtonColor {

    buttonColor() {
        return {
            manualAction: "#039BE5",
            deleted: "#039BE5",
            systemAction: "#039BE5",
            highPriority: "#039BE5",
            success: "#039BE5",
            canceled: "#039BE5",
            lowPriority: "#039BE5",
            waiting: "#039BE5",
            inProcess: "#039BE5",
            rejected: "#039BE5",
            mediumPriority: "#039BE5",
            trueValue: "#039BE5",
            falseValue: "#039BE5",
            red: "#FF0000",
            green: "	#32CD32"
        };
    }

}