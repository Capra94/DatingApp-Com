import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PhotoManagementComponent } from './photo-management.component';

describe('PhotoManagementComponent', () => {
  let component: PhotoManagementComponent;
  let fixture: ComponentFixture<PhotoManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PhotoManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PhotoManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
