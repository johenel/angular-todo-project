import { HttpClient } from '@angular/common/http';
import { Component, Inject, Input, Output, EventEmitter, SimpleChange, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-tag',
  templateUrl: './tag.component.html',
})

export class TagComponent implements OnChanges {
  @Output() tagsEmitted = new EventEmitter<Tag[]>();
  @Input() tags: Tag[] = [];

  @Output() selectedTagEmitted = new EventEmitter<number|null>();
  @Input() selectedTag: number|null = null;

  @Input() todos: Todos[] = [];

  public _http: HttpClient;
  public _baseUrl: string;
  public hasDue: boolean = false;
  public search: string = "";
  public showCreateTagModal: boolean = false;
  public showDeleteTagModal: boolean = false;
  public formName: string = "";

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        this._http = http;
        this._baseUrl = baseUrl;

        this.loadTags();
    }

    ngOnChanges(changes: SimpleChanges) {
      if (changes.todos) {
        this.loadTags()
      }
    }

    get deleteTagConfirmationMessage(): string {
      if (this.selectedTag) {
        let tag = this.tags.filter(tag => tag.id == this.selectedTag)[0]
        return "Are you sure deleting this tag with " + tag?.count + " todo(s) tagged?";
      }

      return "";
    }

    get validForm(): boolean {

      return this.formName ? true : false;
    }

    get filteredTags(): Tag[] {
        if (this.search) {
          return this.tags.filter((tag) => tag.name.toLocaleLowerCase().includes(this.search.toLowerCase()))
        }

        return this.tags;
    }

    loadTags() {
        this._http.get<Tag[]>(this._baseUrl + 'tags').subscribe(result => {
            this.tags = result;
            this.tagsEmitted.emit(this.tags);
          }, error => console.error(error));
    }

    createTag() {
      if (this.validForm) {
        this._http.post<void>(this._baseUrl + 'tags', {name: this.formName}).subscribe(result => {
            this.formName = "";
            this.showCreateTagModal = false;
            this.loadTags()
        })
      } else {
        alert("Please fill required fields!");
      }
    }

    deleteTag() {
      if (this.selectedTag) {
        this._http.delete<void>(this._baseUrl + 'tags/' + this.selectedTag).subscribe(result => {
          this.showDeleteTagModal = false;
          this.selectTag(null)
          this.loadTags()
        })
      }
    }

    tagIcon(hasDue: boolean):string {
      return hasDue ? "assets/tag-red.png" : "assets/tag-green.png"
    }

    selectTag(tagId: number|null) {
      this.selectedTag = tagId;
      this.selectedTagEmitted.emit(tagId);
    }

    goShowCreateTagModal() {
      this.showCreateTagModal = true;
    }

    closeCreateTagModal() {
      this.formName = "";
      this.showCreateTagModal = false;
    }

    closeDeleteTagModal() {
      this.showDeleteTagModal = false;
    }
}

interface Tag {
  id: number,
  name: string,
  count: number,
}

interface Todos {
  id: number,
  task: string,
  bg_color: string|null,
  order: number,
  completed: boolean,
  due_date: string|null,
  tags: any
}
