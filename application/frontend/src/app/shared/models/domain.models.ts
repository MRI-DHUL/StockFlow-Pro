// Product Models
export interface Product {
  id: string;
  name: string;
  sku: string;
  description?: string;
  category: string;
  unitPrice: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateProductDto {
  name: string;
  sku: string;
  description?: string;
  category: string;
  unitPrice: number;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  category: string;
  unitPrice: number;
}

// Inventory Models
export interface InventoryItem {
  id: string;
  productId: string;
  productName: string;
  warehouseId: string;
  warehouseName: string;
  quantity: number;
  threshold: number;
  lastUpdated: Date;
}

export interface UpdateInventoryDto {
  productId: string;
  warehouseId: string;
  quantity: number;
  threshold: number;
}

// Order Models
export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4
}

export interface Order {
  id: string;
  orderNumber: string;
  customerId?: string;
  customerName?: string;
  customerEmail?: string;
  status: OrderStatus;
  totalAmount: number;
  createdAt: Date;
  updatedAt?: Date;
  orderItems?: OrderItem[];
}

export interface OrderItem {
  id: string;
  orderId: string;
  productId: string;
  productName?: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface CreateOrderDto {
  customerId?: string;
  customerName?: string;
  customerEmail?: string;
  shippingAddress?: string;
  notes?: string;
  orderItems: CreateOrderItemDto[];
}

export interface CreateOrderItemDto {
  productId: string;
  quantity: number;
}

export interface UpdateOrderDto {
  status: OrderStatus;
  shippingAddress?: string;
  notes?: string;
}

// Purchase Order Models
export enum PurchaseOrderStatus {
  Draft = 0,
  Submitted = 1,
  Approved = 2,
  Received = 3,
  Cancelled = 4
}

export interface PurchaseOrder {
  id: string;
  poNumber: string;
  supplierId: string;
  supplierName: string;
  expectedDeliveryDate?: Date;
  status: PurchaseOrderStatus;
  totalAmount: number;
  createdAt: Date;
  updatedAt?: Date;
  purchaseOrderItems?: PurchaseOrderItem[];
}

export interface PurchaseOrderItem {
  id: string;
  purchaseOrderId: string;
  productId: string;
  productName?: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface CreatePurchaseOrderDto {
  supplierId: string;
  expectedDeliveryDate?: Date;
  purchaseOrderItems: CreatePurchaseOrderItemDto[];
}

export interface CreatePurchaseOrderItemDto {
  productId: string;
  quantity: number;
  unitPrice: number;
}

export interface UpdatePurchaseOrderDto {
  status: PurchaseOrderStatus;
  expectedDeliveryDate?: Date;
  notes?: string;
}

// Supplier Models
export interface Supplier {
  id: string;
  name: string;
  contactInfo?: string;
  email?: string;
  phone?: string;
  leadTimeDays?: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateSupplierDto {
  name: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
}

export interface UpdateSupplierDto {
  name: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
}

// Warehouse Models
export interface Warehouse {
  id: string;
  name: string;
  location: string;
  capacity: number;
  contactInfo?: string;
  email?: string;
  phone?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateWarehouseDto {
  name: string;
  location: string;
  capacity: number;
}

export interface UpdateWarehouseDto {
  name: string;
  location: string;
  capacity: number;
}

// Stock Movement Models
export enum MovementType {
  In = 0,
  Out = 1,
  Transfer = 2,
  Adjustment = 3
}

export interface StockMovement {
  id: string;
  productId: string;
  productName?: string;
  fromWarehouseId?: string;
  fromWarehouseName?: string;
  toWarehouseId?: string;
  toWarehouseName?: string;
  type: MovementType;
  quantity: number;
  referenceNumber?: string;
  notes?: string;
  movementDate: Date;
  performedBy?: string;
}

export interface CreateStockMovementDto {
  productId: string;
  fromWarehouseId?: string;
  toWarehouseId?: string;
  type: MovementType;
  quantity: number;
  referenceNumber?: string;
  notes?: string;
}

// Audit Log Models
export interface AuditLog {
  id: string;
  entityName: string;
  entityId: string;
  action: string;
  userName?: string;
  timestamp: Date;
  oldValues?: string;
  newValues?: string;
  ipAddress?: string;
}
