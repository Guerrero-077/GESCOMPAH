import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListEstablishmentComponent } from './list-establishment-component';

describe('ListEstablishmentComponent', () => {
  let component: ListEstablishmentComponent;
  let fixture: ComponentFixture<ListEstablishmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListEstablishmentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListEstablishmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
