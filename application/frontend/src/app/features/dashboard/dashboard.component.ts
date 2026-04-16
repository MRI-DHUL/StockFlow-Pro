import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { PusherService, PusherNotification } from '../../core/services/pusher.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly pusherService = inject(PusherService);
  private readonly router = inject(Router);

  currentUser = this.authService.currentUser;
  notifications = signal<PusherNotification[]>([]);

  ngOnInit(): void {
    // Subscribe to Pusher notifications
    this.pusherService.notifications$.subscribe(notification => {
      this.notifications.update(notifs => [notification, ...notifs].slice(0, 10));
    });
  }

  logout(): void {
    this.authService.logout();
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }
}
