import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalComponenet } from './modal-componenet';

describe('ModalComponenet', () => {
  let component: ModalComponenet;
  let fixture: ComponentFixture<ModalComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ModalComponenet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ModalComponenet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
