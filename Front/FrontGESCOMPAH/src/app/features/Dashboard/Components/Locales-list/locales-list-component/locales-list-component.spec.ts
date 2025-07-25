import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocalesListComponent } from './locales-list-component';

describe('LocalesListComponent', () => {
  let component: LocalesListComponent;
  let fixture: ComponentFixture<LocalesListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocalesListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LocalesListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
