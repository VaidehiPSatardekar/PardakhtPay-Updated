import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QRCodeRegistrationComponent } from './qr-code-registration.component';

describe('QRCodeRegistrationComponent', () => {
    let component: QRCodeRegistrationComponent;
    let fixture: ComponentFixture<QRCodeRegistrationComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [QRCodeRegistrationComponent]
        })
            .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(QRCodeRegistrationComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
