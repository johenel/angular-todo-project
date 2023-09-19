import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TagComponent } from './tag.component';

describe('TagComponent', () => {
  let fixture: ComponentFixture<TagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagComponent);
    fixture.detectChanges();
  })
  
  it('should create a tag', async(()=> {
    const addTagBtn = fixture.nativeElement.querySelector('#btnAddTag');
    addTagBtn.click();
    fixture.detectChanges();
    const tagNameInput = fixture.nativeElement.querySelector('#inputTagName');
    tagNameInput.value = "test tag";
    const addTagSubmit = fixture.nativeElement.querySelector('#addTagSubmit');
    addTagSubmit.click();
    fixture.detectChanges();

    const tagCreated = fixture.nativeElement.querySelector('.tag-created');

    expect(tagCreated.length > 0);
  }))
});
