import { Component, NgModule } from '@angular/core';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public selectedTag: number|null = null;
  public tags: Tag[] = [];
  public todos: Todos[] = [];

  onTagsEmitted(tags: Tag[]) {
    this.tags = tags;
  }

  onTodosEmitted(todos: Todos[]) {
    this.todos = todos;
  }

  changeSelectedTag(tagId: number|null) {
    this.selectedTag = tagId;
  }
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

interface Tag {
  id: number,
  name: string,
  count: number
}