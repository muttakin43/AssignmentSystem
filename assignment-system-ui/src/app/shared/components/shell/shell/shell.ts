import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';

import { NavbarComponent } from '../../navbar/navbar/navbar';
import { SidebarComponent } from '../../sidebar/sidebar/sidebar';

@Component({
  selector: 'app-shell',
  imports: [
    RouterOutlet,
    MatSidenavModule,
    NavbarComponent,
    SidebarComponent
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class ShellComponent {}