import { Injectable, inject } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import Pusher, { Channel } from 'pusher-js';
import { environment } from '../../../environments/environment';

export interface PusherNotification {
  title: string;
  message: string;
  type: string;
  timestamp: Date;
  data?: any;
}

@Injectable({
  providedIn: 'root'
})
export class PusherService {
  private pusher: Pusher;
  private channel: Channel | null = null;
  private notificationSubject = new Subject<PusherNotification>();
  public notifications$ = this.notificationSubject.asObservable();

  constructor() {
    // Initialize Pusher
    this.pusher = new Pusher(environment.pusher.appKey, {
      cluster: environment.pusher.cluster
    });

    // Subscribe to default channel
    this.subscribeToChannel(environment.pusher.channel);
  }

  subscribeToChannel(channelName: string): void {
    this.channel = this.pusher.subscribe(channelName);
    
    // Listen to common events
    this.channel.bind('low-stock-alert', (data: any) => {
      this.notificationSubject.next({
        title: 'Low Stock Alert',
        message: data.message || 'Stock level is low',
        type: 'warning',
        timestamp: new Date(),
        data
      });
    });

    this.channel.bind('order-placed', (data: any) => {
      this.notificationSubject.next({
        title: 'New Order',
        message: data.message || 'A new order has been placed',
        type: 'info',
        timestamp: new Date(),
        data
      });
    });

    this.channel.bind('stock-update', (data: any) => {
      this.notificationSubject.next({
        title: 'Stock Updated',
        message: data.message || 'Stock has been updated',
        type: 'success',
        timestamp: new Date(),
        data
      });
    });

    this.channel.bind('system-test', (data: any) => {
      this.notificationSubject.next({
        title: 'System Notification',
        message: data.message || 'Test notification received',
        type: 'info',
        timestamp: new Date(),
        data
      });
    });
  }

  unsubscribeFromChannel(channelName: string): void {
    if (this.channel) {
      this.pusher.unsubscribe(channelName);
      this.channel = null;
    }
  }

  disconnect(): void {
    this.pusher.disconnect();
  }
}
