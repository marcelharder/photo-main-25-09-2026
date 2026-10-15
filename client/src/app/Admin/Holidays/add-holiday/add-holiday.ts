
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-add-holiday',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-holiday.html',
  styleUrls: ['./add-holiday.scss']
})
export class addHoliday {
  @Output() dataEmitter = new EventEmitter<string>();

  hm = {
    title: ''
  };

  hideAddHoliday(): void {
    this.dataEmitter.emit('hide');
  }

  saveHoliday(): void {
    // Original save functionality needs to be restored.
    console.log('Holiday:', this.hm);
  }
}
