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
    
    // Listen to low stock detected event (matches backend event name)
    this.channel.bind('low-stock-detected', (data: any) => {
      const message = `${data.productName} (${data.sku}) - Current: ${data.currentQuantity}, Threshold: ${data.threshold}`;
      this.notificationSubject.next({
        title: 'Low Stock Alert',
        message: message,
        type: 'warning',
        timestamp: new Date(data.timestamp || new Date()),
        data
      });
    });

    // Listen to order placed event
    this.channel.bind('order-placed', (data: any) => {
      const message = `Order #${data.orderNumber} by ${data.customerName} - Total: $${data.totalAmount}`;
      this.notificationSubject.next({
        title: 'New Order Placed',
        message: message,
        type: 'info',
        timestamp: new Date(data.timestamp || new Date()),
        data
      });
    });

    // Listen to stock updated event (matches backend event name)
    this.channel.bind('stock-updated', (data: any) => {
      const message = `${data.productName} in ${data.warehouseName} - ${data.movementType}: ${data.quantity} units`;
      this.notificationSubject.next({
        title: 'Stock Updated',
        message: message,
        type: 'success',
        timestamp: new Date(data.timestamp || new Date()),
        data
      });
    });

    // Keep system-test for testing purposes
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
