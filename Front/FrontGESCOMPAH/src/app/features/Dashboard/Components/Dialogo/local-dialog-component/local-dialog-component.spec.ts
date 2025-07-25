import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocalDialogComponent } from './local-dialog-component';

describe('LocalDialogComponent', () => {
  let component: LocalDialogComponent;
  let fixture: ComponentFixture<LocalDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocalDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LocalDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
