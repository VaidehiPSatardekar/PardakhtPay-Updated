import { Component, OnInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-delete-login-dialog',
  templateUrl: './delete-login-dialog.component.html',
  styleUrls: ['./delete-login-dialog.component.scss']
})
export class DeleteLoginDialogComponent implements OnInit {

    friendlyName: string;

    constructor(public dialogRef: MatDialogRef<DeleteLoginDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: string) {
        this.friendlyName = data;
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
