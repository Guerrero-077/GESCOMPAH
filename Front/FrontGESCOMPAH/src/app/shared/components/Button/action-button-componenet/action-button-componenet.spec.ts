import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActionButtonComponenet } from './action-button-componenet';

describe('ActionButtonComponenet', () => {
  let component: ActionButtonComponenet;
  let fixture: ComponentFixture<ActionButtonComponenet>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ActionButtonComponenet]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ActionButtonComponenet);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
