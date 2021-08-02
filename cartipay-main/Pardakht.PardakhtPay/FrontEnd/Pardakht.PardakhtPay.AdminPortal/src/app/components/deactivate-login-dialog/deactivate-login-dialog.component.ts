import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-deactivate-login-dialog',
    templateUrl: './deactivate-login-dialog.component.html',
    styleUrls: ['./deactivate-login-dialog.component.scss']
})
export class DeactivateLoginDialogComponent implements OnInit {

    message: string;

    constructor(public dialogRef: MatDialogRef<DeactivateLoginDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: string) {
        this.message = data;
    }

    ngOnInit() {
    }

    onNoClick(): void {
        this.dialogRef.close(false);
    }

    onYesClick(): void {
        this.dialogRef.close(true);
    }

}
