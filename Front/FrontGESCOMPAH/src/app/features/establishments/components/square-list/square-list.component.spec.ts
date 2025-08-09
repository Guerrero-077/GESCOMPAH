import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SquareListComponent } from './square-list.component';

describe('SquareListComponent', () => {
  let component: SquareListComponent;
  let fixture: ComponentFixture<SquareListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SquareListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SquareListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
