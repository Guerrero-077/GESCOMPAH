import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImagePreviewDialogComponentComponent } from './image-preview-dialog-component.component';

describe('ImagePreviewDialogComponentComponent', () => {
  let component: ImagePreviewDialogComponentComponent;
  let fixture: ComponentFixture<ImagePreviewDialogComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImagePreviewDialogComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImagePreviewDialogComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
