import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Inject, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
})

export class TodoComponent implements OnInit, OnChanges {
  @Input() tags: Tag[] = [];
  @Input() selectedTag: number|null = null;

  @Output() todosEmitted = new EventEmitter<Todos[]>();
  @Input() todos: Todos[] = [];

  public showCreateOrUpdateTodoModal: boolean = false;
  public showTagSelectorModal: boolean = false;
  public showDeleteTodoModal: boolean = false;
  public bgColor: string = '';
  public _http: HttpClient;
  public _baseUrl: string;
  public selectedTags: number[] = [];
  public formGroup!: FormGroup;
  public tagSelectorSelectedIndex = 0;
  public deleteTodoId: number|null = null;
  public updateMode: boolean = false;
  public updateTodoId: number|null = null;
  public search: string = "";

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
}

  ngOnInit() {
    this.initializeFormGroup();
    this.loadTodos();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.selectedTag) {
      this.loadTodos()
    }
  } 

  initializeFormGroup(): void {
    this.formGroup = new FormGroup({
      Id: new FormControl(null),
      Task: new FormControl('', Validators.required),
      BackgroundColor: new FormControl('', Validators.required),
      DueDate: new FormControl(''),
      TagIds: new FormControl([]),
      Completed: new FormControl(false)
    })
  }

  get filteredTodos(): Todos[] {
    if (this.search) {
      return this.todos.filter((todo) => todo.task.toLocaleLowerCase().includes(this.search.toLowerCase()))
    }

    return this.todos;
}

  get filteredTodoByTag() {
    return this.todos.filter(todo => {
      if (this.selectedTag) {
        let tags: Tag[] = todo.tags;
         if (tags.filter(tag => tag.id == this.selectedTag).length > 0) {
          return true;
         }
      }

      return false;
    })
  }

  get availableTags() {
    return this.tags?.filter(tag => !this.selectedTags.includes(tag.id))
  }

  get displayAddTodoModalTags()
  {
    return this.tags?.filter(tag => this.selectedTags.includes(tag.id))
  }

  get isFormGroupInvalid()
  {
    return this.formGroup.invalid;
  }

  updateOrCreate(updateMode: boolean = false, todoId: number|null) {
    this.updateMode = updateMode;
    this.updateTodoId = todoId;

    if (this.updateMode) {
      this.openUpdateModal();
    } else {
      this.createTodo();
    }
  }

  loadTodos() {
    let endPoint = "todos";
    if (this.selectedTag) {
      endPoint = "todos?tagId=" + this.selectedTag;
    }

    this._http.get<Todos[]>(this._baseUrl + endPoint).subscribe(result => {
      this.todos = result;
      this.todosEmitted.emit(this.todos);
    }, error => console.error(error));
  }

  goShowCreateTodoModal() {
    this.showCreateOrUpdateTodoModal = true;
  }

  closeCreateTodoModal() {
    this.formGroup.reset();
    this.showCreateOrUpdateTodoModal = false;
    this.selectedTags = []
  }

  updateOrCreateTodo() {
    if (this.updateMode) {
      this.updateTodo();
    } else {
      this.createTodo();
    }
  }

  createTodo() {
    if (this.formGroup.valid) {
      this.formGroup.get('TagIds')?.patchValue(this.selectedTags);
      this._http.post<void>(this._baseUrl + 'todos', this.formGroup.value).subscribe(result => {
          this.showCreateOrUpdateTodoModal = false;
          this.formGroup.reset();
          this.selectedTags = [];
          this.loadTodos()
      })
    }
  }

  openUpdateModal() {
    if (this.updateTodoId) {
      this.setUpdateFormGroup(this.updateTodoId);
      this.showCreateOrUpdateTodoModal = true;
   }
  }

  updateTodo() {
    this.formGroup.get('TagIds')?.patchValue(this.selectedTags);
    this.formGroup.get('Id')?.patchValue(this.updateTodoId);
    this._http.put<void>(this._baseUrl + 'todos/' + this.updateTodoId, this.formGroup.value).subscribe(result => {
      this.showCreateOrUpdateTodoModal = false;
      this.loadTodos()
    })
  }

  toggleCompleteTodo(todoId: number) {
    this.setUpdateFormGroup(todoId);
    this.updateTodoId = todoId;
    this.updateTodo();
  }

  deleteTodo() {
    if (this.deleteTodoId) {
      this._http.delete<void>(this._baseUrl + 'todos/' + this.deleteTodoId).subscribe(result => {
        this.showDeleteTodoModal = false;
        this.loadTodos()
      })
    }
  }

  colorPickerChange(event: any) {
    this.formGroup.get('BackgroundColor')?.patchValue(event)
  }

  onTagSelect(event: any) {
    this.selectedTags = [parseInt(event.target.value), ...this.selectedTags];
    console.log(this.selectedTags);
    this.tagSelectorSelectedIndex = 0;
    this.showTagSelectorModal = false;
  }

  removeFromSelectedTag(removeTag: any) {
    this.selectedTags = this.selectedTags.filter(tag => tag != removeTag)
  }

  getDueDate(date: any) {
    if (typeof date === 'string') {
      return date
    }

    return "(Any date)"
  }

  setUpdateFormGroup(todoId: number) {
    let todo = this.todos.filter(todo => todo.id == todoId)[0];

    this.formGroup.get('Task')?.patchValue(todo.task);
    this.formGroup.get('BackgroundColor')?.patchValue(todo.bg_color);
    this.formGroup.get('DueDate')?.patchValue(todo.due_date);
    this.formGroup.get('Completed')?.patchValue(!todo.completed);
    this.formGroup.get('TagIds')?.patchValue(todo.tags.map(tag => tag.id));
  }
}

interface Todos {
  id: number,
  task: string,
  bg_color: string|null,
  order: number,
  completed: boolean,
  due_date: string|null,
  tags: Tag[]
}

interface Tag {
  id: number,
  name: string,
  count: number
}