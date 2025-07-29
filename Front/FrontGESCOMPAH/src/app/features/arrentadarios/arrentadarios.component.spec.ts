import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArrentadariosComponent } from './arrentadarios.component';

describe('ArrentadariosComponent', () => {
  let component: ArrentadariosComponent;
  let fixture: ComponentFixture<ArrentadariosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ArrentadariosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ArrentadariosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
