// Product Models
export interface Product {
  id: number;
  name: string;
  sku: string;
  description?: string;
  category: string;
  unitPrice: number;
  reorderLevel: number;
  quantityPerUnit: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateProductDto {
  name: string;
  sku: string;
  description?: string;
  category: string;
  unitPrice: number;
  reorderLevel: number;
  quantityPerUnit: string;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  category: string;
  unitPrice: number;
  reorderLevel: number;
  quantityPerUnit: string;
  isActive: boolean;
}

// Inventory Models
export interface InventoryItem {
  id: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  quantityOnHand: number;
  quantityReserved: number;
  quantityAvailable: number;
  lastUpdated: Date;
}

export interface UpdateInventoryDto {
  quantityOnHand: number;
  quantityReserved: number;
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
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerId?: string;
  customerName?: string;
  status: OrderStatus;
  totalAmount: number;
  shippingAddress?: string;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
  orderItems: OrderItem[];
}

export interface OrderItem {
  id: number;
  orderId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CreateOrderDto {
  customerId?: string;
  customerName?: string;
  shippingAddress?: string;
  notes?: string;
  orderItems: CreateOrderItemDto[];
}

export interface CreateOrderItemDto {
  productId: number;
  quantity: number;
  unitPrice: number;
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
  id: number;
  poNumber: string;
  supplierId: number;
  supplierName: string;
  orderDate: Date;
  expectedDeliveryDate?: Date;
  status: PurchaseOrderStatus;
  totalAmount: number;
  notes?: string;
  createdAt: Date;
  updatedAt?: Date;
  items: PurchaseOrderItem[];
}

export interface PurchaseOrderItem {
  id: number;
  purchaseOrderId: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface CreatePurchaseOrderDto {
  supplierId: number;
  expectedDeliveryDate?: Date;
  notes?: string;
  items: CreatePurchaseOrderItemDto[];
}

export interface CreatePurchaseOrderItemDto {
  productId: number;
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
  id: number;
  name: string;
  contactName: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateSupplierDto {
  name: string;
  contactName: string;
  email: string;
  phone: string;
  address: string;
}

export interface UpdateSupplierDto {
  name: string;
  contactName: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
}

// Warehouse Models
export interface Warehouse {
  id: number;
  name: string;
  location: string;
  capacity: number;
  isActive: boolean;
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
  isActive: boolean;
}

// Stock Movement Models
export enum MovementType {
  In = 0,
  Out = 1,
  Transfer = 2,
  Adjustment = 3
}

export interface StockMovement {
  id: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  movementType: MovementType;
  quantity: number;
  referenceNumber?: string;
  notes?: string;
  movementDate: Date;
  createdBy?: string;
}

export interface CreateStockMovementDto {
  productId: number;
  warehouseId: number;
  movementType: MovementType;
  quantity: number;
  referenceNumber?: string;
  notes?: string;
}

// Audit Log Models
export interface AuditLog {
  id: number;
  entityName: string;
  entityId: string;
  action: string;
  performedBy?: string;
  timestamp: Date;
  changes?: string;
  ipAddress?: string;
}
