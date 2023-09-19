import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})

export class ModalComponent {
  @Input() isOpen = false;
  @Output() propertyEmitter: EventEmitter<boolean> = new EventEmitter<boolean>();

  closeModal(): void {
    this.propertyEmitter.emit(false);
  }
}