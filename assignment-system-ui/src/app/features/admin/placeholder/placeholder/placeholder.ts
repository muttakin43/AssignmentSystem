import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-placeholder',
  standalone: true,
  templateUrl: './placeholder.html',
  styleUrl: './placeholder.scss'
})
export class Placeholder {

  private route = inject(ActivatedRoute);

  path = this.route.snapshot.url.join('/');
}