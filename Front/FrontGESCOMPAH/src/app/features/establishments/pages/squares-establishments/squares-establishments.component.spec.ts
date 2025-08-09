import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SquaresEstablishmentsComponent } from './squares-establishments.component';

describe('SquaresEstablishmentsComponent', () => {
  let component: SquaresEstablishmentsComponent;
  let fixture: ComponentFixture<SquaresEstablishmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SquaresEstablishmentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SquaresEstablishmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
